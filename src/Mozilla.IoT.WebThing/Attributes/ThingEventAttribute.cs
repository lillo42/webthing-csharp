using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    /// <summary>
    /// Event information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public class ThingEventAttribute : Attribute
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
    }
}
