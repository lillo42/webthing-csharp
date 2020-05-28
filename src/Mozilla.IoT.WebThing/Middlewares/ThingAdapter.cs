using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Middlewares
{
    internal static class ThingAdapter
    {
        private static readonly object s_locker = new object();
        public static void Adapt(HttpContext context, Thing thing)
        {
            var response = thing.ThingContext.Response;
            if (!response.ContainsKey("id"))
            {
                lock (s_locker)
                {
                    if (!response.ContainsKey("id"))
                    {
                        var option = context.RequestServices.GetRequiredService<ThingOption>();
                        var value = UriHelper.BuildAbsolute(context.Request.Scheme, context.Request.Host);
                        var builder = new UriBuilder(value) {Path = $"/things/{option.PropertyNamingPolicy.ConvertName(thing.Name)}"};

                        if (option.UseThingAdapterUrl)
                        {
                            response.TryAdd("id", thing.Id);
                            response.TryAdd("href", builder.Path);
                            response.TryAdd("base", builder.Uri.ToString());
                        }
                        else
                        {
                            response.TryAdd("id", builder.Uri.ToString());
                        }
                            
                        builder.Scheme = builder.Scheme == "http" ? "ws" : "wss";
                        
                        var links =  (List<Link>)response[option.PropertyNamingPolicy.ConvertName("Links")]!;
                        links.Add(new Link("alternate", builder.Uri.ToString()));
                    }
                }
            }
        }
    }
}
