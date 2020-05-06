using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Collection of <see cref="ThingActionInformation"/>
    /// </summary>
    public class ActionCollection : IEnumerable<ThingActionInformation>
    {
        private readonly ConcurrentDictionary<Guid, ThingActionInformation> _actions;
        private readonly DictionaryInputConvert _inputConvert;
        private readonly IActionInfoFactory _actionInfoFactory;

        /// <summary>
        /// Event to when Status of <see cref="ThingActionInformation"/> changed.
        /// </summary>
        public event EventHandler<ThingActionInformation>? Change;

        /// <summary>
        /// Initialize a new instance of <see cref="ActionCollection"/>.
        /// </summary>
        /// <param name="inputConvert">The <see cref="DictionaryInputConvert"/>.</param>
        /// <param name="actionInfoFactory">The <see cref="IActionInfoFactory"/>.</param>
        public ActionCollection(DictionaryInputConvert inputConvert, IActionInfoFactory actionInfoFactory)
        {
            _actionInfoFactory = actionInfoFactory ?? throw new ArgumentNullException(nameof(actionInfoFactory));
            _inputConvert = inputConvert;
            _actions = new ConcurrentDictionary<Guid, ThingActionInformation>();
        }

        /// <summary>
        /// Try to add Action to collection.
        /// </summary>
        /// <param name="value">The <see cref="object"/> to be convert to <see cref="ThingActionInformation"/>.</param>
        /// <param name="info">The <see cref="ThingActionInformation"/> created.</param>
        /// <returns>Return true if could convert and added on collection, otherwise return false.</returns>
        public bool TryAdd(object value, [NotNullWhen(true)]out ThingActionInformation? info)
        {
            info = null;
            Dictionary<string, object>? inputValues = null;
            if (element.TryGetProperty("input", out var inputProperty))
            {
                if (inputProperty.ValueKind == JsonValueKind.Object 
                    && !_inputConvert.TryConvert(inputProperty, out inputValues!))
                {
                    return false;
                }
            }

            inputValues ??= new Dictionary<string, object>();

            info = _actionInfoFactory.CreateActionInfo(inputValues!);
            if (info == null)
            {
                return false;
            }
            
            info.StatusChanged += OnStatusChange;
            
            return _actions.TryAdd(info.GetId(), info);
        }

        /// <summary>
        /// Try to get <see cref="ThingActionInformation"/> by Id.
        /// </summary>
        /// <param name="id">The id of <see cref="ThingActionInformation"/>.</param>
        /// <param name="action">The <see cref="ThingActionInformation"/>.</param>
        /// <returns>Return true if could get <see cref="ThingActionInformation"/> by Id, otherwise return false.</returns>
        public bool TryGetValue(Guid id, [NotNullWhen(true)]out ThingActionInformation? action)
            => _actions.TryGetValue(id, out action);
        
        /// <summary>
        /// Try to remove <see cref="ThingActionInformation"/> by Id.
        /// </summary>
        /// <param name="id">The id of <see cref="ThingActionInformation"/>.</param>
        /// <param name="action">The <see cref="ThingActionInformation"/>.</param>
        /// <returns>Return true if could remove <see cref="ThingActionInformation"/> by Id, otherwise return false.</returns>
        public bool TryRemove(Guid id, [NotNullWhen(true)]out ThingActionInformation? action)
        {
            var result = _actions.TryRemove(id, out action);
            if (result && action != null)
            {
                action.StatusChanged -= OnStatusChange;   
            }
            
            return result;
        }
        
        private void OnStatusChange(object? sender, EventArgs args)
        {
            var change = Change;
            change?.Invoke(this, (ThingActionInformation)sender!);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ThingActionInformation> GetEnumerator() 
            => _actions.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}
