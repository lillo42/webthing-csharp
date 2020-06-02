namespace Mozilla.IoT.WebThing.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class WriteOnlyPropertyException : ThingException
    {
        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public WriteOnlyPropertyException(string propertyName)
            : base($"Trying to get {propertyName}.")
        {
            PropertyName = propertyName;
        }
        
    }
}
