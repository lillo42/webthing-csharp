using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ThingPropertyAttribute : Attribute
    {
        public string? Name { get; set; }
        public bool Ignore { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string[]? Type { get; set; }
        public bool IsReadOnly { get; set; }
        internal bool? IsWriteOnlyValue { get; set; }

        public bool IsWriteOnly
        {
            get => IsWriteOnlyValue.GetValueOrDefault();
            set => IsWriteOnlyValue = value;
        }
        
        public object[]? Enum { get; set; }
        internal double? MinimumValue { get; set; }
        public double Minimum
        {
            get => MinimumValue ?? 0;
            set => MinimumValue = value;
        }
        
        internal double? MaximumValue { get; set; }
        public double Maximum
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

        internal double? ExclusiveMinimumValue { get; set; }
        public double ExclusiveMinimum
        {
            get => ExclusiveMinimumValue ?? 0;
            set => ExclusiveMinimumValue = value;
        }
        
        internal double? ExclusiveMaximumValue { get; set; }
        public double ExclusiveMaximum
        {
            get => ExclusiveMaximumValue ?? 0;
            set => ExclusiveMaximumValue = value;
        }
        
        internal int? MinimumLengthValue { get; set; }
        public int MinimumLength
        {
            get => MinimumLengthValue.GetValueOrDefault();
            set => MinimumLengthValue = value;
        }

        internal int? MaximumLengthValue { get; set; }
        public int MaximumLength
        {
            get => MaximumLengthValue.GetValueOrDefault();
            set => MaximumLengthValue = value;
        }
        
        public string? Pattern { get; set; }
    }
}
