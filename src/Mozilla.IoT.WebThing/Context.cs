using System;
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing
{
    public class Context
    {
        public Context(IThingConverter converter, 
            IPropertiesOld propertiesOld, 
            Dictionary<string, EventCollection> events,
            Dictionary<string, ActionContext> actions)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            PropertiesOld = propertiesOld ?? throw new ArgumentNullException(nameof(propertiesOld));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public IThingConverter Converter { get; }
        
        public IPropertiesOld PropertiesOld { get; }
        
        public Dictionary<string, EventCollection> Events { get; }
        public Dictionary<string, ActionContext> Actions { get; } 
    }
}
