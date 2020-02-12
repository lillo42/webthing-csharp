using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mozilla.IoT.WebThing
{
    public class Property<T> : Property,  IEquatable<Property<T>>
    {
        public Property()
        {
        }

        public Property(Thing thing, string name, object value, IDictionary<string, object> metadata) 
            : base(thing, name, value, metadata)
        {
        }

        public Property(string name, object value, IDictionary<string, object> metadata) 
            : base(name, value, metadata)
        {
        }

        private T _value;

        public new virtual T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        internal override Type Type => typeof(T);

        public new event EventHandler<ValueChangedEventArgs<T>> ValuedChanged;
        
        protected override void OnValueChanged()
        {
            var @event = ValuedChanged;
            @event?.Invoke(this, new ValueChangedEventArgs<T>(Value, this));
            base.OnValueChanged();
        }

        public bool Equals(Property<T> other) 
            => base.Equals(other);
    }
    
    [DebuggerDisplay("Value = {Value}")]
    public class Property : IEquatable<Property>
    {
        public Property()
        {
            
        }

        public Property(Thing thing, string name, object value, IDictionary<string, object> metadata)
        {
            Thing = thing;
            Name = name;
            _value = value;
            Metadata = metadata;
        }
        
        public Property(string name, object value, IDictionary<string, object> metadata)
        {
            _value = value;
            Name = name;
            Metadata = metadata;
        }


        public virtual Thing Thing { get;  set; }
        public virtual string Name { get; set; }
        public virtual string Href => $"/properties/{Name}";
        public virtual string HrefPrefix { get; set; }
        private object _value;
        public virtual object Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged();
            }
        }
        public virtual IDictionary<string, object> Metadata { get; set; }
        internal virtual Type Type => typeof(object);
        public event EventHandler<ValueChangedEventArgs> ValuedChanged;
        
        protected virtual void OnValueChanged()
        {
            var @event = ValuedChanged; 
            @event?.Invoke(this, new ValueChangedEventArgs(Value, this));
        }

        public bool Equals(Property other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(_value, other._value) 
                   && Equals(Thing, other.Thing) 
                   && string.Equals(Name, other.Name) 
                   && string.Equals(HrefPrefix, other.HrefPrefix) 
                   && Equals(Metadata, other.Metadata);
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

            return obj.GetType() == GetType() && Equals((Property) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_value?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Thing?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (HrefPrefix?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Metadata?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
