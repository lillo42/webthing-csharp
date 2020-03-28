using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Collection of <see cref="ActionInfo"/>
    /// </summary>
    public class ActionCollection : IEnumerable<ActionInfo>
    {
        private readonly ConcurrentDictionary<Guid, ActionInfo> _actions;
        private readonly DictionaryInputConvert _inputConvert;
        private readonly IActionInfoFactory _actionInfoFactory;

        /// <summary>
        /// Event to when Status of <see cref="ActionInfo"/> changed.
        /// </summary>
        public event EventHandler<ActionInfo>? Change;

        /// <summary>
        /// Initialize a new instance of <see cref="ActionCollection"/>.
        /// </summary>
        /// <param name="inputConvert">The <see cref="DictionaryInputConvert"/>.</param>
        /// <param name="actionInfoFactory">The <see cref="IActionInfoFactory"/>.</param>
        public ActionCollection(DictionaryInputConvert inputConvert, IActionInfoFactory actionInfoFactory)
        {
            _actionInfoFactory = actionInfoFactory ?? throw new ArgumentNullException(nameof(actionInfoFactory));
            _inputConvert = inputConvert;
            _actions = new ConcurrentDictionary<Guid, ActionInfo>();
        }

        /// <summary>
        /// Try to add Action to collection.
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/> to be convert to <see cref="ActionInfo"/>.</param>
        /// <param name="info">The <see cref="ActionInfo"/> created.</param>
        /// <returns>Return true if could convert and added on collection, otherwise return false.</returns>
        public bool TryAdd(JsonElement element, [NotNullWhen(true)]out ActionInfo? info)
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
        /// Try to get <see cref="ActionInfo"/> by Id.
        /// </summary>
        /// <param name="id">The id of <see cref="ActionInfo"/>.</param>
        /// <param name="action">The <see cref="ActionInfo"/>.</param>
        /// <returns>Return true if could get <see cref="ActionInfo"/> by Id, otherwise return false.</returns>
        public bool TryGetValue(Guid id, [NotNullWhen(true)]out ActionInfo? action)
            => _actions.TryGetValue(id, out action);
        
        /// <summary>
        /// Try to remove <see cref="ActionInfo"/> by Id.
        /// </summary>
        /// <param name="id">The id of <see cref="ActionInfo"/>.</param>
        /// <param name="action">The <see cref="ActionInfo"/>.</param>
        /// <returns>Return true if could remove <see cref="ActionInfo"/> by Id, otherwise return false.</returns>
        public bool TryRemove(Guid id, [NotNullWhen(true)]out ActionInfo? action)
        {
            var result =_actions.TryRemove(id, out action);
            if (result && action != null)
            {
                action.StatusChanged -= OnStatusChange;   
            }
            
            return result;
        }
        
        private void OnStatusChange(object? sender, EventArgs args)
        {
            var change = Change;
            change?.Invoke(this, (ActionInfo)sender!);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ActionInfo> GetEnumerator() 
            => _actions.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}
