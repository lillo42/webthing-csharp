using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.SchemaValidations;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Collection of <see cref="ThingActionInformation"/>
    /// </summary>
    public class ActionCollection : IEnumerable<ThingActionInformation>
    {
        private readonly ConcurrentDictionary<Guid, ThingActionInformation> _actions;
        private readonly IJsonConvertible _inputConvertible;
        private readonly IJsonSchemaValidation _inputValidation;
        private readonly IActionInformationConvertible _actionInformationConvertible;

        /// <summary>
        /// Event to when Status of <see cref="ThingActionInformation"/> changed.
        /// </summary>
        public event EventHandler<ThingActionInformation>? Change;

        /// <summary>
        /// Initialize a new instance of <see cref="ActionCollection"/>.
        /// </summary>
        /// <param name="inputConvertible">The <see cref="IJsonConvertible"/>.</param>
        /// <param name="inputValidation"></param>
        /// <param name="actionInformationConvertible">The <see cref="IActionInformationConvertible"/>.</param>
        public ActionCollection(IJsonConvertible inputConvertible,
            IJsonSchemaValidation inputValidation, 
            IActionInformationConvertible actionInformationConvertible)
        {
            _actionInformationConvertible = actionInformationConvertible ?? throw new ArgumentNullException(nameof(actionInformationConvertible));
            _inputValidation = inputValidation ?? throw new ArgumentNullException(nameof(inputValidation));
            _inputConvertible = inputConvertible;
            _actions = new ConcurrentDictionary<Guid, ThingActionInformation>();
        }

        /// <summary>
        /// Try to add Action to collection.
        /// </summary>
        /// <param name="source">The <see cref="object"/> to be convert to <see cref="ThingActionInformation"/>.</param>
        /// <param name="info">The <see cref="ThingActionInformation"/> created.</param>
        /// <returns>Return true if could convert and added on collection, otherwise return false.</returns>
        public bool TryAdd(object source, [NotNullWhen(true)]out ThingActionInformation? info)
        {
            info = null;

            if (!_inputConvertible.TryConvert(source, out var inputProperty))
            {
                return false;
            }

            if (!_inputValidation.IsValid(inputProperty))
            {
                return false;
            }
            
            info = _actionInformationConvertible.Convert(inputProperty as Dictionary<string, object?>);
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
