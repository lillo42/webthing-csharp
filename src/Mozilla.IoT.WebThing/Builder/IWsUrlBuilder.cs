using Microsoft.AspNetCore.Http;

namespace Mozilla.IoT.WebThing.Builder
{
    public interface IWsUrlBuilder
    {
        string Build(HttpRequest request, string thing);
    }
}
