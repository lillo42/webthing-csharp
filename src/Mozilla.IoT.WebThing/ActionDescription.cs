using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public class ActionDescription : IEquatable<ActionDescription>
    {
        #region Properties
        /// <summary>
        /// Name of Action
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A human friendly nam
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// A human friendly description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The metadata of event
        /// </summary>
        public Input Input { get; set; }
        #endregion

        #region Constructor

        public ActionDescription(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ActionDescription(string name, string? title)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title;
        }

        public ActionDescription(string name, string? title, string? description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title;
            Description = description;
        }

        public ActionDescription(string name, string? title, string? description, Input input)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title;
            Description = description;
            Input = input;
        }

        #endregion

        #region IEquatable

        public bool Equals(ActionDescription? other)
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
                   && Input.Equals(other.Input);
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

            return Equals((ActionDescription) obj);
        }

        public override int GetHashCode() 
            => HashCode.Combine(Name, Title, Description, Input);
        
        #endregion
    }

    public class Input : IEquatable<Input>
    {
        #region Properties
        /// <summary>
        /// Identifying a type from the linked <see cref="Thing.Context"/>
        /// </summary>
        public string[]? Type { get; set; }
        
        /// <summary>
        /// The metadata of event
        /// </summary>
        public IDictionary<string, object>  Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Properties
        /// </summary>
        public ICollection<PropertyDescription> Properties { get; set; } = new LinkedList<PropertyDescription>();
        
        #endregion

        #region Constructor

        public Input()
        {
            
        }

        public Input(string[]? type)
        {
            Type = type;
        }
        
        public Input(string[]? type, IDictionary<string, object>  metadata)
        {
            Type = type;
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }
        
        public Input(string[]? type, IDictionary<string, object>  metadata, ICollection<PropertyDescription> properties)
        {
            Type = type;
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        #endregion

        #region IEquatable

        public bool Equals(Input? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Type, other.Type) 
                   && Metadata.Equals(other.Metadata) 
                   && Properties.Equals(other.Properties);
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

            return Equals((Input) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Type, Metadata, Properties);

        #endregion
    }
}
