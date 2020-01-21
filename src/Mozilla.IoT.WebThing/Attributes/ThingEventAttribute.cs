using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = false)]
    public class ThingEventAttribute : Attribute
    {
        public bool Ignore { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string[]? Type { get; set; }
        public string? Unit { get; set; }
    }
}
