using System.Collections.Generic;
using System.Linq;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Factories.Generator.Visitor;

namespace Mozilla.IoT.WebThing.Factories
{
    internal static class CodeGeneratorFactory
    {
        public static void Generate(Thing thing, IEnumerable<IInterceptorFactory> factories)
        {
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
