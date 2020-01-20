using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ThingPropertyAttribute : Attribute
    {
        public string? Name { get; set; }
        public bool Ignore { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string[]? Type { get; set; }
        public bool IsReadOnly { get; set; }
        public object[]? Enum { get; set; }
        internal float? MinimumValue { get; set; }

        public float Minimum
        {
            get => MinimumValue ?? 0;
            set => MinimumValue = value;
        }
        
        internal float? MaximumValue { get; set; }

        public float Maximum
        {
            get => MaximumValue ?? 0;
            set => MaximumValue = value;
        }


        internal int? MultipleOfValue { get; set; }

        public int MultipleOf
        {
            get => MultipleOfValue ?? 0;
            set => MultipleOfValue = value;
        }
    }
}
