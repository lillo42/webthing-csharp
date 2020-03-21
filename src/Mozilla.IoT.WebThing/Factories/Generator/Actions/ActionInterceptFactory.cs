using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Actions
{
    /// <summary>
    /// Create Action
    /// </summary>
    public class ActionInterceptFactory : IInterceptorFactory
    {
        private readonly ActionIntercept _intercept;
        private readonly EmptyIntercept _empty;

        /// <summary>
        /// The <see cref="ActionCollection"/> Create, map by action name.
        /// </summary>
        public Dictionary<string, ActionCollection> Actions => _intercept.Actions;
        
        /// <summary>
        /// Initialize a new instance of <see cref="ActionInterceptFactory"/>.
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/>.</param>
        public ActionInterceptFactory(ThingOption option)
        {
            _empty = new EmptyIntercept();
            var assemblyName = new AssemblyName("ActionAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("ActionModule");
            _intercept = new ActionIntercept(moduleBuilder, option);
        }
        
        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IThingIntercept CreateThingIntercept()
            => _empty;

        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IPropertyIntercept CreatePropertyIntercept()
            => _empty;

        /// <inheritdoc /> 
        public IActionIntercept CreatActionIntercept()
            => _intercept;

        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IEventIntercept CreatEventIntercept()
            => _empty;
    }
}
