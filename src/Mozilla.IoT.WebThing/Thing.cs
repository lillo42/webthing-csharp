using System;
using System.Collections.Generic;
using System.Linq;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// The thing
    /// </summary>
    public class Thing : IEquatable<Thing>
    {
        private readonly ICollection<Property> _properties;
        private readonly IDictionary<string, (Type action, ActionDescription description)> _actions;
        private readonly ICollection<EventDescription> _events;

        #region Properties
        /// <summary>
        /// URI for a schema repository which defines standard schemas for common "types" of device capabilities.
        /// </summary>
        public virtual string Context { get; set; } = DefaultContext;

        /// <summary>
        /// The name of the thing.
        /// It's used to generated Id.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The title member is a human friendly string which describes the device.
        /// This can be set to any value by the device creator and may include a brand name or model number.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// The description member is a human friendly string which describes the device and its functions.
        /// This can be set to any value by the device creator.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The names of schemas for types of capabilities a device supports.
        /// </summary>
        public virtual string[] Type { get; set; }

        /// <summary>
        /// A <see cref="IEnumerable{T}"/> of <see cref="Property"/> which describe the attributes of the device.
        /// </summary>
        public IEnumerable<Property> Properties => _properties;

        /// <summary>
        /// A <see cref="IEnumerable{T}"/> of <see cref="Event"/> which define the types of events which may be emitted by a device
        /// </summary>
        public IEnumerable<EventDescription> EventDescription => _events;

        /// <summary>
        /// A <see cref="IEnumerable{T}"/> of <see cref="Action"/>> which describe functions that can be carried out on a device.
        /// </summary>
        public IEnumerable<ActionDescription> ActionDescription => _actions.Values
            .Select(x => x.description);

        #endregion

        #region Constructor

        public Thing()
        {
            Name = Guid.NewGuid().ToString();
            _properties = new LinkedList<Property>();
            _actions = new Dictionary<string, (Type action, ActionDescription description)>();
            _events = new LinkedList<EventDescription>();
        }

        public Thing(string name) 
            : this()
        {
            Name = name;
        }
        
        public Thing(string name, string title) 
            : this()
        {
            Name = name;
            Title = title;
        }
        
        public Thing(string name, string title, string description) 
            : this()
        {
            Name = name;
            Title = title;
            Description = description;
        }
        
        public Thing(string name, string title, string description, string[] type) 
            : this()
        {
            Name = name;
            Title = title;
            Description = description;
            Type = type;
        }

        #endregion

        #region IEquatable

        public bool Equals(Thing? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _properties.Equals(other._properties)
                   && EventDescription.Equals(other.EventDescription)
                   && ActionDescription.Equals(other.ActionDescription)
                   && Context == other.Context
                   && Name == other.Name
                   && Description == other.Description
                   && Type.Equals(other.Type);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Thing thing)
            {
                return Equals(this, thing);
            }

            return false;
        }

        public override int GetHashCode()
            => HashCode.Combine(_properties, EventDescription, ActionDescription, Context, Name, Description, Type);

        #endregion

        #region Operator

        public static bool operator ==(Thing thing1, Thing thing2) =>
            thing1 switch
            {
                null when thing2 is null => true,
                null => false,
                _ => thing1.Equals(thing2)
            };

        public static bool operator !=(Thing thing1, Thing thing2) 
            => !(thing1 == thing2);

        #endregion

        #region Actions

        public void RegisterAction<T>(ActionDescription description)
            where T : Action
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }
            
            RegisterAction<T>(description, description.Name);
        }

        public void RegisterAction<T>(ActionDescription description, string name)
            where T : Action
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _actions.Add(name, (typeof(T), description));
        }

        public void RegisterAction(ActionDescription description, Type actionType)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (actionType == null)
            {
                throw new ArgumentNullException(nameof(actionType));
            }

           
            RegisterAction(description, actionType, description.Name);
        }
        
        public void RegisterAction(ActionDescription description, Type actionType, string name)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (actionType == null)
            {
                throw new ArgumentNullException(nameof(actionType));
            }

            if (!actionType.IsInstanceOfType(typeof(Action)))
            {
                throw new ArgumentException("Should inheritance of Action", nameof(actionType));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            
            _actions.Add(name, (actionType, description));
        }

        #endregion

        #region Event
        public void RegisterEvent(EventDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            _events.Add(description);
        }
        #endregion

        #region Properties

        public void RegisterProperty<T>(T property)
            where T : Property
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _properties.Add(property);
        }

        #endregion
    }
}
