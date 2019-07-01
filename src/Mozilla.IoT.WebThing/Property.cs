using System;
using System.Collections.Generic;
using System.Linq;
using Mozilla.IoT.WebThing.Exceptions;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// A Property represents an individual state value of a thing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Property<T> : Property
    {
        public new event EventHandler<ValueChangedEventArgs<T>> ValuedChanged;

        public new T Value
        {
            get => (T)base.Value;
            set => base.Value = value;
        }

        public Property(Thing thing, string name, T value)
            : base(thing, name, value)
        {
        }


        public Property(Thing thing, string name, T value, IDictionary<string, object> metadata)
            : base(thing, name, value, metadata)
        {
        }

        protected override void OnValueChanged()
        {
            ValuedChanged?.Invoke(this, new ValueChangedEventArgs<T>(Value));
        }
    }

    public class Property
    {
        private const string REL = "rel";
        private const string PROPERTY = "property";
        private const string HREF = "href";
        private const string LINKS = "links";
        private const string DEFAULT_PREFIX = "/";

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

        private string _hrefPreix;

        /// <summary>
        /// The prefix of any hrefs associated with this property.
        /// </summary>
        public string HrefPrefix
        {
            get => string.IsNullOrEmpty(_hrefPreix) ? DEFAULT_PREFIX : _hrefPreix;
            set => _hrefPreix = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, object> Metadata { get; }

        protected IJsonSchema Schema { get; }

        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                ValidateValue(value);
                _value = value;

                OnValueChanged();
            }
        }


        public event EventHandler<ValueChangedEventArgs> ValuedChanged;

        public Property(Thing thing, string name, object value)
            : this(thing, name, value, null)
        {
        }

        public Property(Thing thing, string name, object value, IDictionary<string, object> metadata)
        {
            Thing = thing;
            Name = name;
            HrefPrefix = string.Empty;
            Href = $"properties/{name}";
            Metadata = metadata ?? new Dictionary<string, object>();
            _value = value;
            //Schema = JSchema.Load(Metadata.CreateReader());
        }

        /// <summary>
        /// Get the property description.
        /// </summary>
        /// <returns>Description of the property as an object</returns>
        public virtual IDictionary<string, object> AsPropertyDescription()
        {
            var description = Metadata.ToDictionary(
                entry => entry.Key,
                entry => entry.Value);

            var link = new Dictionary<string, object>
            {
                [REL] = PROPERTY,
                [HREF] = HrefPrefix.JoinUrl(Href)
            };

            if (description.TryGetValue(LINKS, out var token))
            {
                if (token is ICollection<object>  array)
                {
                    array.Add(link);
                }
            }
            else
            {
                ICollection<object> links = new LinkedList<object>();
                links.Add(link);
                description.Add(LINKS, link);
            }

            return description;
        }

        protected virtual void ValidateValue(object value)
        {
            if (Schema.IsReadOnly)
            {
                throw new PropertyException($"readonly property {Name}");
            }

            if (!Schema.IsValid(value))
            {
                throw new PropertyException("Invalid property value");
            }
        }

        protected virtual void OnValueChanged()
        {
            ValuedChanged?.Invoke(this, new ValueChangedEventArgs(Value));
        }
    }

    public class ValueChangedEventArgs : EventArgs
    {
        public object Value { get; }

        public ValueChangedEventArgs(object value)
            => Value = value;
    }

    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Value { get; }

        public ValueChangedEventArgs(T value)
            => Value = value;
    }
}
