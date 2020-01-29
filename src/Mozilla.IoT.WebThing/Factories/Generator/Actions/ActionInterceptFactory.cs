using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Actions
{
    public class ActionInterceptFactory : IInterceptorFactory
    {
        private readonly ActionIntercept _intercept;

        public Dictionary<string, ActionContext> Actions => _intercept.Actions;
        public ActionInterceptFactory()
        {
            var assemblyName = new AssemblyName("ActionAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("ActionModule");
            _intercept = new ActionIntercept(moduleBuilder);
        }
        
        public IThingIntercept CreateThingIntercept()
            => new EmptyIntercept();

        public IPropertyIntercept CreatePropertyIntercept()
            => new EmptyIntercept();

        public IActionIntercept CreatActionIntercept()
            => _intercept;

        public IEventIntercept CreatEventIntercept()
            => new EmptyIntercept();
    }
}
