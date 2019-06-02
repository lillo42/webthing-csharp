using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Middleware;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Middleware
{
    public class GetThingMiddlewareTest
    {
        private readonly Fixture _fixture;
        private readonly ILoggerFactory _factory;
        private readonly RequestDelegate _next;

        private readonly MemoryStream _body;
        private readonly HttpContext _httpContext;
        private readonly HttpResponse _response;
        private readonly IRoutingFeature _routing;

        public GetThingMiddlewareTest()
        {
            _factory = Substitute.For<ILoggerFactory>();
            _next = Substitute.For<RequestDelegate>();
            _httpContext = Substitute.For<HttpContext>();
            _body = new MemoryStream();
            _routing = Substitute.For<IRoutingFeature>();
            _response = Substitute.For<HttpResponse>();

            _httpContext.Features[typeof(IRoutingFeature)].Returns(_routing);
            _httpContext.Response.Returns(_response);
            _response.Body.Returns(_body);

            _fixture = new Fixture();
        }

        #region Single

        [Fact]
        public async Task Invoke_Single()
        {
            var thing = _fixture.Create<Thing>();

            var property = _fixture.Create<Property<int>>();
            property.Value = _fixture.Create<int>();
            thing.AddProperty(property);
            thing.AddProperty(_fixture.Create<Property<int>>());

            var single = new SingleThing(thing);
            var middleware = new GetThingMiddleware(_next, _factory, single);
            
            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            _routing.RouteData.Returns(new RouteData());
            
            await middleware.Invoke(_httpContext);

            Assert.True(code == (int)HttpStatusCode.OK);
            Assert.True(_body.Length > 0);
        }

        #endregion

        #region Multi

        [Fact]
        public async Task Invoke_Multi()
        {
            var thing = _fixture.Create<Thing>();

            var property = _fixture.Create<Property<int>>();
            property.Value = _fixture.Create<int>();
            thing.AddProperty(property);
            thing.AddProperty(_fixture.Create<Property<int>>());

            var multi = new MultipleThings(new List<Thing> {thing, _fixture.Create<Thing>()},
                _fixture.Create<string>());
            var middleware = new GetThingMiddleware(_next, _factory, multi);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", 0),
                })));

            await middleware.Invoke(_httpContext);

            Assert.True(code == (int)HttpStatusCode.OK);
            Assert.True(_body.Length > 0);
        }
        
        [Fact]
        public async Task Invoke_Multi_Not_Found()
        {
            var thing = _fixture.Create<Thing>();

            var property = _fixture.Create<Property<int>>();
            property.Value = _fixture.Create<int>();
            thing.AddProperty(property);
            thing.AddProperty(_fixture.Create<Property<int>>());

            var multi = new MultipleThings(new List<Thing> {thing, _fixture.Create<Thing>()},
                _fixture.Create<string>());
            var middleware = new GetPropertyThingMiddleware(_next, _factory, multi);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                })));

            await middleware.Invoke(_httpContext);

            Assert.True(code == (int)HttpStatusCode.NotFound);
            Assert.True(_body.Length > 0);
        }

        #endregion
    }
}
