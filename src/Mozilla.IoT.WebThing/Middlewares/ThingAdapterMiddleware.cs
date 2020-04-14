using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Middlewares
{
    internal class ThingAdapterMiddleware
    {
        private readonly IEnumerable<Thing> _things;
        private readonly ThingOption _option;
        private readonly RequestDelegate _next;

        private static bool s_isIdResolved = false;
        private static readonly object s_locker = new object();
        public ThingAdapterMiddleware(RequestDelegate next, IEnumerable<Thing> things, ThingOption option)
        {
            _next = next;
            _things = things;
            _option = option;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (!s_isIdResolved)
            {
                lock (s_locker)
                {
                    if (!s_isIdResolved)
                    {
                        var value = UriHelper.BuildAbsolute(httpContext.Request.Scheme, httpContext.Request.Host);
                       
                        foreach (var thing in _things)
                        {
                            var builder = new UriBuilder(value)
                            {
                                Path = $"/things/{_option.PropertyNamingPolicy.ConvertName(thing.Name)}"
                            };
                            
                            if (_option.UseThingAdapterUrl)
                            {
                                thing.ThingContext.Response.Add(_option.PropertyNamingPolicy.ConvertName("Id"), thing.Name);
                                thing.ThingContext.Response.Add(_option.PropertyNamingPolicy.ConvertName("Href"), builder.Path);
                                thing.ThingContext.Response.Add(_option.PropertyNamingPolicy.ConvertName("base"), builder.Uri.ToString());
                            }
                            else
                            {
                                thing.ThingContext.Response.Add(_option.PropertyNamingPolicy.ConvertName("Id"), builder.Uri.ToString());
                            }
                            
                            builder.Scheme = builder.Scheme == "http" ? "ws" : "wss";
                            ((List<Link>)thing.ThingContext.Response[_option.PropertyNamingPolicy.ConvertName("Links")])!
                                .Add(new Link("alternate", builder.Uri.ToString()));
                        }
                        s_isIdResolved = true;
                    }
                }
            }
            
            return _next(httpContext);
        }
    }
}
