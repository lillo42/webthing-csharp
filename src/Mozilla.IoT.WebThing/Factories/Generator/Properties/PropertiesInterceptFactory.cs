using System;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    internal class PropertiesInterceptFactory : IInterceptorFactory
    {
        private readonly Thing _thing;
        private readonly PropertiesIntercept _intercept;

        public PropertiesInterceptFactory(Thing thing)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _intercept = new PropertiesIntercept();
        }

        public IThingIntercept CreateThingIntercept() => new EmptyIntercept();

        public IPropertyIntercept CreatePropertyIntercept()
            => _intercept;

        public IActionIntercept CreatActionIntercept()
            => new EmptyIntercept();

        public IEventIntercept CreatEventIntercept()
            => new EmptyIntercept();

        public IProperties Create() 
            => new WebThing.Properties(_thing, _intercept.Properties);
    }
}
