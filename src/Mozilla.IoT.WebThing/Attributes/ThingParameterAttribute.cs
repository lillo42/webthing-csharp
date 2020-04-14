using System;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Attributes
{
    /// <summary>
    /// Action parameter information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ThingParameterAttribute : Attribute, IJsonSchema
    {
        /// <summary>
        /// Action parameter name.
        /// </summary>
        public string? Name { get; set; }
        
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
        
        /// <inheritdoc/>
        bool? IJsonSchema.IsReadOnly { get; set; }
        
        /// <inheritdoc/>
        bool? IJsonSchema.IsWriteOnly { get; set; }
        
        /// <inheritdoc/>
        public object[]? Enum { get; set; }

        private decimal? _minimum;
        decimal? IJsonSchema.Minimum
        {
            get => _minimum;
            set => _minimum = value;
        }

        /// <summary>
        /// Minimum accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double Minimum
        {
            get => Convert.ToDouble(_minimum);
            set => _minimum = Convert.ToDecimal(value);
        }
        
        private decimal? _maximum;
        decimal? IJsonSchema.Maximum
        {
            get => _maximum;
            set => _maximum = value;
        }
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double Maximum 
        {
            get => Convert.ToDouble(_maximum);
            set => _maximum = Convert.ToDecimal(value);
        }
        
        private decimal? _multipleOf;
        decimal? IJsonSchema.MultipleOf
        {
            get => _multipleOf;
            set => _multipleOf = value;
        }
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double MultipleOf
        {
            get => Convert.ToDouble(_multipleOf);
            set => _multipleOf = Convert.ToDecimal(value);
        }
        
        private decimal? _exclusiveMinimum;
        decimal? IJsonSchema.ExclusiveMinimum
        {
            get => _exclusiveMinimum;
            set => _exclusiveMinimum = value;
        }

        /// <summary>
        /// Exclusive minimum (less than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double ExclusiveMinimum 
        {
            get => Convert.ToDouble(_exclusiveMinimum);
            set => _exclusiveMinimum = Convert.ToDecimal(value);
        }
        
        private decimal? _exclusiveMaximum;
        decimal? IJsonSchema.ExclusiveMaximum
        {
            get => _exclusiveMaximum;
            set => _exclusiveMaximum = value;
        }
        
        /// <summary>
        /// Exclusive maximum (great than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double ExclusiveMaximum 
        {
            get => Convert.ToDouble(_exclusiveMaximum);
            set => _exclusiveMaximum = Convert.ToDecimal(value);
        }
        
        private int? _minimumLength;
        int? IJsonSchema.MinimumLength
        {
            get => _minimumLength;
            set => _minimumLength = value;
        }
        
        /// <summary>
        /// Minimum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        public int MinimumLength
        {
            get => _minimumLength ?? 0;
            set => _minimumLength = value;
        }
        
        private int? _maximumLength;
        int? IJsonSchema.MaximumLength
        {
            get => _maximumLength;
            set => _maximumLength = value;
        }

        /// <summary>
        /// Maximum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        public int MaximumLength
        {
            get => _maximumLength ?? 0;
            set => _maximumLength = value;
        }
        
        /// <inheritdoc/>
        public string? Pattern { get; set; }
        
        private int? _minimumItems;
        int? IJsonSchema.MinimumItems
        {
            get => _minimumItems;
            set => _minimumItems = value;
        }
        
        /// <summary>
        /// Minimum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        public int MinimumItems 
        {
            get => _minimumItems ?? 0;
            set => _minimumItems = value;
        }
        
        private int? _maximumItems;
        int? IJsonSchema.MaximumItems
        {
            get => _maximumItems;
            set => _maximumItems = value;
        }
        
        /// <summary>
        /// Maximum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        public int MaximumItems 
        {
            get => _maximumItems ?? 0;
            set => _maximumItems = value;
        }
        
        private bool? _uniqueItems;
        bool? IJsonSchema.UniqueItems
        {
            get => _uniqueItems;
            set => _uniqueItems = value;
        }
        
        /// <summary>
        /// If array accepts only unique items.
        /// This property should be use only for collection. 
        /// </summary>
        public bool UniqueItems 
        {
            get => _uniqueItems ?? false;
            set => _uniqueItems = value;
        }
    }
}
