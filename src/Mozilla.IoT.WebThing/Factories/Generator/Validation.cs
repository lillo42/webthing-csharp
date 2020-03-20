using System.Linq;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    /// <summary>
    /// Represent property/parameter validation
    /// </summary>
    public readonly struct Validation
    {
        public Validation(double? minimum, double? maximum,
            double? exclusiveMinimum, double? exclusiveMaximum, double? multipleOf,
            int? minimumLength, int? maximumLength, string? pattern, object[]? enums)
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
        }

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
        /// If has validation or all value are null.
        /// </summary>
        public bool HasValidation
            => Minimum.HasValue
               || Maximum.HasValue
               || ExclusiveMinimum.HasValue
               || ExclusiveMaximum.HasValue
               || MultipleOf.HasValue
               || MinimumLength.HasValue
               || MaximumLength.HasValue
               || Pattern != null
               || (Enums != null && Enums.Length > 0);

        /// <summary>
        /// If Enum has null value.
        /// </summary>
        public bool HasNullValueOnEnum
            => Enums != null && Enums.Contains(null);
    }
}
