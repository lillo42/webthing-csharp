using System;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Attributes
{
    /// <summary>
    /// Action information. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ThingActionAttribute : Attribute, IJsonSchema
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

        string? IJsonSchema.Unit => null;

        bool? IJsonSchema.IsReadOnly => null;

        bool? IJsonSchema.IsWriteOnly => null;

        bool? IJsonSchema.IsNullable => null;

        object[]? IJsonSchema.Enum => null;

        decimal? IJsonSchema.Minimum => null;

        decimal? IJsonSchema.Maximum => null;

        decimal? IJsonSchema.ExclusiveMinimum => null;

        decimal? IJsonSchema.ExclusiveMaximum => null;

        decimal? IJsonSchema.MultipleOf => null;

        int? IJsonSchema.MinimumLength => null;

        int? IJsonSchema.MaximumLength => null;

        string? IJsonSchema.Pattern => null;

        int? IJsonSchema.MinimumItems => null;

        int? IJsonSchema.MaximumItems => null;

        bool? IJsonSchema.UniqueItems => null;
    }
}
