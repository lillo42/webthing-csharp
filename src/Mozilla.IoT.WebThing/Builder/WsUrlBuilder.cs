using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Mozilla.IoT.WebThing.Builder
{
    public class WsUrlBuilder : IWsUrlBuilder
    {
        private const string WS = "ws";
        private const string WSS = "wss";

        public string Build(HttpRequest request, string thing)
        {
            return UriHelper.BuildAbsolute(GetScheme(request.Scheme), 
                request.Host, 
                string.IsNullOrEmpty(thing) ? request.Path 
                    : request.Path.Add(PathString.FromUriComponent($"/things/{thing}")));
        }

        private static string GetScheme(string scheme)
            => scheme switch
            {
                "https" => WSS,
                _ => WS
            };
    }
}
