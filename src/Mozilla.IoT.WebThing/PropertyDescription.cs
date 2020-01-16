using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// <see cref="Property"/> description
    /// </summary>
    public class PropertyDescription : IEquatable<PropertyDescription>
    {
        #region Properties
        
        /// <summary>
        /// Name of Action
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// A human friendly name
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// A human friendly description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Identifying a type from the linked <see cref="Thing.Context"/>
        /// </summary>
        public string[]? Type { get; set; }

        /// <summary>
        /// The metadata of property
        /// </summary>
        public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        #endregion

        #region Constructor

        public PropertyDescription(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public PropertyDescription(string name, string? title)
        {
            Title = title;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public PropertyDescription(string name, string? title, string? description)
        {
            Title = title;
            Description = description;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public PropertyDescription(string name, string? title, string? description, string[]? type)
        {
            Title = title;
            Description = description;
            Type = type;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        #endregion
        
        #region IEquatable
        public bool Equals(PropertyDescription? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Title == other.Title 
                   && Description == other.Description 
                   && Equals(Type, other.Type) 
                   && Metadata.Equals(other.Metadata);
            
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

            return Equals((PropertyDescription) obj);
        }

        public override int GetHashCode() 
            => HashCode.Combine(Name, Title, Description, Type, Metadata);
        #endregion
    }
}
