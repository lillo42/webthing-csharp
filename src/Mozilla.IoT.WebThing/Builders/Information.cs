using System.Linq;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <summary>
    /// Represent property/parameter validation
    /// </summary>
    public readonly struct Information
    {
        private readonly bool _isNullable;

        /// <summary>
        /// Initialize a new instance of <see cref="Information"/>.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <param name="exclusiveMinimum">The exclusive minimum value.</param>
        /// <param name="exclusiveMaximum">The exclusive maximum value.</param>
        /// <param name="multipleOf">The multiple of value.</param>
        /// <param name="minimumLength">The minimum length value.</param>
        /// <param name="maximumLength">The maximum length value.</param>
        /// <param name="pattern">The pattern value.</param>
        /// <param name="enums">The enums values.</param>
        /// <param name="isReadOnly">Is is read-only</param>
        /// <param name="name">The name</param>
        /// <param name="isNullable"></param>
        public Information(double? minimum, double? maximum,
            double? exclusiveMinimum, double? exclusiveMaximum, double? multipleOf,
            int? minimumLength, int? maximumLength, string? pattern, object[]? enums,
            bool isReadOnly, string name, bool isNullable)
        {
            Minimum = minimum;
            Maximum = maximum;
            ExclusiveMinimum = exclusiveMinimum;
            ExclusiveMaximum = exclusiveMaximum;
            MultipleOf = multipleOf;
            MinimumLength = minimumLength;
            MaximumLength = maximumLength;
            Pattern = pattern;
            Enums = enums;
            IsReadOnly = isReadOnly;
            Name = name;
            _isNullable = isNullable;
        }

        
        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Minimum value.
        /// </summary>
        public double? Minimum { get; }
        
        /// <summary>
        /// Maximum value.
        /// </summary>
        public double? Maximum { get; }
        
        /// <summary>
        /// Exclusive minimum value.
        /// </summary>
        public double? ExclusiveMinimum { get; }
        
        /// <summary>
        /// Exclusive maximum value.
        /// </summary>
        public double? ExclusiveMaximum { get; }
        
        /// <summary>
        /// Multiple of value.
        /// </summary>
        public double? MultipleOf { get; }
        
        /// <summary>
        /// Minimum length value.
        /// </summary>
        public int? MinimumLength { get; }
        
        /// <summary>
        /// Maximum length value.
        /// </summary>
        public int? MaximumLength { get; }
        
        /// <summary>
        /// String pattern value.
        /// </summary>
        public string? Pattern { get; }
        
        /// <summary>
        /// Possible values.
        /// </summary>
        public object[]? Enums { get; }
        
        /// <summary>
        /// If is Read-only
        /// </summary>
        public bool IsReadOnly { get; }
        

        /// <summary>
        /// IsNullable.
        /// </summary>
        public bool IsNullable
            => _isNullable || (Enums != null && Enums.Contains(null!));
    }
}
