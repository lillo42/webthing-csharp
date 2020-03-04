using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Actions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    public class PropertiesInterceptFactory : IInterceptorFactory
    {
        private readonly EmptyIntercept _empty = new EmptyIntercept();
        private readonly PropertiesIntercept _intercept;

        public Dictionary<string, IProperty> Properties => _intercept.Properties;

        public PropertiesInterceptFactory(ThingOption option)
        {
            var assemblyName = new AssemblyName("PropertyAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("Property");
            _intercept = new PropertiesIntercept(option, moduleBuilder);
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
