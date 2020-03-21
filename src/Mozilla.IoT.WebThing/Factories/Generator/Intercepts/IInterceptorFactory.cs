namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    /// <summary>
    /// The <see cref="IIntercept"/> factory.
    /// </summary>
    public interface IInterceptorFactory
    {
        /// <summary>
        /// Create new instance of <see cref="IThingIntercept"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IThingIntercept"/>.</returns>
        IThingIntercept CreateThingIntercept();
        
        /// <summary>
        /// Create new instance of <see cref="IPropertyIntercept"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IPropertyIntercept"/>.</returns>
        IPropertyIntercept CreatePropertyIntercept();
        
        /// <summary>
        /// Create new instance of <see cref="IActionIntercept"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IActionIntercept"/>.</returns>
        IActionIntercept CreatActionIntercept();
        
        /// <summary>
        /// Create new instance of <see cref="IEventIntercept"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IEventIntercept"/>.</returns>
        IEventIntercept CreatEventIntercept();
    }
}
