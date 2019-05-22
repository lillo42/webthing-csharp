using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using WebThing.Annotations;
using WebThing.Exceptions;

namespace WebThing
{
    /// <summary>
    /// A Property represents an individual state value of a thing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Property<T> : Property
    {
        public new T Value
        {
            get => (T)base.Value;
            set => base.Value = value;
        }

        public Property(Thing thing, string name, T value, JObject metadata = null)
        : base(thing, name, value, metadata)
        {
        }
    }

    public class Property : INotifyPropertyChanged
    {
        private const string REL = "rel";
        private const string PROPERTY = "property";
        private const string HREF = "href";
        private const string LINKS = "links";
        
        /// <summary>
        /// The href of this property
        /// </summary>
        public Thing Thing { get; }
        
        /// <summary>
        /// The name of this property
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The href of this property
        /// </summary>
        public string Href { get; }
        
        /// <summary>
        /// The prefix of any hrefs associated with this property.
        /// </summary>
        public string HrefPrefix { get; set; }
        
        
        /// <summary>
        /// 
        /// </summary>
        public JObject Metadata { get; }
        
        protected JSchema Schema { get; }
        
        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                ValidateValue(value);
                _value = value;
                OnPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public Property(Thing thing, string name, object value, JObject metadata = null)
        {
            Thing = thing;
            Name = name;
            HrefPrefix = string.Empty;
            Href = $"/properties/{name}";
            Metadata = metadata ?? new JObject();
            Value = value;
            Schema = JSchema.Load(Metadata.CreateReader());
        }
        
        /// <summary>
        /// Get the property description.
        /// </summary>
        /// <returns>Description of the property as an object</returns>
        public JObject AsPropertyDescription()
        {
            var description = new JObject(Metadata.DeepClone());
            var link = new JObject(
                new JProperty(REL, PROPERTY),
                new JProperty(HREF, HrefPrefix + Href));

            if (description.TryGetValue(LINKS, out JToken token))
            {
                if (token is JArray array)
                {
                    array.Add(link);
                }
                else
                {
                    throw new JsonException();
                }
            }
            else
            {
                description.Add(LINKS, new JArray(link));
            }

            return description;
        }
        
        protected void ValidateValue(object value)
        {
            if (Schema.ReadOnly.HasValue && Schema.ReadOnly.Value)
            {
                throw  new PropertyException($"readonly property {Name}");
            }

            if (Schema.IsValid(value))
            {
                throw new PropertyException("Invalid property value");
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
