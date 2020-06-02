namespace Mozilla.IoT.WebThing.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadOnlyPropertyException : ThingException
    {
        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public ReadOnlyPropertyException(string propertyName)
            : base($"Trying to set {propertyName}.")
        {
            PropertyName = propertyName;
        }
        
    }
}
