using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing
{
    public class ThingContext
    {
        public ThingContext(IThingConverter converter, IProperties properties)
        {
            Converter = converter;
            Properties = properties;
        }

        public IThingConverter Converter { get; }
        
        public IProperties Properties { get; }
    }
}
