using System;
using System.Collections.Generic;
using System.Linq;

using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Description
{
    public class ThingDescriptor : IDescriptor<Thing>
    {
        private readonly IDescriptor<Property> _propertyDescriptor;

        public ThingDescriptor(IDescriptor<Property> propertyDescriptor)
        {
            _propertyDescriptor = propertyDescriptor;
        }

        public IDictionary<string, object> CreateDescription(Thing value)
        {
            var actions = new Dictionary<string, object>();
            
            value.ActionsTypeInfo.ForEach(action =>
            {
                var metadata = action.Value.metadata.ToDictionary(x => x.Key, x => x.Value);
                metadata.Add(LINKS, new Dictionary<string, object>
                {
                    [REL] = RelType.Action.ToString().ToLower(),
                    [HREF] = value.HrefPrefix.JoinUrl($"/actions/{action.Key}")
                });
                actions.Add(action.Key, metadata);
            });
            
            var events = new Dictionary<string, object>();
            value.AvailableEvent.Values.ForEach(@event =>
            {
                var metadata = @event.Metadata.ToDictionary(x => x.Key, x => x.Value);
                metadata.Add(LINKS, new Dictionary<string, object>
                {
                    [REL] = RelType.Event.ToString().ToLower(),
                    [HREF] = value.HrefPrefix.JoinUrl($"/event/{@event.Name}") 
                });
                
                events.Add(@event.Name, metadata);
            });
            
            var properties = new Dictionary<string, object>();
            value.Properties.ForEach(property =>
            {
                properties.Add(property.Name, _propertyDescriptor.CreateDescription(property));
            });
            
            var result = new Dictionary<string, object>
            {
                ["name"] = value.Name,
                ["href"] = value.HrefPrefix,
                ["@context"] = value.Context,
                ["@type"] = value.Type,
                ["properties"] = properties,
                ["actions"] = actions,
                ["events"] = events
            };
            
            if (value.Description != null)
            {
                result.Add("description", value.Description);
            }

            var propertiesLink = new Dictionary<string, object>
            {
                [REL] = RelType.Properties.ToString().ToLower(), 
                [HREF] = value.HrefPrefix.JoinUrl("properties")
            };

            var actionsLink = new Dictionary<string, object>
            {
                [REL] = RelType.Actions.ToString().ToLower(), 
                [HREF] = value.HrefPrefix.JoinUrl("actions")
            };

            var eventsLink = new Dictionary<string, object>
            {
                [REL] = RelType.Events.ToString().ToLower(), 
                [HREF] = value.HrefPrefix.JoinUrl("events")
            };

            var links = new List<IDictionary<string, object>>
            {
                propertiesLink,
                actionsLink,
                eventsLink
            };
            
            if (value.UiHref != null)
            {
                var uiLink = new Dictionary<string, object>
                {
                    [REL] = RelType.Alternate.ToString().ToLower(), 
                    ["mediaType"] = "text/html", 
                    [HREF] = value.UiHref
                };

                links.Add(uiLink);
            }

            result.Add(LINKS, links);

            return result;
        }
    }
}
