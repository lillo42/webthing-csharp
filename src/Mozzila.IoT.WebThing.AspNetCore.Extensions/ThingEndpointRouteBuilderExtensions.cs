using System;
using Microsoft.AspNetCore.Routing;
using Mozzila.IoT.WebThing;
using WebThing.AspNetCore.Extensions;
using WebThing.AspNetCore.Extensions.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class ThingEndpointRouteBuilderExtensions
    {
        public static IApplicationBuilder UseMultiThing(this IApplicationBuilder app, string name, Action<ThingBindingOption> thingOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (thingOptions == null)
            {
                throw new ArgumentNullException(nameof(thingOptions));
            }
            var option = new ThingBindingOption();

            thingOptions(option);

            return AddRoute(app, new MultipleThings(option.Things, name), "/{thingId}");
        }

        public static IApplicationBuilder UseSingleThing(this IApplicationBuilder app, Thing thing)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing));
            }

            return AddRoute(app, new SingleThing(thing), string.Empty);
        }

        private static IApplicationBuilder AddRoute(IApplicationBuilder app, IThingType thingType, string prefix)
        {
            var router = new RouteBuilder(app);

            #region Property
            router.MapMiddlewareGet($"{prefix}/properties",
                builder => builder.UseMiddleware<GetPropertiesMiddleware>(thingType));

            router.MapMiddlewareGet($"{prefix}/properties/{{propertyName}}", 
                builder => builder.UseMiddleware<GetPropertyThingMiddleware>(thingType));
            #endregion

            #region Actions

            router.MapMiddlewareGet($"{prefix}/actions/{{actionName}}/{{actionId}}", 
                builder => builder.UseMiddleware<GetActionByIdMiddleware>(thingType));
            
            router.MapMiddlewareDelete($"{prefix}/actions/{{actionName}}/{{actionId}}", 
                builder => builder.UseMiddleware<DeleteActionByIdMiddleware>(thingType));
            
            
            router.MapMiddlewareGet($"{prefix}/actions/{{actionName}}", 
                builder => builder.UseMiddleware<GetActionMiddleware>(thingType));
            
            router.MapMiddlewarePost($"{prefix}/actions/{{actionName}}", 
                builder => builder.UseMiddleware<PostActionMiddleware>(thingType));
            
            router.MapMiddlewareGet($"{prefix}/actions", 
                builder => builder.UseMiddleware<GetActionsMiddleware>(thingType));
            
            router.MapMiddlewarePost($"{prefix}/actions", 
                builder => builder.UseMiddleware<PostActionsMiddleware>(thingType));

            #endregion

            #region Events

            router.MapMiddlewareGet($"{prefix}/events/{{eventName}}", 
                builder => builder.UseMiddleware<GetEventMiddleware>(thingType));
            
            router.MapMiddlewareGet($"{prefix}/events", 
                builder => builder.UseMiddleware<GetEventsMiddleware>(thingType));

            router.MapMiddlewareGet($"{prefix}/", 
                builder => builder.UseMiddleware<GetPropertyThingMiddleware>(thingType));
            
            #endregion
            
            #region Thing

            if (thingType is SingleThing)
            {
                router.MapMiddlewareGet("/", 
                    builder => builder.UseMiddleware<GetThingMiddleware>(thingType));
            }
            else
            {
                router.MapMiddlewareGet(prefix, 
                    builder => builder.UseMiddleware<GetThingMiddleware>(thingType));
                
                router.MapMiddlewareGet("/", 
                    builder => builder.UseMiddleware<GetThingsMiddleware>(thingType));
            }
            #endregion
            
            return app.UseRouter(router.Build());;
        }
    }
}
