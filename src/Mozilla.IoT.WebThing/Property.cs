using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public class Property<T> : Property
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

        public virtual new T Value
        {
            get => (T)base.Value;
            set => base.Value = value;
        }
        
        public new event EventHandler<ValueChangedEventArgs<T>> ValuedChanged;
        
        protected override void OnValueChanged()
        {
            ValuedChanged?.Invoke(this, new ValueChangedEventArgs<T>(Value));
            base.OnValueChanged();
        }
    }
    
    public class Property
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
        public event EventHandler<ValueChangedEventArgs> ValuedChanged;
        
        protected virtual void OnValueChanged()
        {
            ValuedChanged?.Invoke(this, new ValueChangedEventArgs(Value));
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
