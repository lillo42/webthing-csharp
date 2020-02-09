using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing;
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

            endpoint.MapGet("/things", GetAllThings.InvokeAsync);
            endpoint.MapGet("/things/{name}",  GetThing.InvokeAsync);
            endpoint.MapGet("/things/{name}/properties",  GetThingProperties.InvokeAsync);
            endpoint.MapGet("/things/{name}/properties/{property}",  GetThingProperty.InvokeAsync);
            endpoint.MapPut("/things/{name}/properties/{property}",  PutThingProperty.InvokeAsync);
            endpoint.MapGet("/things/{name}/events",  GetThingEvents.InvokeAsync);
            endpoint.MapPost("/things/{name}/actions",  PostThingActions.InvokeAsync);
            endpoint.MapPost("/things/{name}/actions/{action}",  PostThingAction.InvokeAsync);
            endpoint.MapGet("/things/{name}/actions/{action}",  GetThingAction.InvokeAsync);
            endpoint.MapGet("/things/{name}/actions/{action}/{id}",  GetThingActionById.InvokeAsync);

            //To Force bind
            endpoint.ServiceProvider.GetService<IEnumerable<Thing>>();
        }
    }
}
