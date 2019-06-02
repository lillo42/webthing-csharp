using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Middleware;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Middleware
{
    public class GetActionMiddlewareTest
    {
        private readonly Fixture _fixture;
        private readonly ILoggerFactory _factory;
        private readonly RequestDelegate _next;

        private readonly MemoryStream _body;
        private readonly HttpContext _httpContext;
        private readonly HttpResponse _response;
        private readonly IRoutingFeature _routing;
        private readonly IServiceProvider _service;
        
        public GetActionMiddlewareTest()
        {
            _factory = Substitute.For<ILoggerFactory>();
            _next = Substitute.For<RequestDelegate>();
            _httpContext = Substitute.For<HttpContext>();
            _routing = Substitute.For<IRoutingFeature>();
            _body = new MemoryStream();
            _response = Substitute.For<HttpResponse>();
            _service = Substitute.For<IServiceProvider>();

            _httpContext.Features[typeof(IRoutingFeature)].Returns(_routing);
            _httpContext.Response.Returns(_response);
            _response.Body.Returns(_body);
            _httpContext.RequestServices.Returns(_service);

            _service.GetService(typeof(JsonSerializerSettings))
                .Returns(new JsonSerializerSettings());

            _fixture = new Fixture();
        }

        #region Single

        [Fact]
        public async Task Invoke_Single_NotFound()
        {
            var single = new SingleThing(null);

            var middleware = new GetActionMiddleware(_next, _factory, single);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);


            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                })));

            await middleware.Invoke(_httpContext);

            Assert.True(code == (int)HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Invoke_Single()
        {
            var thing = _fixture.Create<Thing>();
            string actionName = _fixture.Create<string>();
            
            thing.AddAvailableAction<TestAction>(actionName);

            await thing.PerformActionAsync(actionName, null, CancellationToken.None);
            
            var single = new SingleThing(thing);
            var middleware = new GetActionMiddleware(_next, _factory, single);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);
            
            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                    new KeyValuePair<string, object>("actionName", actionName)
                })));

            await middleware.Invoke(_httpContext);

            Assert.True(code == (int)HttpStatusCode.OK);
            Assert.True(_body.Length > 0);
        }

        #endregion

        #region Multi

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public async Task Invoke_Multi_NotFound(int index)
        {
            var multi = new MultipleThings(new List<Thing>(), _fixture.Create<string>());

            var middleware = new GetActionMiddleware(_next, _factory, multi);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[] {new KeyValuePair<string, object>("thingId", index),})));

            await middleware.Invoke(_httpContext);

            Assert.True(code == (int)HttpStatusCode.NotFound);
        }

        
        [Fact]
        public async Task Invoke_Multi()
        {
            var thing = _fixture.Create<Thing>();
            string actionName = _fixture.Create<string>();
            
            thing.AddAvailableAction<TestAction>(actionName);

            await thing.PerformActionAsync(actionName, null, CancellationToken.None);
            
            var single = new MultipleThings(new List<Thing>
                {
                    thing,
                    _fixture.Create<Thing>()
                },
                _fixture.Create<string>() );
            var middleware = new GetActionMiddleware(_next, _factory, single);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);


            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", 0),
                    new KeyValuePair<string, object>("actionName", actionName),
                })));

            await middleware.Invoke(_httpContext);

            Assert.True(code == (int)HttpStatusCode.OK);
            Assert.True(_body.Length > 0);
        }
        
        #endregion
        
        
        private class TestAction : Action
        {
            public static string ID { get; } = Guid.NewGuid().ToString();
            public TestAction(Thing thing, JObject input) 
                : base(thing, input)
            {
            }

            public override string Id => ID;
            public override string Name => "test";
        }
    }
}
