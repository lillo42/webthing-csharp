using System;
using Mozilla.IoT.WebThing.Mapper;

namespace Mozilla.IoT.WebThing
{
    internal sealed class Property
    {
        public Property(Func<object, object> getter,
            Action<object, object> setter,
            IPropertyValidator validator,
            IJsonMapper mapper)
        {
            Getter = getter ?? throw new ArgumentNullException(nameof(getter));
            Setter = setter ?? throw new ArgumentNullException(nameof(setter));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Action<object, object> Setter { get; }
        public Func<object, object> Getter { get; }
        public IPropertyValidator Validator { get; }
        public IJsonMapper Mapper { get; }
    }
}
