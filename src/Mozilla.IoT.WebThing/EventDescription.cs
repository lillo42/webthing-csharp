using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// <see cref="Event"/> description
    /// </summary>
    public class EventDescription : IEquatable<EventDescription>
    {
        #region Properties
        /// <summary>
        /// A human friendly nam
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A human friendly nam
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// A human friendly description
        /// </summary>
        public string? Description { get; }
        
        /// <summary>
        /// Identifying a type from the linked <see cref="Thing.Context"/>
        /// </summary>
        public string[]? Type { get; }
        
        /// <summary>
        /// The metadata of event
        /// </summary>
        public IDictionary<string, object>  Metadata { get; set; } = new Dictionary<string, object>();
        #endregion

        #region Constructor

        public EventDescription(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public EventDescription(string name, string? title)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title;
        }

        public EventDescription(string name, string? title, string? description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title;
            Description = description;
        }
        
        public EventDescription(string name, string? title, string? description, string[]? type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title;
            Description = description;
            Type = type;
        }

        #endregion
        
        #region IEquatable
        public bool Equals(EventDescription? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name 
                   && Title == other.Title 
                   && Description == other.Description 
                   && Equals(Type, other.Type) && Metadata.Equals(other.Metadata);
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

            return Equals((EventDescription) obj);
        }

        public override int GetHashCode() 
            => HashCode.Combine(Name, Title, Description, Type, Metadata);
        #endregion
    }
}
