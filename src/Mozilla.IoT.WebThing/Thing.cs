using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// The thing
    /// </summary>
    public abstract class Thing
    {
        #region Properties
        /// <summary>
        /// URI for a schema repository which defines standard schemas for common "types" of device capabilities.
        /// </summary>
        public virtual string Context { get; } = DefaultContext;

        /// <summary>
        /// The name of the thing.
        /// It's used to generated Id.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The title member is a human friendly string which describes the device.
        /// This can be set to any value by the device creator and may include a brand name or model number.
        /// </summary>
        public virtual string? Title { get; }

        /// <summary>
        /// The description member is a human friendly string which describes the device and its functions.
        /// This can be set to any value by the device creator.
        /// </summary>
        public virtual string? Description { get; }

        /// <summary>
        /// The names of schemas for types of capabilities a device supports.
        /// </summary>
        public virtual string[]? Type { get; }
        #endregion
    }
}
