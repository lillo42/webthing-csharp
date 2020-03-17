using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    /// <summary>
    /// Action parameter information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ThingParameterAttribute : Attribute
    {
        /// <summary>
        /// Action parameter title.
        /// </summary>
        public string? Title { get; set; }
        
        /// <summary>
        /// Action parameter description.
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Unit of Action parameter.
        /// </summary>
        public string? Unit { get; set; }
        internal double? MinimumValue { get; private set; }

        /// <summary>
        /// Minimum accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double Minimum
        {
            get => MinimumValue ?? 0;
            set => MinimumValue = value;
        }
        
        internal double? MaximumValue { get; private set; }

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
        
        /// <summary>
        /// Possible value this action parameter should have.
        /// </summary>
        public object[]? Enum { get; set; }
    }
}
