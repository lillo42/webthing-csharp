using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using Mozilla.IoT.WebThing.Collections;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    public class Thing : IEquatable<Thing>
    {
        /// <summary>
        /// The type context of the thing.
        /// </summary>
        public virtual string Context => DEFAULT_CONTEXT;

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

        internal virtual ConcurrentDictionary<string, AvailableEvent> AvailableEvent { get; } =
            new ConcurrentDictionary<string, AvailableEvent>();

        public virtual ConcurrentDictionary<Guid, WebSocket> Subscribers { get; } =
            new ConcurrentDictionary<Guid, WebSocket>();

        internal virtual ObservableActionCollection Actions { get; } = new ObservableActionCollection();

        private readonly Dictionary<string, (Type type, IDictionary<string, object> metadata)> _actionsTypeInfo =
            new Dictionary<string, (Type type, IDictionary<string, object> metadata)>();

        public Thing()
        {
            Properties = new PropertyCollection(this);
        }

        #region Actions
        
        public virtual void AddAction<T>(IDictionary<string, object> metadata = null)
            where T : Action 
            => AddAction<T>(typeof(T).Name.Replace("Action", ""), metadata);

        public virtual void AddAction<T>(string name, IDictionary<string, object> metadata = null)
            where T : Action 
            => _actionsTypeInfo.Add(name, (typeof(T), metadata));

        public virtual void AddEvent<T>(IDictionary<string, object> metadata = null)
            where T : Event
        {
            string name = typeof(T).Name.Remove(typeof(T).Name.Length - 5);
            AvailableEvent.TryAdd(name, new AvailableEvent(name, metadata));
        }

        public virtual void AddEvent(string name, IDictionary<string, object> metadata = null)
            => AvailableEvent.TryAdd(name, new AvailableEvent(name, metadata));

        #endregion

        public bool Equals(Thing other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(_hrefPrefix, other._hrefPrefix)
                   && Equals(_actionsTypeInfo, other._actionsTypeInfo)
                   && string.Equals(Context, other.Context)
                   && string.Equals(Name, other.Name)
                   && string.Equals(Description, other.Description)
                   && string.Equals(UiHref, other.UiHref)
                   && Equals(Properties, other.Properties)
                   && Equals(Events, other.Events)
                   && Equals(Type, other.Type)
                   && Equals(Subscribers, other.Subscribers)
                   && Equals(AvailableEvent, other.AvailableEvent)
                   && Equals(Actions, other.Actions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Thing)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_hrefPrefix?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (_actionsTypeInfo?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Context?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Description?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (UiHref?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Properties?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Events?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Subscribers?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (AvailableEvent?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Actions?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
