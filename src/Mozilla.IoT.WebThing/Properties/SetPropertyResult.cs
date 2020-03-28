namespace Mozilla.IoT.WebThing.Properties
{
    /// <summary>
    /// Result of set property
    /// </summary>
    public enum SetPropertyResult
    {
        /// <summary>
        /// Set property is OK.
        /// </summary>
        Ok,
        
        /// <summary>
        /// Invalid value to set value. 
        /// </summary>
        InvalidValue,
        
        /// <summary>
        /// If property is read-only.
        /// </summary>
        ReadOnly
    }
}
