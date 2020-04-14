namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// The json schema
    /// </summary>
    public interface IJsonSchema
    {
        /// <summary>
        /// If property is read-only.
        /// </summary>
        bool? IsReadOnly { get; set; }
        
        /// <summary>
        /// If property is write-only.
        /// </summary>
        bool? IsWriteOnly { get; set; }
        
        /// <summary>
        /// Possible value this property should have.
        /// </summary>
        object[]? Enum { get; set; }
        
        /// <summary>
        /// Minimum accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? Minimum { get; set; }
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? Maximum { get; set; }
        
        /// <summary>
        /// Exclusive minimum (less than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? ExclusiveMinimum { get; set; }
        
        /// <summary>
        /// Exclusive maximum (great than and not equal) accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? ExclusiveMaximum { get; set; }
        
        /// <summary>
        /// Multiple of accepts value.
        /// This property should be use only for number(int, long, double, byte and etc).
        /// </summary>
        decimal? MultipleOf { get; set; }
        
        /// <summary>
        /// Minimum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        int? MinimumLength { get; set; }
        
        /// <summary>
        /// Maximum string length accepts.
        /// This property should be use only for string.
        /// </summary>
        int? MaximumLength { get; set; }
        
        /// <summary>
        /// Pattern this action parameter must have.
        /// This property should be use only for string.
        /// </summary>
        string? Pattern { get; set; }
        
        /// <summary>
        /// Minimum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        int? MinimumItems { get; set; }
        
        /// <summary>
        /// Maximum array length accepts.
        /// This property should be use only for collection.
        /// </summary>
        int? MaximumItems { get; set; }
        
        /// <summary>
        /// If array accepts only unique items.
        /// This property should be use only for collection. 
        /// </summary>
        bool? UniqueItems { get; set; }
    }
}
