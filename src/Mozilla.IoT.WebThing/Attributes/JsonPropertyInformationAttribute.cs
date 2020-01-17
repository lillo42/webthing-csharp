using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonPropertyInformationAttribute : Attribute
    {
        public string? Title { get; }
        public string? Description { get; }
        public string[]? Type { get; }
        public bool IsReadOnly { get; }
        public object[] Enum { get; }
        public decimal? Minimum { get; }
        public decimal? Maximum { get; }
        public int? MultipleOf { get; }

        public JsonPropertyInformationAttribute(string? title = null, string? description = null, string[]? type = null, bool isReadOnly = false, object[] @enum = null, decimal? minimum = null, decimal? maximum = null, int? multipleOf = null)
        {
            Title = title;
            Description = description;
            Type = type;
            IsReadOnly = isReadOnly;
            Enum = @enum;
            Minimum = minimum;
            Maximum = maximum;
            MultipleOf = multipleOf;
        }
    }
}
