using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class ThingParameterAttribute : Attribute
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        internal double? MinimumValue { get; private set; }

        /// <summary>
        /// Validates only if the instance is greater than or exactly equal to "minimum"
        /// </summary>
        public double Minimum
        {
            get => MinimumValue ?? 0;
            set => MinimumValue = value;
        }
        
        internal double? MaximumValue { get; private set; }

        /// <summary>
        ///  Validates only if the instance is less than or exactly equal to "maximum"
        /// </summary>
        public double Maximum
        {
            get => MaximumValue ?? 0;
            set => MaximumValue = value;
        }

        internal int? MultipleOfValue { get; set; }

        /// <summary>
        /// Valid only if it has a value strictly less than (not equal to) "exclusiveMaximum".
        /// </summary>
        public int MultipleOf
        {
            get => MultipleOfValue ?? 0;
            set => MultipleOfValue = value;
        }
        
        internal double? ExclusiveMinimumValue { get; set; }
        
        /// <summary>
        /// Valid only if it has a value strictly less than (not equal to) "exclusiveMaximum".
        /// </summary>
        public double ExclusiveMinimum
        {
            get => ExclusiveMinimumValue ?? 0;
            set => ExclusiveMinimumValue = value;
        }
        
        internal double? ExclusiveMaximumValue { get; set; }
        
        /// <summary>
        /// Valid only if it has a value strictly greater than (not equal to) "exclusiveMinimum"
        /// </summary>
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
        
        public object[]? Enum { get; set; }
    }
}
