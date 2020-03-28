using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    /// <summary>
    /// Property information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ThingPropertyAttribute : Attribute
    {
        /// <summary>
        /// Custom property name.
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// If property should be ignore.
        /// </summary>
        public bool Ignore { get; set; }
        
        /// <summary>
        /// Property title.
        /// </summary>
        public string? Title { get; set; }
        
        /// <summary>
        /// Property description.
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Unit of property value.
        /// </summary>
        public string? Unit { get; set; }
        
        /// <summary>
        /// Property types.
        /// </summary>
        public string[]? Type { get; set; }
        
        /// <summary>
        /// If property is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }
        
        /// <summary>
        /// If property is write-only.
        /// </summary>
        internal bool? IsWriteOnlyValue { get; set; }

        /// <summary>
        /// If property is write-only.
        /// </summary>
        public bool IsWriteOnly
        {
            get => IsWriteOnlyValue.GetValueOrDefault();
            set => IsWriteOnlyValue = value;
        }
        
        /// <summary>
        /// Possible value this property should have.
        /// </summary>
        public object[]? Enum { get; set; }
        internal double? MinimumValue { get; set; }
        
        /// <summary>
        /// Minimum accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double Minimum
        {
            get => MinimumValue ?? 0;
            set => MinimumValue = value;
        }
        
        internal double? MaximumValue { get; set; }
        
        /// <summary>
        /// Maximum accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double Maximum
        {
            get => MaximumValue ?? 0;
            set => MaximumValue = value;
        }
        
        internal int? MultipleOfValue { get; set; }
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public int MultipleOf
        {
            get => MultipleOfValue ?? 0;
            set => MultipleOfValue = value;
        }

        internal double? ExclusiveMinimumValue { get; set; }
        
        /// <summary>
        /// Exclusive minimum (less than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double ExclusiveMinimum
        {
            get => ExclusiveMinimumValue ?? 0;
            set => ExclusiveMinimumValue = value;
        }
        
        internal double? ExclusiveMaximumValue { get; set; }
        /// <summary>
        /// Exclusive maximum (great than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double ExclusiveMaximum
        {
            get => ExclusiveMaximumValue ?? 0;
            set => ExclusiveMaximumValue = value;
        }
        
        internal int? MinimumLengthValue { get; set; }
        /// <summary>
        /// Minimum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        public int MinimumLength
        {
            get => MinimumLengthValue.GetValueOrDefault();
            set => MinimumLengthValue = value;
        }

        internal int? MaximumLengthValue { get; set; }
        /// <summary>
        /// Maximum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        public int MaximumLength
        {
            get => MaximumLengthValue.GetValueOrDefault();
            set => MaximumLengthValue = value;
        }
        
        /// <summary>
        /// Pattern this action parameter must have.
        /// This property should be use only for string.
        /// </summary>
        public string? Pattern { get; set; }
    }
}
