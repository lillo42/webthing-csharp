namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// The json schema
    /// </summary>
    public interface IJsonSchema
    {
        /// <summary>
        /// Action parameter name.
        /// </summary>
        string? Name { get; }
        
        /// <summary>
        /// Action parameter title.
        /// </summary>
        string? Title { get; }
        
        /// <summary>
        /// Action parameter description.
        /// </summary>
        string? Description { get; }
        
        /// <summary>
        /// If property should be ignore.
        /// </summary>
        bool Ignore { get; }
        
        /// <summary>
        /// Property types.
        /// </summary>
        string[]? Type { get; }
        
        /// <summary>
        /// Unit of Action parameter.
        /// </summary>
        public string? Unit { get; }
        
        /// <summary>
        /// If property is read-only.
        /// </summary>
        bool? IsReadOnly { get; }
        
        /// <summary>
        /// If property is write-only.
        /// </summary>
        bool? IsWriteOnly { get; }
        
        /// <summary>
        /// If property is acceptance nullable value.
        /// </summary>
        bool? IsNullable { get; }
        
        /// <summary>
        /// Possible value this property should have.
        /// </summary>
        object[]? Enum { get; }
        
        /// <summary>
        /// Minimum accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? Minimum { get; }
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? Maximum { get; }
        
        /// <summary>
        /// Exclusive minimum (less than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? ExclusiveMinimum { get; }
        
        /// <summary>
        /// Exclusive maximum (great than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? ExclusiveMaximum { get; }
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? MultipleOf { get; }
        
        /// <summary>
        /// Minimum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        int? MinimumLength { get; }
        
        /// <summary>
        /// Maximum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        int? MaximumLength { get; }
        
        /// <summary>
        /// Pattern this action parameter must have.
        /// This property should be use only for string.
        /// </summary>
        string? Pattern { get; }
        
        /// <summary>
        /// Minimum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        int? MinimumItems { get; }
        
        /// <summary>
        /// Maximum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        int? MaximumItems { get; }
        
        /// <summary>
        /// If array accepts only unique items.
        /// This property should be use only for collection. 
        /// </summary>
        bool? UniqueItems { get; }
    }
}
