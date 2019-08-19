using System;
using System.Collections.Generic;
using System.Linq;
using Mozilla.IoT.WebThing.Exceptions;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Descriptor
{
    public class PropertyDescriptor : IDescriptor<Property>
    {
        public IDictionary<string, object> CreateDescription(Property value)
        {
            var result = value.Metadata?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, object>();

            var link = new Dictionary<string, object>
            {
                [REL] = RelType.Property.ToString().ToLower(),
                [HREF] = value.HrefPrefix.JoinUrl(value.Href)
            };

            ICollection<IDictionary<string, object>> links;

            if (result.TryGetValue(LINKS, out object data))
            {
                if (data is ICollection<IDictionary<string, object>> array)
                {
                    links = array;
                }
                else
                {
                    throw new DescriptionException("Invalid type on property links");
                }
            }
            else
            {
                result.Add(LINKS, links = new LinkedList<IDictionary<string, object>>());
            }

            links.Add(link);


            return result;
        }
        
    }
}
