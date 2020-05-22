using System.Linq;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <summary>
    /// Represent property/parameter validation
    /// </summary>
    public readonly struct JsonSchema
    {
        private readonly bool _isNullable;

        /// <summary>
        /// Initialize a new instance of <see cref="JsonSchema"/>.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="jsonType"></param>
        /// <param name="name">The name</param>
        /// <param name="isNullable"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="enums"></param>
        /// <param name="isWriteOnly"></param>
        public JsonSchema(IJsonSchema? schema, object[]? enums, JsonType jsonType, string name, bool isNullable, bool? isReadOnly, bool? isWriteOnly)
            : this(schema, enums, jsonType, name, isNullable)
        {
            IsReadOnly = isReadOnly;
            IsWriteOnly = isWriteOnly;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="JsonSchema"/>.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="enums"></param>
        /// <param name="jsonType"></param>
        /// <param name="name">The name</param>
        /// <param name="isNullable"></param>
        public JsonSchema(IJsonSchema? schema, object[]? enums, JsonType jsonType, string name, bool isNullable)
        {
            Title = schema?.Title;
            Type = schema?.Type;
            Unit = schema?.Unit;
            Description = schema?.Description;
            
            Name = name;
            Enums = enums;
            IsReadOnly = schema?.IsReadOnly;
            IsWriteOnly = schema?.IsWriteOnly;
            Deprecated = null;
            
            Minimum = schema?.Minimum;
            Maximum = schema?.Maximum;
            ExclusiveMinimum = schema?.ExclusiveMinimum;
            ExclusiveMaximum = schema?.ExclusiveMaximum;
            MultipleOf = schema?.MultipleOf;
            
            MinimumLength = schema?.MinimumLength;
            MaximumLength = schema?.MaximumLength;
            Pattern = schema?.Pattern;
            
            MinimumItems = schema?.MinimumItems;
            MaximumItems = schema?.MaximumItems;
            UniqueItems = schema?.UniqueItems;
            
            JsonType = jsonType;
            _isNullable = isNullable;
        }

        
        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Action parameter title.
        /// </summary>
        public string? Title { get; }
        
        /// <summary>
        /// Action parameter description.
        /// </summary>
        public string? Description { get; }
        
        /// <summary>
        /// Property types.
        /// </summary>
        public string[]? Type { get; }
        
        /// <summary>
        /// Unit of Action parameter.
        /// </summary>
        public string? Unit { get; }
        
        /// <summary>
        /// Minimum value.
        /// </summary>
        public decimal? Minimum { get; }
        
        /// <summary>
        /// Maximum value.
        /// </summary>
        public decimal? Maximum { get; }
        
        /// <summary>
        /// Exclusive minimum value.
        /// </summary>
        public decimal? ExclusiveMinimum { get; }
        
        /// <summary>
        /// Exclusive maximum value.
        /// </summary>
        public decimal? ExclusiveMaximum { get; }
        
        /// <summary>
        /// Multiple of value.
        /// </summary>
        public decimal? MultipleOf { get; }
        
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
        public bool? IsReadOnly { get; }
        
        /// <summary>
        /// If is Write-only
        /// </summary>
        public bool? IsWriteOnly { get; }

        /// <summary>
        /// Minimum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        public int? MinimumItems { get; } 
        
        /// <summary>
        /// Maximum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        public int? MaximumItems { get; }
        
        /// <summary>
        /// If array accepts only unique items.
        /// This property should be use only for collection. 
        /// </summary>
        public bool? UniqueItems { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public JsonType JsonType { get; }


        /// <summary>
        /// Is a deprecated value
        /// </summary>
        public bool? Deprecated { get; }
        
        /// <summary>
        /// IsNullable.
        /// </summary>
        public bool IsNullable
            => _isNullable || (Enums != null && Enums.Contains(null!));
    }
}
