using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <summary>
    /// The <see cref="ThingContext"/> factory
    /// </summary>
    public interface IThingContextFactory
    {
        /// <summary>
        /// Create new instance of <see cref="ThingContext"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="option">The <see cref="ThingOption"/>.</param>
        /// <returns>The new instance of <see cref="ThingContext"/>.</returns>
        ThingContext Create(Thing thing, ThingOption option);
    }
}
