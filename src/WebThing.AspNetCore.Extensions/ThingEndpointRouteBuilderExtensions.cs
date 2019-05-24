using System;
using WebThing;

namespace Microsoft.AspNetCore.Builder
{
    public static class ThingEndpointRouteBuilderExtensions
    {
        public static void UseThing(this IApplicationBuilder app, Action<ThingBindingOption> thing)
        {
            
        }
    }

    public class ThingBindingOption
    {
        public void AddThing<T>()
            where T : Thing
        {
            
        }
    }
}
