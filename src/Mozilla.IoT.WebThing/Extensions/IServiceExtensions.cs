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
        public static IThingCollectionBuilder AddThings(this IServiceCollection service, Action<JsonSerializerOptions>? options = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var jsonOption = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            };
            
            jsonOption.Converters.Add(new ThingConverter());
            options?.Invoke(jsonOption);
            service.TryAddSingleton(jsonOption);
            service.TryAddSingleton<IThingConverterFactory, ThingConverterFactory>();
            var builder = new ThingCollectionBuilder(service);
            return builder;
        }
    }
}
