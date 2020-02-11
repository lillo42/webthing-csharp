namespace Mozilla.IoT.WebThing.Exceptions
{
    public class PropertyNotFoundException : ThingException
    {
        public PropertyNotFoundException(string propertyName) 
            : base($"Property not found {propertyName}")
        {
        }
    }
}
