using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceExtensions
    {
        public static IThingCollectionBuilder AddThings(this IServiceCollection service, Action<ThingOption>? options = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var thingOption = new ThingOption();
            options?.Invoke(thingOption);

            service.AddSingleton(thingOption);
            
            var builder = new ThingCollectionBuilder(service);
            return builder;
        }
    }
}
