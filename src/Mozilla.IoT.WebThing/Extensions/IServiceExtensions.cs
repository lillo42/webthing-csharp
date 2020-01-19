using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceExtensions
    {
        public static IThingCollectionBuilder AddThings(this IServiceCollection service, Action<JsonSerializerOptions> options = null)
        {
            service.Configure("ThingOption",options)
                .PostConfigure<JsonSerializerOptions>("ThingOption", opt =>
                {
                    opt.Converters.Add(new ThingConverter());
                });
            
            service.TryAddSingleton<IThingConverterFactory, ThingConverterFactory>();
            var builder = new ThingCollectionBuilder(service);
            return builder;
        }
    }
}
