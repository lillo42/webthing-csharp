using System;
using System.Linq;
using WebThing;
using WebThing.AspNetCore.Extensions;

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

            var multi = new MultipleThings(option.Things, name);

            return app.UseMiddleware<ThingsMiddleware>(new MultipleThings(option.Things, name));
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
            
            
            
            return app.UseMiddleware<ThingsMiddleware>(new SingleThing(thing));
        }
    }
}
