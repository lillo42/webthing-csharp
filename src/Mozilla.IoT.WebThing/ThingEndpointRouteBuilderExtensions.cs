using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    public static class ThingEndpointRouteBuilderExtensions
    {
        public static void MapThing<T>(this IEndpointRouteBuilder builder)
            where T : Thing
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ValidateServicesRegistered(builder.ServiceProvider);

            var things = builder.ServiceProvider.GetService<IReadOnlyList<Thing>>();
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
            return app;
        }

        private static IApplicationBuilder AddRoute(IApplicationBuilder app, IReadOnlyList<Thing> thingType)
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

            foreach (Thing thing in thingType)
            {
                var notify = new NotifySubscribesOnEventAdded(thing,
                    app.ApplicationServices.GetService<IDescription<Event>>(),
                    app.ApplicationServices.GetService<IJsonConvert>(),
                    app.ApplicationServices.GetService<IJsonSerializerSettings>()
                );

                if (thing.Events == null)
                {
                    thing.Events = app.ApplicationServices.GetService<IObservableCollection<Event>>();
                }

                thing.Events.CollectionChanged += notify.Notify;
                
                var jsonSchemaValidator = app.ApplicationServices.GetService<IJsonSchemaValidator>();
                ((PropertyCollection)thing.Properties).JsonSchemaValidator = jsonSchemaValidator;
                thing.Properties.Cast<PropertyProxy>().ForEach(property => property.SchemaValidator = jsonSchemaValidator);
            }

            return app.UseRouter(router.Build());
        }
    }
}
