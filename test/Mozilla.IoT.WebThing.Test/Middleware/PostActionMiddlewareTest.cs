using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AutoFixture;
using FluentAssertions;
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
    public class PostActionMiddlewareTest
    {
        private readonly Fixture _fixture;
        private readonly ILoggerFactory _factory;
        private readonly RequestDelegate _next;

        private readonly MemoryStream _requestBody;
        private readonly MemoryStream _responseBody;
        private readonly HttpContext _httpContext;
        private readonly HttpResponse _response;
        private readonly HttpRequest _request;
        private readonly IRoutingFeature _routing;
        private readonly IServiceProvider _service;
        
        public PostActionMiddlewareTest()
        {
            _factory = Substitute.For<ILoggerFactory>();
            _next = Substitute.For<RequestDelegate>();
            _httpContext = Substitute.For<HttpContext>();
            _routing = Substitute.For<IRoutingFeature>();
            _requestBody = new MemoryStream();
            _responseBody = new MemoryStream();
            _response = Substitute.For<HttpResponse>();
            _request = Substitute.For<HttpRequest>();
            _service = Substitute.For<IServiceProvider>();

            _httpContext.Features[typeof(IRoutingFeature)].Returns(_routing);
            _httpContext.Response.Returns(_response);
            _httpContext.Request.Returns(_request);
            _response.Body.Returns(_responseBody);
            _request.Body.Returns(_requestBody);
            _httpContext.RequestServices.Returns(_service);

            _service.GetService(typeof(JsonSerializerSettings))
                .Returns(new JsonSerializerSettings());

            _fixture = new Fixture();
        }

        #region Single

        [Fact]
        public async Task Invoke_Single_NotFound()
        {
            var thingType = new SingleThing(null);

            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);


            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                })));

            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Invoke_Single_NotFound_Property()
        {
            var thing = _fixture.Create<Thing>();
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());

            var thingType = new SingleThing(thing);
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);


            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                    new KeyValuePair<string, object>("actionName", _fixture.Create<string>())
                })));

            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes("{}"));

            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Invoke_Single_NotFound_Action()
        {
            var thing = _fixture.Create<Thing>();
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());

            var thingType = new SingleThing(thing);
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);


            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                    new KeyValuePair<string, object>("actionName", _fixture.Create<string>())
                })));
            
            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes($@"{{
                ""input"": {{
                    ""{_fixture.Create<string>()}"": ""{_fixture.Create<string>()}""
                }}
            }}"));
            _requestBody.Seek(0, SeekOrigin.Begin);

            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task Invoke_Single_ActionNotAvailable()
        {
            var thing = _fixture.Create<Thing>();
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());

            var thingType = new SingleThing(thing);
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            string actionName = _fixture.Create<string>();

            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                    new KeyValuePair<string, object>("actionName", actionName)
                })));

            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes($@"{{
                ""{actionName}"": {{
                    ""input"": {{
                        ""{_fixture.Create<string>()}"": ""{_fixture.Create<string>()}""
                    }}
                }}
            }}"));
            
            _requestBody.Seek(0, SeekOrigin.Begin);
            
            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task Invoke_Single_No_Input()
        {
            var thing = _fixture.Create<Thing>();
            string actionName = _fixture.Create<string>();
            
            thing.AddAvailableAction<TestAction>(actionName);

            await thing.PerformActionAsync(actionName, null, CancellationToken.None);
            
            var thingType = new SingleThing(thing);
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);
            
            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
                    new KeyValuePair<string, object>("actionName", actionName)
                })));
            
            var buffer = new BufferBlock<Action>();
            _service.GetService(typeof(ITargetBlock<Action>))
                .Returns(buffer);
            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes($@"{{
                ""{actionName}"": {{
                    ""input"": {{
                        ""{_fixture.Create<string>()}"": ""{_fixture.Create<string>()}""
                    }}
                }}
            }}"));
            _requestBody.Seek(0, SeekOrigin.Begin);
            
            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.Created);
            _responseBody.Length.Should().BeGreaterThan(0);
            buffer.Count.Should().BeGreaterThan(0);
        }

        #endregion

        #region Multi

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public async Task Invoke_Multi_NotFound(int index)
        {
            var thingType = new MultipleThings(new List<Thing>(), _fixture.Create<string>());

            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[] {new KeyValuePair<string, object>("thingId", index),})));

            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Invoke_Multi_NotFound_Property()
        {
            var thing = _fixture.Create<Thing>();
            string actionName = _fixture.Create<string>();
            
            thing.AddAvailableAction<TestAction>(actionName);

            var thingType = new MultipleThings(new List<Thing>
                {
                    thing,
                    _fixture.Create<Thing>()
                },
                _fixture.Create<string>() );
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);


            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", 0),
                    new KeyValuePair<string, object>("actionName", _fixture.Create<string>())
                })));

            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes("{}"));
            _requestBody.Seek(0, SeekOrigin.Begin);

            await middleware.Invoke(_httpContext);

             code.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Invoke_Multi_NotFound_Action()
        {
            var thing = _fixture.Create<Thing>();
            string actionName = _fixture.Create<string>();
            
            thing.AddAvailableAction<TestAction>(actionName);

            await thing.PerformActionAsync(actionName, null, CancellationToken.None);
            
            var thingType = new MultipleThings(new List<Thing>
                {
                    thing,
                    _fixture.Create<Thing>()
                },
                _fixture.Create<string>() );
            
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);
            
            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", 0),
                    new KeyValuePair<string, object>("actionName", _fixture.Create<string>())
                })));

            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes($@"{{
                ""input"": {{
                    ""{_fixture.Create<string>()}"": ""{_fixture.Create<string>()}""
                }}
            }}"));
            
            _requestBody.Seek(0, SeekOrigin.Begin);

            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task Invoke_Multi_ActionNotAvailable()
        {
            var thing = _fixture.Create<Thing>();
            
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());

            var thingType = new MultipleThings(new List<Thing>
                {
                    thing,
                    _fixture.Create<Thing>()
                },
                _fixture.Create<string>() );
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            string actionName = _fixture.Create<string>();
            
            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId",0),
                    new KeyValuePair<string, object>("actionName", actionName)
                })));

            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes($@"{{
                ""{actionName}"": {{
                    ""input"": {{
                        ""{_fixture.Create<string>()}"": ""{_fixture.Create<string>()}""
                    }}
                }}
            }}"));
            
            _requestBody.Seek(0, SeekOrigin.Begin);

            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task Invoke_Multi_No_Input()
        {
            var thing = _fixture.Create<Thing>();
            string actionName = _fixture.Create<string>();
            
            thing.AddAvailableAction<TestAction>(actionName);

            await thing.PerformActionAsync(actionName, null, CancellationToken.None);
            
            var thingType = new MultipleThings(new List<Thing>
                {
                    thing,
                    _fixture.Create<Thing>()
                },
                _fixture.Create<string>() );
            var middleware = new PostActionMiddleware(_next, _factory, thingType);

            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);
            
            _routing.RouteData.Returns(new RouteData(
                RouteValueDictionary.FromArray(new[]
                {
                    new KeyValuePair<string, object>("thingId", 0),
                    new KeyValuePair<string, object>("actionName", actionName)
                })));
            
            var buffer = new BufferBlock<Action>();
            _service.GetService(typeof(ITargetBlock<Action>))
                .Returns(buffer);
            
            await _requestBody.WriteAsync(Encoding.UTF8.GetBytes($@"{{
                ""{actionName}"": {{
                    ""input"": {{
                        ""{_fixture.Create<string>()}"": ""{_fixture.Create<string>()}""
                    }}
                }}
            }}"));
            _requestBody.Seek(0, SeekOrigin.Begin);
            
            await middleware.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.Created);
            _responseBody.Length.Should().BeGreaterThan(0);
            buffer.Count.Should().BeGreaterThan(0);
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
