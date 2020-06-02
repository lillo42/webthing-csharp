namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// Represent json type
    /// </summary>
    public enum JsonType
    {
        /// <summary>
        /// The <see cref="bool"/>
        /// </summary>
        Boolean,
        
        /// <summary>
        /// The <see cref="string"/>
        /// </summary>
        String,
        
        /// <summary>
        /// The <see cref="int"/>
        /// </summary>
        Integer,
        
        /// <summary>
        /// The <see cref="decimal"/>
        /// </summary>
        Number,
        
        /// <summary>
        /// The array
        /// </summary>
        Array,
        
        /// <summary>
        /// The <see cref="object"/>
        /// </summary>
        Object
    }
}
