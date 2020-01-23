using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Mozilla.IoT.WebThing.Factories.Generator.Converter;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Factories.Generator.Properties;
using Mozilla.IoT.WebThing.Factories.Generator.Visitor;

namespace Mozilla.IoT.WebThing.Factories
{
    public class ThingFactory
    {
        public void Generate(Thing thing, JsonSerializerOptions options)
        {
            var factories = new IInterceptorFactory[]
            {
                new ConverterInterceptorFactory(thing, options), 
                new PropertiesInterceptFactory(thing, options)
            };

            var thingVisitor = factories
                .Select(x => x.CreateThingIntercept())
                .ToArray();
            
            foreach (var intercept in thingVisitor)
            {
                intercept.Before(thing);
            }
            
            VisitProperties(thing, factories);
            VisitActions(thing, factories);
            VisitEvents(thing, factories);
            
            foreach (var intercept in thingVisitor)
            {
                intercept.After(thing);
            }
        }


        private static void VisitActions(Thing thing, IEnumerable<IInterceptorFactory> factories)
        {
            var intercepts = factories
                .Select(x => x.CreatActionIntercept())
                .ToArray();

            ActionVisitor.Visit(intercepts, thing);
        }
        
        private static void VisitProperties(Thing thing, IEnumerable<IInterceptorFactory> factories)
        {
            var intercepts = factories
                .Select(x => x.CreatePropertyIntercept())
                .ToArray();

            PropertiesVisitor.Visit(intercepts, thing);
        }
        
        private static void VisitEvents(Thing thing, IEnumerable<IInterceptorFactory> factories)
        {
            var intercepts = factories
                .Select(x => x.CreatEventIntercept())
                .ToArray();

            EventVisitor.Visit(intercepts, thing);
        }
    }
}
