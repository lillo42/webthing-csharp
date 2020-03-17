using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    /// <summary>
    /// Action information. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ThingActionAttribute : Attribute
    {
        /// <summary>
        /// If action should be ignore.
        /// </summary>
        public bool Ignore { get; set; }
        
        /// <summary>
        /// Custom action name.
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Action title.
        /// </summary>
        public string? Title { get; set; }
        
        /// <summary>
        /// Action description.
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Action types
        /// </summary>
        public string[]? Type { get; set; }
    }
}
