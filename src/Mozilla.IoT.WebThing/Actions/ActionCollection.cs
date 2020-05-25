using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

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
        private readonly IConvertible _convertible;
        private readonly IActionInformationFactory _actionInformationFactory;

        /// <summary>
        /// Event to when Status of <see cref="ThingActionInformation"/> changed.
        /// </summary>
        public event EventHandler<ThingActionInformation>? Change;

        /// <summary>
        /// Initialize a new instance of <see cref="ActionCollection"/>.
        /// </summary>
        /// <param name="inputConvertible">The <see cref="IJsonConvertible"/>.</param>
        /// <param name="inputValidation"></param>
        /// <param name="actionInformationFactory">The <see cref="IActionInformationFactory"/>.</param>
        /// <param name="convertible"></param>
        public ActionCollection(IJsonConvertible inputConvertible,
            IJsonSchemaValidation inputValidation, 
            IConvertible convertible,
            IActionInformationFactory actionInformationFactory)
        {
            _actionInformationFactory = actionInformationFactory ?? throw new ArgumentNullException(nameof(actionInformationFactory));
            _convertible = convertible ?? throw new ArgumentNullException(nameof(convertible));
            _inputValidation = inputValidation ?? throw new ArgumentNullException(nameof(inputValidation));
            _inputConvertible = inputConvertible ?? throw new ArgumentNullException(nameof(inputConvertible));
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

            info = _actionInformationFactory.Convert(_convertible.Convert(inputProperty) as Dictionary<string, object?>);

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

            if (action == null)
            {
                return false;
            }
            
            action.StatusChanged -= OnStatusChange;
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
