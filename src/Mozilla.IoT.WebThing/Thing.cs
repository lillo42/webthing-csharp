using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing
{
    public partial class Thing
    {
        private const string DEFAULT_CONTEXT = "https://iot.mozilla.org/schemas";
        private const string DEFAULT_HREF_PREFIX = "/";

        private readonly IDictionary<string, Property> _properties = new Dictionary<string, Property>();

        private readonly IDictionary<string, AvailableAction> _availableActions =
            new Dictionary<string, AvailableAction>();

        private readonly IDictionary<string, AvailableEvent>
            _availableEvents = new Dictionary<string, AvailableEvent>();

        private readonly IDictionary<string, ICollection<Action>> _actions =
            new Dictionary<string, ICollection<Action>>();

        private readonly ICollection<Event> _events = new LinkedList<Event>();

        private readonly ISet<WebSocket> _subscribers = new HashSet<WebSocket>();

        /// <summary>
        /// The type context of the thing.
        /// </summary>
        public virtual string Context { get; }

        /// <summary>
        /// The name of the thing.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The description of the thing.
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// The type(s) of the thing.
        /// </summary>
        public virtual ICollection<object> Type { get; }

        /// <summary>
        /// The href of this thing's custom UI.
        /// </summary>
        public virtual string UiHref { get; set; }

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

                _properties.ForEach(x => x.Value.HrefPrefix = value);
                _actions.ForEach(x => x.Value.ForEach(y => y.HrefPrefix = value));
            }
        }

        protected internal Thing()
            : this(Guid.NewGuid().ToString())
        {
        }

        public Thing(string name)
            : this(name, null)
        {
        }

        public Thing(string name, ICollection<object> type)
            : this(name, type, null)
        {
        }

        public Thing(string name, ICollection<object> type, string description)
        {
            Name = name;
            Context = DEFAULT_CONTEXT;
            Type = type;
            Description = description;
            HrefPrefix = string.Empty;
            UiHref = null;
        }


        public virtual IDictionary<string, object> AsThingDescription()
        {
            var actions = new Dictionary<string, object>();

            _availableActions.ForEach(action =>
            {
                var link = new Dictionary<string, object>
                {
                    ["rel"] = "action", ["href"] = $"{HrefPrefix}actions/{action.Key}"
                };
                IDictionary<string, object> metadata = action.Value.Metadata;
                metadata.Add("links",);

                actions.Add(action.Key, metadata);
            });

            var events = new Dictionary<string, object>();

            _availableEvents.ForEach(@event =>
            {
                var link = new Dictionary<string, object>
                {
                    ["rel"] = "event", ["href"] = $"{HrefPrefix}event/{@event.Key}"
                };

                IDictionary<string, object> metadata = @event.Value.Metadata;

                metadata.Add("links", new List<IDictionary<string, object>> {link});

                @events.Add(@event.Key, metadata);
            });

            var obj = new Dictionary<string, object>
            {
                ["name"] = Name,
                ["href"] = HrefPrefix,
                ["@context"] = Context,
                ["@type"] = Type,
                ["properties"] = GetPropertyDescriptions(),
                ["actions"] = actions,
                ["events"] = events
            };

            if (Description != null)
            {
                obj.Add("description", Description);
            }

            var propertiesLink = new Dictionary<string, object>
            {
                ["rel"] = "properties", ["href"] = $"{HrefPrefix}properties"
            };

            var actionsLink = new Dictionary<string, object>
            {
                ["rel"] = "actions", 
                ["href"] = $"{HrefPrefix}actions"
            };

            var eventsLink = new Dictionary<string, object>
            {
                ["rel"] = "events", 
                ["href"] = $"{HrefPrefix}events"
            };

            var links = new List<IDictionary<string, object>>
            {
                propertiesLink, 
                actionsLink, 
                eventsLink
            };

            if (UiHref != null)
            {
                var uiLink = new Dictionary<string, object>
                {
                    ["rel"] = "alternate", 
                    ["mediaType"] = "text/html", 
                    ["href"] = UiHref
                };

                links.Add(uiLink);
            }

            obj.Add("links", links);

            return obj;
        }


        #region Property

        /// <summary>
        /// Get the thing's properties as a <see cref="IDictionary{TKey,TValue}"/>
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<string, object> GetPropertyDescriptions()
        {
            var obj = new Dictionary<string, object>();
            _properties.ForEach(p => obj.Add(p.Key, p.Value.AsPropertyDescription()));
            return obj;
        }

        /// <summary>
        /// Add a property to this thing.
        /// </summary>
        /// <param name="property">Property to add.</param>
        public virtual void AddProperty(Property property)
        {
            property.HrefPrefix = HrefPrefix;
            _properties.Add(property.Name, property);
        }

        /// <summary>
        /// Remove a property from this thing.
        /// </summary>
        /// <param name="property">Property to remove.</param>
        public virtual void RemoveProperty(Property property)
        {
            if (_properties.ContainsKey(property.Name))
            {
                _properties.Remove(property.Name);
            }
        }

        /// <summary>
        /// Find a property by name.
        /// </summary>
        /// <param name="propertyName">Name of the property to find</param>
        /// <returns>Property if found, else null.</returns>
        public virtual Property FindProperty(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
            {
                return _properties[propertyName];
            }

            return null;
        }

        /// <summary>
        /// Find a property by name.
        /// </summary>
        /// <param name="propertyName">Name of the property to find</param>
        /// <returns>Property if found, else null.</returns>
        /// <typeparam name="T">Type of the property value</typeparam>
        public virtual Property<T> FindProperty<T>(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
            {
                return _properties[propertyName] as Property<T>;
            }

            return null;
        }

        /// <summary>
        /// Get a property's value.
        /// </summary>
        /// <param name="propertyName">Name of the property to get the value of</param>
        /// <returns>Current property value if found, else null.</returns>
        public virtual object GetProperty(string propertyName)
            => FindProperty(propertyName)?.Value;

        /// <summary>
        /// Get a property's value.
        /// </summary>
        /// <param name="propertyName">Name of the property to get the value of</param>
        /// <returns>Current property value if found, else default.</returns>
        /// <typeparam name="T">Type of the property value</typeparam>
        public virtual T GetProperty<T>(string propertyName)
        {
            var property = FindProperty<T>(propertyName);
            return property != null ? property.Value : default;
        }

        /// <summary>
        /// Determine whether or not this thing has a given property.
        /// </summary>
        /// <param name="propertyName">The property to look for</param>
        /// <returns>Indication of property presence.</returns>
        public virtual bool ContainsProperty(string propertyName)
            => !string.IsNullOrEmpty(propertyName) && _properties.ContainsKey(propertyName);


        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="value">Value to set</param>
        /// <typeparam name="T">Type of the property value</typeparam>
        public virtual void SetProperty(string propertyName, object value)
        {
            Property property = FindProperty(propertyName);
            if (property != null)
            {
                property.Value = value;
            }
        }

        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="value">Value to set</param>
        /// <typeparam name="T">Type of the property value</typeparam>
        public virtual void SetProperty<T>(string propertyName, T value)
        {
            Property<T> property = FindProperty<T>(propertyName);
            if (property != null)
            {
                property.Value = value;
            }
        }

        /// <summary>
        /// Notify all subscribers of a property change.
        /// </summary>
        /// <param name="property">The property that changed</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public virtual async Task PropertyNotifyAsync(Property property, CancellationToken cancellation)
        {
            var json = new Dictionary<string, object>
            {
                ["messageType"] = "propertyStatus"
            };
            
            var inner = new Dictionary<string, object>
            {
                [property.Name] =  property.Value    
            };

            json.Add("data", inner);

            await NotifyAllAsync(_subscribers, json, cancellation);
        }

        #endregion

        #region Actions

        /// <summary>
        /// Get the thing's actions as a <see cref="Newtonsoft.Json.Linq.JArray"/>
        /// </summary>
        /// <param name="name">Optional action name to get descriptions for</param>
        /// <returns>Action descriptions.</returns>
        public virtual ICollection<IDictionary<string, object>> GetActionDescriptions(string name = null)
        {
            ICollection<IDictionary<string, object>> array = new LinkedList<IDictionary<string, object>>();

            if (name == null)
            {
                _actions.ForEach(list =>
                {
                    list.Value.ForEach(action =>
                    {
                        array.Add(action.AsActionDescription());
                    });
                });
            }
            else if (_actions.ContainsKey(name))
            {
                _actions[name].ForEach(action =>
                {
                    array.Add(action.AsActionDescription());
                });
            }

            return array;
        }

        /// <summary>
        /// Get an action.
        /// </summary>
        /// <param name="name">Name of the action</param>
        /// <param name="id">ID of the action</param>
        /// <returns>The requested action if found, else null.</returns>
        public virtual Action GetAction(string name, string id)
        {
            if (!_actions.ContainsKey(name))
            {
                return null;
            }

            return _actions[name].FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Perform an action on the thing.
        /// </summary>
        /// <param name="actionName">name of the action</param>
        /// <param name="input">Any action inputs</param>
        /// <param name="cancellation"></param>
        /// <returns>The action that was created.</returns>
        public virtual async Task<Action> PerformActionAsync(string actionName, object input,
            CancellationToken cancellation)
        {
            if (!_availableActions.ContainsKey(actionName))
            {
                return null;
            }

            AvailableAction availableAction = _availableActions[actionName];
            if (!availableAction.ValidateActionInput(input))
            {
                return null;
            }

            try
            {
                Action action = (Action)Activator.CreateInstance(availableAction.Type, this, input);

                action.HrefPrefix = HrefPrefix;
                await ActionNotifyAsync(action, cancellation)
                    .ConfigureAwait(false);

                _actions[actionName].Add(action);
                return action;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Notify all subscribers of an action status change.
        /// </summary>
        /// <param name="action">The action whose status changed</param>
        /// <param name="cancellation"></param>
        public virtual async Task ActionNotifyAsync(Action action, CancellationToken cancellation)
        {
            if (!_subscribers.Any())
            {
                return;
            }

            var json = new Dictionary<string, object>
            {
                ["messageType"] = "actionStatus",
                ["data"] = action.AsActionDescription()
            };

            await NotifyAllAsync(_subscribers, json, cancellation);
        }

        /// <summary>
        /// Remove an existing action. 
        /// </summary>
        /// <param name="name">name of the action</param>
        /// <param name="id">ID of the action</param>
        /// <returns>indicating the presence of the action.</returns>
        public virtual bool RemoveAction(string name, string id)
        {
            Action action = GetAction(name, id);
            if (action == null)
            {
                return false;
            }

            _actions[name].Remove(action);
            return true;
        }

        /// <summary>
        /// Add an available action.
        /// </summary>
        /// <param name="name">Name of the action</param>
        /// <param name="metadata">Action metadata, i.e. type, description, etc., as a <see cref="IDictionary{TKey,TValue}"/></param>
        public virtual void AddAvailableAction<T>(string name, IDictionary<string, object> metadata = null)
            where T : Action
        {
            if (metadata == null)
            {
                metadata = new Dictionary<string, object>();
            }

            _availableActions.Add(name, new AvailableAction(metadata, typeof(T)));
            _actions.Add(name, new LinkedList<Action>());
        }

        #endregion

        #region Event

        /// <summary>
        /// Get the thing's events as a <see cref="IDictionary{TKey,TValue}"/>
        /// </summary>
        /// <param name="name">Optional event name to get descriptions for</param>
        /// <returns>Event descriptions.</returns>
        public virtual ICollection<IDictionary<string, object>> GetEventDescriptions(string name = null)
        {
            ICollection<IDictionary<string, object>> array = new LinkedList<IDictionary<string, object>>();();

            if (name == null)
            {
                _events.ForEach(@event => array.Add(@event.AsEventDescription()));
            }
            else
            {
                _events.ForEach(@event =>
                {
                    if (@event.Name == name)
                    {
                        array.Add(@event.AsEventDescription());
                    }
                });
            }

            return array;
        }

        public async Task AddEventAsync(Event @event, CancellationToken cancellation)
        {
            _events.Add(@event);
            await EventNotifyAsync(@event, cancellation);
        }

        /// <summary>
        /// Notify all subscribers of an event.
        /// </summary>
        /// <param name="event">The event that occurred.</param>
        /// <param name="cancellation"></param>
        public virtual async Task EventNotifyAsync(Event @event, CancellationToken cancellation)
        {
            if (!_availableEvents.ContainsKey(@event.Name))
            {
                return;
            }

            var json = new Dictionary<string, object>
            {
                ["messageType"] = "event",
                ["data"] =  @event.AsEventDescription()
            };

            await NotifyAllAsync(_availableEvents[@event.Name]
                .Subscribers, json.ToString(), cancellation);
        }

        /// <summary>
        /// Add an available event.
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <param name="metadata">Event metadata, i.e. type, description, etc., as a <see cref="IDictionary{TKey,TValue}"/>></param>
        public virtual void AddAvailableEvent(string name, IDictionary<string, object> metadata = null)
        {
            if (metadata == null)
            {
                metadata = new Dictionary<string, object>();
            }

            _availableEvents.Add(name, new AvailableEvent(metadata));
        }

        /// <summary>
        /// Add a new websocket subscriber to an event.
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <param name="ws">The websocket</param>
        public virtual void AddEventSubscriber(string name, WebSocket ws)
        {
            if (_availableEvents.ContainsKey(name))
            {
                _availableEvents[name].AddSubscriber(ws);
            }
        }

        /// <summary>
        /// Remove a websocket subscriber from an event.
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <param name="ws">The websocket</param>
        public virtual void RemoveEventSubscriber(string name, WebSocket ws)
        {
            if (_availableEvents.ContainsKey(name))
            {
                _availableEvents[name].RemoveSubscriber(ws);
            }
        }

        #endregion

        #region Subscribe

        /// <summary>
        /// Add a new websocket subscriber.
        /// </summary>
        /// <param name="ws">The websocket</param>
        public virtual void AddSubscriber(WebSocket ws)
            => _subscribers.Add(ws);

        /// <summary>
        /// Remove a websocket subscriber 
        /// </summary>
        /// <param name="ws">The websocket</param>
        public virtual void RemoveSubscriber(WebSocket ws)
        {
            if (_subscribers.Contains(ws))
            {
                _subscribers.Remove(ws);
            }

            _availableEvents.ForEach(@event =>
            {
                RemoveEventSubscriber(@event.Key, ws);
            });
        }

        #endregion

        private static async Task NotifyAllAsync(IEnumerable<WebSocket> subscribers, string message,
            CancellationToken cancellation)
        {
            var write = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

            await subscribers
                .ForEachAsync(async subscriber =>
                {
                    await subscriber.SendAsync(write, WebSocketMessageType.Text, true, cancellation);
                });
        }
    }
}
