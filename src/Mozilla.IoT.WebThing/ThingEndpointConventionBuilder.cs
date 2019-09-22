using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace Mozilla.IoT.WebThing
{
    public class ThingEndpointConventionBuilder : IEndpointConventionBuilder
    {
        private readonly IEnumerable<IEndpointConventionBuilder> _endpointConventionBuilders;

        public ThingEndpointConventionBuilder(IEnumerable<IEndpointConventionBuilder> endpointConventionBuilders)
        {
            _endpointConventionBuilders = endpointConventionBuilders;
        }

        public void Add(Action<EndpointBuilder> convention)
        {
            foreach (var endpointConventionBuilder in _endpointConventionBuilders)
            {
                endpointConventionBuilder.Add(convention);
            }
        }
    }
}
