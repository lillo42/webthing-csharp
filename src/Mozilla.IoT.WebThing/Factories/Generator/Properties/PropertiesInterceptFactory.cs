using System.Collections.Generic;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Properties;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    /// <inheritdoc/> 
    public class PropertiesInterceptFactory : IInterceptorFactory
    {
        private readonly EmptyIntercept _empty = new EmptyIntercept();
        private readonly PropertiesIntercept _intercept;

        /// <summary>
        /// The <see cref="IProperty"/> created, map by action name.
        /// </summary>
        public Dictionary<string, IProperty> Properties => _intercept.Properties;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertiesInterceptFactory"/>.
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/>.</param>
        public PropertiesInterceptFactory(ThingOption option)
        {
            _intercept = new PropertiesIntercept(option);
        }

        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IThingIntercept CreateThingIntercept() 
            => _empty;

        /// <inheritdoc/> 
        public IPropertyIntercept CreatePropertyIntercept() 
            => _intercept;

        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IActionIntercept CreatActionIntercept()
            => _empty;

        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IEventIntercept CreatEventIntercept()
            => _empty;
    }
}
