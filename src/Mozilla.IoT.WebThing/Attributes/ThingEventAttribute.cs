using System;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Attributes
{
    /// <summary>
    /// Event information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public class ThingEventAttribute : Attribute, IJsonSchema
    {
        /// <summary>
        /// If event should be ignore.
        /// </summary>
        public bool Ignore { get; set; }
        
        /// <summary>
        /// Custom event name.
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Event title.
        /// </summary>
        public string? Title { get; set; }
        
        /// <summary>
        /// Event description.
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Event types
        /// </summary>
        public string[]? Type { get; set; }
        
        /// <summary>
        /// Unit of event
        /// </summary>
        public string? Unit { get; set; }

        bool? IJsonSchema.IsReadOnly => null;

        bool? IJsonSchema.IsWriteOnly => null;

        /// <inheritdoc/>
        bool? IJsonSchema.IsNullable => _isNullable;

        private bool? _isNullable;


        /// <summary>
        /// If property is acceptance nullable value.
        /// </summary>
        public bool IsNullable
        {
            get => _isNullable.GetValueOrDefault();
            set => _isNullable = value;
        }

        /// <inheritdoc/>
        public object[]? Enum { get; set; }

        private decimal? _minimum;
        decimal? IJsonSchema.Minimum  => _minimum;

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
        decimal? IJsonSchema.Maximum  => _maximum;
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double Maximum 
        {
            get => Convert.ToDouble(_maximum);
            set => _maximum = Convert.ToDecimal(value);
        }

        private decimal? _exclusiveMinimum;
        decimal? IJsonSchema.ExclusiveMinimum => _exclusiveMinimum;

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
        decimal? IJsonSchema.ExclusiveMaximum => _exclusiveMaximum;
        
        /// <summary>
        /// Exclusive maximum (great than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double ExclusiveMaximum 
        {
            get => Convert.ToDouble(_exclusiveMaximum);
            set => _exclusiveMaximum = Convert.ToDecimal(value);
        }

        private decimal? _multipleOf;
        decimal? IJsonSchema.MultipleOf => _multipleOf;
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        public double MultipleOf
        {
            get => Convert.ToDouble(_multipleOf);
            set => _multipleOf = Convert.ToDecimal(value);
        }

        private int? _minimumLength;
        int? IJsonSchema.MinimumLength  => _minimumLength;

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
        int? IJsonSchema.MaximumLength => _maximumLength;

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
        int? IJsonSchema.MinimumItems => _minimumItems;

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
        int? IJsonSchema.MaximumItems => _maximumItems;
        
        
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
        bool? IJsonSchema.UniqueItems => _uniqueItems;

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
