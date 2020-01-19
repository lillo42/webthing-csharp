using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Endpoints;

namespace Microsoft.AspNetCore.Routing
{
    public static class IEndpointRouteBuilderExtensions
    {
        public static void MapThings(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapGet("/things", GetAllThingEndpoint.InvokeAsync);
            
            endpoint.MapGet("/things/{name}", context => context.WebSockets.IsWebSocketRequest ? 
                GetAllThingEndpoint.InvokeAsync(context) : GetThingEndpoint.InvokeAsync(context));
            
        }
    }
}
