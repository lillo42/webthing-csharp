using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mozilla.IoT.WebThing.DebugView;

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

        public new virtual T Value
        {
            get
            {
                if (base.Value == null)
                {
                    return default;
                }

                return (T)base.Value;
            }
            set => base.Value = value;
        }

        internal override Type Type => typeof(T);

        public new event EventHandler<ValueChangedEventArgs<T>> ValuedChanged;
        
        protected override void OnValueChanged()
        {
            ValuedChanged?.Invoke(this, new ValueChangedEventArgs<T>(Value));
            base.OnValueChanged();
        }

        public bool Equals(Property<T> other) 
            => base.Equals(other);
    }
    
    [DebuggerTypeProxy(typeof(PropertyProxyDebugView))]
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
            ValuedChanged?.Invoke(this, new ValueChangedEventArgs(Value));
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
    
    public class ValueChangedEventArgs : EventArgs
    {
        public object Value { get; }

        public ValueChangedEventArgs(object value)
            => Value = value;
    }

    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Value { get; }

        public ValueChangedEventArgs(T value)
            => Value = value;
    }
}
