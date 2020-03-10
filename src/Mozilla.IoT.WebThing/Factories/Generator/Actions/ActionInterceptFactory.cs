using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Actions
{
    public class ActionInterceptFactory : IInterceptorFactory
    {
        private readonly ActionIntercept _intercept;
        private readonly EmptyIntercept _empty;

        public Dictionary<string, ActionCollection> Actions => _intercept.Actions;
        public ActionInterceptFactory(ThingOption option)
        {
            _empty = new EmptyIntercept();
            var assemblyName = new AssemblyName("ActionAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("ActionModule");
            _intercept = new ActionIntercept(moduleBuilder, option);
        }
        
        public IThingIntercept CreateThingIntercept()
            => _empty;

        public IPropertyIntercept CreatePropertyIntercept()
            => _empty;

        public IActionIntercept CreatActionIntercept()
            => _intercept;

        public IEventIntercept CreatEventIntercept()
            => _empty;
    }
}
