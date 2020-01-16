using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mozilla.IoT.WebThing.EventArgs;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// Describes an attribute of a <see cref="Thing"/>
    /// </summary>
    [DebuggerDisplay("Title = {Title}, Value = {Value}")]
    public class Property : IEquatable<Property>
    {
        private object _value;

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        
        #region Properties

        /// <summary>
        /// The name of property
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Description of property
        /// </summary>
        public PropertyDescription Description { get; }
        
        /// <summary>
        /// The value of property
        /// </summary>
        public object? Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged();
            }
        }
        
        #endregion

        #region Constructor
        
        public Property(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = new PropertyDescription(name);
        }
        
        public Property(string name, PropertyDescription description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public Property(string name, PropertyDescription description, object? value)
        {
            _value = value;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        #endregion
        
        #region Operator

        public static bool operator ==(Property obj1, Property obj2) =>
            obj1 switch
            {
                null when obj2 is null => true,
                null => false,
                _ => obj1.Equals(obj2)
            };

        public static bool operator !=(Property obj1, Property obj2) 
            => !(obj1 == obj2);

        #endregion

        #region IEquatable

        public bool Equals(Property? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _value.Equals(other._value) && Name == other.Name && Description.Equals(other.Description);
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

            if (obj is Property property)
            {
                return Equals(property);
            }

            return false;
        }

        public override int GetHashCode() 
            => HashCode.Combine(_value, Name, Description);

        #endregion
        
        protected virtual void OnValueChanged()
        {
            var @event = ValueChanged;
            @event?.Invoke(this, new ValueChangedEventArgs(_value, this));
        }
    }

    /// <summary>
    /// Describes an attribute of a <see cref="Thing"/>
    /// </summary>
    [DebuggerDisplay("Title = {Title}, Value = {Value}")]
    public class Property<T> : Property
    {
        public new event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
        
        #region Properties

        /// <summary>
        /// The value of property
        /// </summary>
        [MaybeNull]
        public new T Value
        {
            get
            {
                var value = base.Value;
                if (value == null)
                {
                    return default;
                }
                return (T)value;
            }
            
            set => base.Value = value;
        }

        #endregion
        
        #region Constructor
        
        public Property(string name) 
            : base(name)
        {
        }

        public Property(string name, PropertyDescription description)
            : base(name, description)
        {
        }

        public Property(string name, PropertyDescription description, T value) 
            : base(name, description, value)
        {
        }
        
        #endregion

        #region Operator
        public static bool operator ==(Property<T> obj1, Property<T> obj2) =>
            obj1 switch
            {
                null when obj2 is null => true,
                null => false,
                _ => obj1.Equals(obj2)
            };

        public static bool operator !=(Property<T> obj1, Property<T> obj2) 
            => !(obj1 == obj2);

        #endregion

        protected override void OnValueChanged()
        {
            var @event = ValueChanged;
            @event?.Invoke(this, new ValueChangedEventArgs<T>(Value, this));
            base.OnValueChanged();
        }
    }
}
