using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using Mozilla.IoT.WebThing.Collections;
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

        public ICollection<Property> Properties { get; }
        public IObservableCollection<Event> Events { get; internal set; }

        /// <summary>
        /// The type(s) of the thing.
        /// </summary>
        public virtual object Type { get; set; }

        public IReadOnlyDictionary<string, (Type type, IDictionary<string, object> metadata)> ActionsTypeInfo =>
            _actionsTypeInfo;

        public virtual ConcurrentDictionary<Guid, WebSocket> Subscribers { get; } =
            new ConcurrentDictionary<Guid, WebSocket>();

        internal ConcurrentDictionary<string, WebSocket> EventSubscribers { get; } =
            new ConcurrentDictionary<string, WebSocket>();

        internal ObservableActionCollection Actions { get; } = new ObservableActionCollection();

        private readonly Dictionary<string, (Type type, IDictionary<string, object> metadata)> _actionsTypeInfo =
            new Dictionary<string, (Type type, IDictionary<string, object> metadata)>();

        public Thing()
        {
            Context = DEFAULT_CONTEXT;
            Properties = new PropertyCollection(this);
        }

        #region Actions

        protected virtual void AddAction<T>(string name, IDictionary<string, object> metadata = null)
            where T : Action
        {
            _actionsTypeInfo.Add(name, (typeof(T), metadata));
        }

        #endregion
    }
}
