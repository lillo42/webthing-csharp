using System.Collections.Generic;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Description
{
    public class EventDescriptor : IDescriptor<Event>
    {
        public IDictionary<string, object> CreateDescription(Event value)
        {
            var result = new Dictionary<string, object>
            {
                [TIMESTAMP] = value.Time
            };

            if (value.Data != null)
            {
                result.Add(DATA, value.Data);
            }

            return result;
        }
    }
}
