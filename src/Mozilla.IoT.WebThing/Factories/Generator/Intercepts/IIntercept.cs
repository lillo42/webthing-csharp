namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    
    /// <summary>
    /// Basic intercept.
    /// </summary>
    public interface IIntercept
    {
        /// <summary>
        /// Before start visit.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        void Before(Thing thing);
        
        /// <summary>
        /// After finish visit
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        void After(Thing thing);
    }
}
