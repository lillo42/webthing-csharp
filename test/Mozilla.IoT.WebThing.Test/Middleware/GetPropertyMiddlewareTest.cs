//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Threading.Tasks;
//using AutoFixture;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.Logging;
//using Mozilla.IoT.WebThing.Middleware;
//using Newtonsoft.Json;
//using NSubstitute;
//using Xunit;
//using static Xunit.Assert;
//
//namespace Mozilla.IoT.WebThing.Test.Middleware
//{
//    public class GetPropertyMiddlewareTest
//    {
//        private readonly Fixture _fixture;
//        private readonly ILoggerFactory _factory;
//        private readonly RequestDelegate _next;
//
//        private readonly MemoryStream _body;
//        private readonly HttpContext _httpContext;
//        private readonly HttpResponse _response;
//        private readonly IRoutingFeature _routing;
//        private readonly IServiceProvider _service;
//        
//        public GetPropertyMiddlewareTest()
//        {
//            _factory = Substitute.For<ILoggerFactory>();
//            _next = Substitute.For<RequestDelegate>();
//            _httpContext = Substitute.For<HttpContext>();
//            _routing = Substitute.For<IRoutingFeature>();
//            _body = new MemoryStream();
//            _response = Substitute.For<HttpResponse>();
//            _service = Substitute.For<IServiceProvider>();
//
//            _httpContext.Features[typeof(IRoutingFeature)].Returns(_routing);
//            _httpContext.Response.Returns(_response);
//            _response.Body.Returns(_body);
//            
//            _httpContext.RequestServices.Returns(_service);
//
//            _service.GetService(typeof(JsonSerializerSettings))
//                .Returns(new JsonSerializerSettings());
//
//            _fixture = new Fixture();
//        }
//        
//        #region Single
//
//        [Fact]
//        public async Task Invoke_Single_NotFound()
//        {
//            var single = new SingleThing(null);
//
//            var middleware = new GetPropertyThingMiddleware(_next, _factory, single);
//
//            int code = default;
//            _response.StatusCode = Arg.Do<int>(args => code = args);
//
//
//            _routing.RouteData.Returns(new RouteData(
//                RouteValueDictionary.FromArray(new[]
//                {
//                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
//                })));
//
//            await middleware.Invoke(_httpContext);
//
//            True(code == (int)HttpStatusCode.NotFound);
//        }
//        
//        [Fact]
//        public async Task Invoke_Single_Property_NotFound()
//        {
//            var thing = _fixture.Create<Thing>();
//            
//            thing.AddProperty(_fixture.Create<Property<int>>());
//
//            var single = new SingleThing(thing);
//            var middleware = new GetPropertyThingMiddleware(_next, _factory, single);
//
//            int code = default;
//            _response.StatusCode = Arg.Do<int>(args => code = args);
//            
//            _routing.RouteData.Returns(new RouteData(
//                RouteValueDictionary.FromArray(new[]
//                {
//                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
//                    new KeyValuePair<string, object>("propertyName", _fixture.Create<string>()),
//                })));
//
//            await middleware.Invoke(_httpContext);
//
//            True(code == (int)HttpStatusCode.NotFound);
//        }
//        
//        [Fact]
//        public async Task Invoke_Single()
//        {
//            var thing = _fixture.Create<Thing>();
//
//            var property = _fixture.Create<Property<int>>();
//            property.Value = _fixture.Create<int>();
//            thing.AddProperty(property);
//            thing.AddProperty(_fixture.Create<Property<int>>());
//
//            var single = new SingleThing(thing);
//            var middleware = new GetPropertyThingMiddleware(_next, _factory, single);
//
//            int code = default;
//            _response.StatusCode = Arg.Do<int>(args => code = args);
//            
//            _routing.RouteData.Returns(new RouteData(
//                RouteValueDictionary.FromArray(new[]
//                {
//                    new KeyValuePair<string, object>("thingId", _fixture.Create<int>()),
//                    new KeyValuePair<string, object>("propertyName", property.Name),
//                })));
//
//            await middleware.Invoke(_httpContext);
//
//            True(code == (int)HttpStatusCode.OK);
//            True(_body.Length > 0);
//        }
//        #endregion
//        
//        #region Multi
//
//        [Theory]
//        [InlineData(1)]
//        [InlineData(-1)]
//        public async Task Invoke_Multi_NotFound(int index)
//        {
//            var multi = new MultipleThings(new List<Thing>(), _fixture.Create<string>());
//
//            var middleware = new GetPropertyThingMiddleware(_next, _factory, multi);
//
//            int code = default;
//            _response.StatusCode = Arg.Do<int>(args => code = args);
//
//            _routing.RouteData.Returns(new RouteData(
//                RouteValueDictionary.FromArray(new[] {new KeyValuePair<string, object>("thingId", index),})));
//
//            await middleware.Invoke(_httpContext);
//
//            True(code == (int)HttpStatusCode.NotFound);
//        }
//        
//        [Fact]
//        public async Task Invoke_Multi_Property_Not_Found()
//        {
//            var thing = _fixture.Create<Thing>();
//            
//            var property = _fixture.Create<Property<int>>();
//            property.Value = _fixture.Create<int>();
//            thing.AddProperty(property);
//            thing.AddProperty(_fixture.Create<Property<int>>());
//            
//            var single = new MultipleThings(new List<Thing>
//                {
//                    thing,
//                    _fixture.Create<Thing>()
//                },
//                _fixture.Create<string>() );
//            var middleware = new GetPropertyThingMiddleware(_next, _factory, single);
//
//            int code = default;
//            _response.StatusCode = Arg.Do<int>(args => code = args);
//
//
//            _routing.RouteData.Returns(new RouteData(
//                RouteValueDictionary.FromArray(new[]
//                {
//                    new KeyValuePair<string, object>("thingId", 0),
//                    new KeyValuePair<string, object>("propertyName", _fixture.Create<string>()),
//                })));
//
//            await middleware.Invoke(_httpContext);
//
//            True(code == (int)HttpStatusCode.NotFound);
//        }
//        
//        [Fact]
//        public async Task Invoke_Multi()
//        {
//            var thing = _fixture.Create<Thing>();
//            
//            var property = _fixture.Create<Property<int>>();
//            property.Value = _fixture.Create<int>();
//            thing.AddProperty(property);
//            thing.AddProperty(_fixture.Create<Property<int>>());
//            
//            var single = new MultipleThings(new List<Thing>
//                {
//                    thing,
//                    _fixture.Create<Thing>()
//                },
//                _fixture.Create<string>() );
//            var middleware = new GetPropertyThingMiddleware(_next, _factory, single);
//
//            int code = default;
//            _response.StatusCode = Arg.Do<int>(args => code = args);
//
//
//            _routing.RouteData.Returns(new RouteData(
//                RouteValueDictionary.FromArray(new[]
//                {
//                    new KeyValuePair<string, object>("thingId", 0),
//                    new KeyValuePair<string, object>("propertyName", property.Name),
//                })));
//
//            await middleware.Invoke(_httpContext);
//
//            True(code == (int)HttpStatusCode.OK);
//            True(_body.Length > 0);
//        }
//        #endregion
//
//    }
//}
