using System.Collections.Generic;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Properties;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    public class PropertiesInterceptFactory : IInterceptorFactory
    {
        private readonly EmptyIntercept _empty = new EmptyIntercept();
        private readonly PropertiesIntercept _intercept;

        public Dictionary<string, IProperty> Properties => _intercept.Properties;

        public PropertiesInterceptFactory(ThingOption option)
        {
            _intercept = new PropertiesIntercept(option);
        }

        public IThingIntercept CreateThingIntercept() 
            => _empty;

        public IPropertyIntercept CreatePropertyIntercept() 
            => _intercept;

        public IActionIntercept CreatActionIntercept()
            => _empty;

        public IEventIntercept CreatEventIntercept()
            => _empty;
    }
}
