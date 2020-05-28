using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
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
        
        /// <summary>
        /// Context of Property, Event and Action of thing
        /// </summary>
        [ThingProperty(Ignore = true)]
        [JsonIgnore]
        public ThingContext ThingContext { get; set; } = default!;
        
        /// <summary>
        /// URI for a schema repository which defines standard schemas for common "types" of device capabilities.
        /// </summary>
        [ThingProperty(Ignore = true)]
        public virtual string Context { get; } = DefaultContext;

        /// <summary>
        /// The id of the thing.
        /// Default value is same as name
        /// </summary>
        [ThingProperty(Ignore = true)]
        public virtual string Id => Name;
        
        /// <summary>
        /// The name of the thing.
        /// </summary>
        [ThingProperty(Ignore = true)]
        public abstract string Name { get; }

        /// <summary>
        /// The title member is a human friendly string which describes the device.
        /// This can be set to any value by the device creator and may include a brand name or model number.
        /// </summary>
        [ThingProperty(Ignore = true)]
        public virtual string? Title { get; } = null;

        /// <summary>
        /// The description member is a human friendly string which describes the device and its functions.
        /// This can be set to any value by the device creator.
        /// </summary>
        [ThingProperty(Ignore = true)]
        public virtual string? Description { get; } = null;

        /// <summary>
        /// The names of schemas for types of capabilities a device supports.
        /// </summary>
        [ThingProperty(Ignore = true)]
        public virtual string[]? Type { get; } = null;

        /// <summary>
        /// Security define. The default value is nosec_sc
        /// </summary>
        [ThingProperty(Ignore = true)]
        public virtual string Security => "nosec_sc";
        
        
        /// <summary>
        /// The security definition. The default value is.
        /// {
        ///     "nosec_sc":
        ///     {
        ///         "scheme": "nosec"
        ///     }
        /// }
        /// </summary>
        [ThingProperty(Ignore = true)]
        public virtual Dictionary<string, object> SecurityDefinitions { get; } = new Dictionary<string, object>
        {
            ["nosec_sc"] = new Dictionary<string, object>
            {
                ["scheme"] = "nosec"
            }
        };
        #endregion

        /// <summary>
        /// Determine <see cref="Thing"/> the specified object is equal to current object.
        /// </summary>
        /// <param name="other">The <see cref="Thing"/> to comparer with current object.</param>
        /// <returns>A <see cref="bool"/> indicating if the passed in object obj is Equal to this.</returns>
        public bool Equals([AllowNull]Thing other)
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

        /// <summary>
        /// Determine whatever the specified object is equal to current object.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to comparer with current object.</param>
        /// <returns>A <see cref="bool"/> indicating if the passed in object obj is Equal to this.</returns>
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

        /// <summary>
        /// Get Hashcode.
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode() 
            => HashCode.Combine(Context, Title, Description);

        /// <summary>
        /// When Property Change.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invoke event <see cref="PropertyChanged"/>
        /// </summary>
        /// <param name="propertyName">Name of Property that has changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName]string? propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
