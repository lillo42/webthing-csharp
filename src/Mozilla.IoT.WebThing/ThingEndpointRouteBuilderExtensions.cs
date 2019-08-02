using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Endpoints;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Middleware;
using Mozilla.IoT.WebThing.Notify;
using Action = Mozilla.IoT.WebThing.Action;

namespace Microsoft.AspNetCore.Builder
{
    public static class ThingEndpointRouteBuilderExtensions
    {
        public static ThingEndpointConventionBuilder MapThing<T>(this IEndpointRouteBuilder builder)
            where T : Thing
        {
            var activator = builder.ServiceProvider.GetService<IThingActivator>();
            activator.Register<T>(typeof(T).Name);
            
            ValidateServicesRegistered(builder.ServiceProvider);
            
            var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService<ServiceRouteBuilder>();
            var endpointConventionBuilders = serviceRouteBuilder.Build(builder);

            return new ThingEndpointConventionBuilder(endpointConventionBuilders);
        }
        
        private static void ValidateServicesRegistered(IServiceProvider serviceProvider)
        {
            var marker = serviceProvider.GetService(typeof(ThingMarkService));
            if (marker == null)
            {
                throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling " +
                                                    "'IServiceCollection.AddThing' inside the call to 'ConfigureServices(...)' in the application startup code.");
            }
        }

        public static IApplicationBuilder UseThing(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseCors();
            app.UseWebSockets();
            MapThingsRoute(app, app.ApplicationServices.GetService<IReadOnlyList<Thing>>());

            return app;
        }

        private static IApplicationBuilder MapThingsRoute(IApplicationBuilder app, IReadOnlyList<Thing> thingType)
        {
            var router = new RouteBuilder(app);

            string prefix = string.Empty;

            #region Thing

            if (thingType is SingleThingCollection)
            {
                router.MapMiddlewareGet("/",
                    builder => builder.UseMiddleware<GetThingMiddleware>(thingType));
            }
            else
            {
                prefix = "/{thingId}";
                router.MapMiddlewareGet(prefix,
                    builder => builder.UseMiddleware<GetThingMiddleware>(thingType));

                router.MapMiddlewareGet("/",
                    builder => builder.UseMiddleware<GetThingsMiddleware>(thingType));
            }

            #endregion

            #region Property

            router.MapMiddlewareGet($"{prefix}/properties",
                builder => builder.UseMiddleware<GetPropertiesMiddleware>(thingType));

            router.MapMiddlewareGet($"{prefix}/properties/{{propertyName}}",
                builder => builder.UseMiddleware<GetPropertyThingMiddleware>(thingType));

            router.MapMiddlewarePut($"{prefix}/properties/{{propertyName}}",
                builder => builder.UseMiddleware<PutSetPropertyMiddleware>(thingType));

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

            #endregion
            

            return app.UseRouter(router.Build());
        }
    }
}
