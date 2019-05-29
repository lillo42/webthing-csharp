using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares;
using NSubstitute;
using Xunit;
using static Xunit.Assert;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions.Test.Middlewares
{
    public class GetThingsMiddlewareTest
    {
        private readonly Fixture _fixture;
        private readonly ILoggerFactory _factory;
        private readonly RequestDelegate _next;

        private readonly MemoryStream _body;
        private readonly HttpContext _httpContext;
        private readonly HttpResponse _response;
        private readonly IRoutingFeature _routing;
        
        public GetThingsMiddlewareTest()
        {
            _factory = Substitute.For<ILoggerFactory>();
            _next = Substitute.For<RequestDelegate>();
            _httpContext = Substitute.For<HttpContext>();
            _body = new MemoryStream();
            _response = Substitute.For<HttpResponse>();
            _routing = Substitute.For<IRoutingFeature>();
            
            _httpContext.Features[typeof(IRoutingFeature)].Returns(_routing);
            _httpContext.Response.Returns(_response);
            _response.Body.Returns(_body);

            _fixture = new Fixture();
        }
        
        #region Multi

        [Fact]
        public async Task Invoke_Multi()
        {
            var thing = _fixture.Create<Thing>();
            
            var property = _fixture.Create<Property<int>>();
            property.Value = _fixture.Create<int>();
            thing.AddProperty(property);
            thing.AddProperty(_fixture.Create<Property<int>>());
            
            var multi = new MultipleThings(new List<Thing>
                {
                    thing,
                    _fixture.Create<Thing>()
                },
                _fixture.Create<string>() );
            var middleware = new GetThingsMiddleware(_next, _factory, multi);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            _routing.RouteData.Returns(new RouteData());
            
            await middleware.Invoke(_httpContext);

            True(code == (int)HttpStatusCode.OK);
            True(_body.Length > 0);
        }
        #endregion

    }
}
