using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mozilla.IoT.WebThing.Attributes;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// The thing
    /// </summary>
    public abstract class Thing : INotifyPropertyChanged, IEquatable<Thing>
    {
        #region Properties

        internal Uri Prefix { get; set; } = default!;

        [ThingProperty(Ignore = true)]
        public Context ThingContext { get; set; } = default!;
        
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
        public virtual string? Title { get; } = null;

        /// <summary>
        /// The description member is a human friendly string which describes the device and its functions.
        /// This can be set to any value by the device creator.
        /// </summary>
        public virtual string? Description { get; } = null;

        /// <summary>
        /// The names of schemas for types of capabilities a device supports.
        /// </summary>
        public virtual string[]? Type { get; } = null;

        #endregion

        public bool Equals(Thing other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Context == other.Context
                   && Title == other.Title
                   && Description == other.Description;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Thing) obj);
        }

        public override int GetHashCode() 
            => HashCode.Combine(Context, Title, Description);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
