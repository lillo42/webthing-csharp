using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    public class Thing
    {
        /// <summary>
        /// The type context of the thing.
        /// </summary>
        public virtual string Context { get; set; }

        /// <summary>
        /// The name of the thing.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The description of the thing.
        /// </summary>
        public virtual string Description { get; set; }

        private string _hrefPrefix;

        /// <summary>
        /// This thing's href.
        /// </summary>
        public virtual string HrefPrefix
        {
            get => string.IsNullOrEmpty(_hrefPrefix) ? DEFAULT_HREF_PREFIX : _hrefPrefix;
            set
            {
                _hrefPrefix = value;
                if (!_hrefPrefix.EndsWith(DEFAULT_HREF_PREFIX))
                {
                    _hrefPrefix += DEFAULT_HREF_PREFIX;
                }

                Properties.ForEach(property => property.HrefPrefix = value);
                Actions.ForEach(x => x.Value.ForEach(action => action.HrefPrefix = value));
            }
        }

        /// <summary>
        /// The href of this thing's custom UI.
        /// </summary>
        public virtual string UiHref { get; set; }

        public virtual IReadOnlyCollection<Property> Properties => _properties;
        public virtual IObservableCollection<Event> Events { get; set; }

        /// <summary>
        /// The type(s) of the thing.
        /// </summary>
        public virtual object Type { get; set; }

        public virtual IReadOnlyDictionary<string, (Type type, IDictionary<string, object> metadata)> ActionsTypeInfo =>
            _actionsTypeInfo;

        public virtual ConcurrentDictionary<Guid, WebSocket> Subscribers { get; } =
            new ConcurrentDictionary<Guid, WebSocket>();

        internal static LinkedList<Type> ActionsTypes { get; } = new LinkedList<Type>();

        internal ConcurrentDictionary<string, WebSocket> EventSubscribers { get; } =
            new ConcurrentDictionary<string, WebSocket>();

        internal ConcurrentDictionary<string, LinkedList<Action>> Actions { get; } =
            new ConcurrentDictionary<string, LinkedList<Action>>();
        
        internal IJsonSchemaValidator JsonSchemaValidator { get; set; }
        private readonly LinkedList<Property> _properties = new LinkedList<Property>();

        private readonly Dictionary<string, (Type type, IDictionary<string, object> metadata)> _actionsTypeInfo =
            new Dictionary<string, (Type type, IDictionary<string, object> metadata)>();

        public Thing()
        {
            Context = DEFAULT_CONTEXT;
        }

        #region Actions

        protected virtual void AddAction<T>(string name, IDictionary<string, object> metadata = null)
            where T : Action
        {
            _actionsTypeInfo.Add(name, (typeof(T), metadata));
            Actions.TryAdd(name, new LinkedList<Action>());
            ActionsTypes.AddLast(typeof(T));
        }

        #endregion
        
        #region Property

        public virtual void AddProperty(Property property)
        {
            _properties.AddLast(property);

            if (property.Thing == null)
            {
                property.Thing = this;
            }

            property.HrefPrefix = HrefPrefix;
        }

        public virtual void SetProperty(Property property, object value)
        {
            if (property != null && JsonSchemaValidator.IsValid(value, property.Metadata))
            {
                property.Value = value;
            }
        }

        #endregion
        
    }
}
