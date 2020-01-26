using System;
using Microsoft.AspNetCore.Builder;
using Mozilla.IoT.WebThing.Endpoints;

namespace Microsoft.AspNetCore.Routing
{
    public static class IEndpointRouteBuilderExtensions
    {
        public static void MapThings(this IEndpointRouteBuilder endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            endpoint.MapGet("/things", GetAllThingEndpoint.InvokeAsync);
            endpoint.MapGet("/things/{name}",  GetThingEndpoint.InvokeAsync);
            endpoint.MapGet("/things/{name}/properties",  GetThingProperties.InvokeAsync);
            endpoint.MapGet("/things/{name}/properties/{property}",  GetThingProperty.InvokeAsync);
            endpoint.MapPut("/things/{name}/properties/{property}",  PutThingProperty.InvokeAsync);
            endpoint.MapGet("/things/{name}/events",  GetThingEvents.InvokeAsync);
        }
    }
}
