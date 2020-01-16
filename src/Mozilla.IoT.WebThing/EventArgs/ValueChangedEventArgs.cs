using System;

namespace Mozilla.IoT.WebThing.EventArgs
{
    public class ValueChangedEventArgs : System.EventArgs
    {
        public ValueChangedEventArgs(object value, Property property)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public object Value { get; }
        public Property Property { get; }
    }
    
    
    public class ValueChangedEventArgs<T> : System.EventArgs
    {
        public ValueChangedEventArgs(T value, Property<T> property)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public T Value { get; }
        public Property<T> Property { get; }
    }
}
