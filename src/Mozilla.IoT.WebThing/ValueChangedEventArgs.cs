using System;

namespace Mozilla.IoT.WebThing
{
    public class ValueChangedEventArgs : EventArgs
    {
        public object Value { get; }
        public Property Property { get; }
        
        public ValueChangedEventArgs(object value, Property property)
        {
            Value = value;
            Property = property;
        }
    }

    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Value { get; }
        public Property<T> Property { get; }

        public ValueChangedEventArgs(T value, Property<T> property)
        {
            Value = value;
            Property = property;
        }
    }
}
