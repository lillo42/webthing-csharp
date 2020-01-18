using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonPropertyInformationAttribute : Attribute
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string[]? Type { get; set; }
        public bool IsReadOnly { get; set; }
        public object[] Enum { get; set; }
        public float? Minimum { get; set; }
        public float? Maximum { get; set; }
        public int? MultipleOf { get; set; }
    }
}
