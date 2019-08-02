using System;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing;

namespace Microsoft.AspNetCore.Routing
{
    public static class ThingEndpointRouteBuilderExtensions
    {
        public static ThingEndpointConventionBuilder MapThing<T>(this IEndpointRouteBuilder builder)
            where T : Thing
        {
            var activator = builder.ServiceProvider.GetService<IThingActivator>();
            activator.Register<T>(builder.ServiceProvider);
            
            ValidateServicesRegistered(builder.ServiceProvider);
            
            var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService<ServiceRouteBuilder>();
            var endpointConventionBuilders = serviceRouteBuilder.Build(builder);

            return new ThingEndpointConventionBuilder(endpointConventionBuilders);
        }
        
        public static ThingEndpointConventionBuilder MapThing<T>(this IEndpointRouteBuilder builder, string thing)
            where T : Thing
        {
            var activator = builder.ServiceProvider.GetService<IThingActivator>();
            activator.Register<T>(builder.ServiceProvider, thing);
            
            ValidateServicesRegistered(builder.ServiceProvider);
            
            var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService<ServiceRouteBuilder>();
            var endpointConventionBuilders = serviceRouteBuilder.Build(builder);

            return new ThingEndpointConventionBuilder(endpointConventionBuilders);
        }
        
        public static ThingEndpointConventionBuilder MapThing<T>(this IEndpointRouteBuilder builder, T thing)
            where T : Thing
        {
            var activator = builder.ServiceProvider.GetService<IThingActivator>();
            activator.Register<T>(builder.ServiceProvider, thing);
            
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
    }
}
