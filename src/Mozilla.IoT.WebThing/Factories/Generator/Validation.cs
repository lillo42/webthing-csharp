using System.Linq;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
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

        public double? Minimum { get; }
        public double? Maximum { get; }
        public double? ExclusiveMinimum { get; }
        public double? ExclusiveMaximum { get; }
        public double? MultipleOf { get; }
        public int? MinimumLength { get; }
        public int? MaximumLength { get; }
        public string? Pattern { get; }
        public object[]? Enums { get; }

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

        public bool HasNullValueOnEnum
            => Enums != null && Enums.Contains(null);
    }
}
