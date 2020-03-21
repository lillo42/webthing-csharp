namespace Mozilla.IoT.WebThing.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IThingCollectionBuilder
    {
        /// <summary>
        /// Add thing.
        /// </summary>
        /// <typeparam name="T">The <see cref="Thing"/></typeparam>
        /// <returns>Current <see cref="IThingCollectionBuilder"/>.</returns>
        IThingCollectionBuilder AddThing<T>()
            where T : Thing;
        
        
        /// <summary>
        /// Add thing instance.
        /// </summary>
        /// <param name="thing">The instance of <see cref="Thing"/>.</param>
        /// <typeparam name="T">The <see cref="Thing"/></typeparam>
        /// <returns>Current <see cref="IThingCollectionBuilder"/>.</returns>
        IThingCollectionBuilder AddThing<T>(T thing)
            where T : Thing;
        
    }
}
