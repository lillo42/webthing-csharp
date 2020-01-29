using System;
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing
{
    public class Context
    {
        public Context(IThingConverter converter, 
            IProperties properties, 
            Dictionary<string, ThingEventCollection> events,
            Dictionary<string, ActionContext> actions)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public IThingConverter Converter { get; }
        
        public IProperties Properties { get; }
        
        public Dictionary<string, ThingEventCollection> Events { get; }
        public Dictionary<string, ActionContext> Actions { get; } 
    }
}
