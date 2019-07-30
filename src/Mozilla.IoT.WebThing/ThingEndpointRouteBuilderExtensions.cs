using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Middleware;
using Mozilla.IoT.WebThing.Notify;

using Action = Mozilla.IoT.WebThing.Action;

namespace Microsoft.AspNetCore.Builder
{
    public static class ThingEndpointRouteBuilderExtensions
    {
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

            BindingThingNotify(app.ApplicationServices, thingType);

            return app.UseRouter(router.Build());
        }

        private static void BindingThingNotify(IServiceProvider serviceProvider, IReadOnlyList<Thing> thingType)
        {
            var eventDescription = serviceProvider.GetService<IDescription<Event>>();

            var jsonConvert = serviceProvider.GetService<IJsonConvert>();
            var jsonSettings = serviceProvider.GetService<IJsonSerializerSettings>();
            var jsonSchemaValidator = serviceProvider.GetService<IJsonSchemaValidator>();

            var actionNotify = new NotifySubscribesOnActionAdded(serviceProvider.GetService<IDescription<Action>>(),
                jsonConvert,
                jsonSettings
            );

            var propertyNotify = new NotifySubscribesOnPropertyChanged(jsonConvert, jsonSettings);

            foreach (Thing thing in thingType)
            {
                var eventNotify = new NotifySubscribesOnEventAdded(thing,
                    eventDescription,
                    jsonConvert,
                    jsonSettings
                );

                if (thing.Events == null)
                {
                    thing.Events = serviceProvider.GetService<IObservableCollection<Event>>();
                }
                
                ((PropertyCollection) thing.Properties).JsonSchemaValidator = jsonSchemaValidator;

                thing.Events.CollectionChanged += eventNotify.Notify;
                thing.Actions.CollectionChanged += actionNotify.Notify;
                thing.Properties.Cast<PropertyProxy>().ForEach(property =>
                {
                    property.SchemaValidator = jsonSchemaValidator;
                    property.ValuedChanged += propertyNotify.Notify;
                });
            }
        }
    }
}
