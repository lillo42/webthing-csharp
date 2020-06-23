using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Endpoints;
using Mozilla.IoT.WebThing.Middlewares;
using Mozilla.IoT.WebThing.WebSockets;

namespace Microsoft.AspNetCore.Routing
{
    /// <summary>
    /// 
    /// </summary>
    public static class IEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Map Things endpoints.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void MapThings(this IEndpointRouteBuilder endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            
            endpoint.MapGet("/things", GetAllThings.InvokeAsync);
            endpoint.MapGet("/things/{name}",  context => context.WebSockets.IsWebSocketRequest 
                ? WebSocket.InvokeAsync(context) : GetThing.InvokeAsync(context));
            endpoint.MapGet("/things/{name}/properties",  GetProperties.InvokeAsync);
            endpoint.MapGet("/things/{name}/properties/{property}",  GetProperty.InvokeAsync);
            endpoint.MapPut("/things/{name}/properties/{property}",  PutProperty.InvokeAsync);
            endpoint.MapGet("/things/{name}/events",  GetEvents.InvokeAsync);
            endpoint.MapGet("/things/{name}/events/{event}",  GetEvent.InvokeAsync);
            endpoint.MapPost("/things/{name}/actions",  PostActions.InvokeAsync);
            endpoint.MapGet("/things/{name}/actions",  GetActions.InvokeAsync);
            endpoint.MapPost("/things/{name}/actions/{action}",  PostAction.InvokeAsync);
            endpoint.MapGet("/things/{name}/actions/{action}",  GetAction.InvokeAsync);
            endpoint.MapGet("/things/{name}/actions/{action}/{id}",  GetActionById.InvokeAsync);
            endpoint.MapDelete("/things/{name}/actions/{action}/{id}",  DeleteAction.InvokeAsync);

            //To Force bind
            endpoint.ServiceProvider.GetService<IEnumerable<Thing>>();
        }
    }
}
