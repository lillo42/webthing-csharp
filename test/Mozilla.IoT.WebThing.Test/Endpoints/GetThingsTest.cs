using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;
using Mozilla.IoT.WebThing.Builder;
using Mozilla.IoT.WebThing.Descriptor;
using Mozilla.IoT.WebThing.Endpoints;
using NSubstitute;
using Xunit;
using static Mozilla.IoT.WebThing.Endpoints.GetThings;

namespace Mozilla.IoT.WebThing.Test.Endpoints
{
    public class GetThingsTest
    {
        private readonly Fixture _fixture;
        private readonly HttpContext _httpContext;
        private readonly HttpResponse _response;
        private readonly IHttpBodyWriter _writer; 
        private readonly IServiceProvider _service;

        private readonly ILogger<GetThings> _logger;
        private readonly IThingActivator _thingActivator;
        private readonly IWsUrlBuilder _wsUrlBuilder;
        private readonly IDescriptor<Thing> _descriptor;

        public GetThingsTest()
        {
            _fixture = new Fixture();

            _logger = Substitute.For<ILogger<GetThings>>();
            _writer = Substitute.For<IHttpBodyWriter>();
            _service = Substitute.For<IServiceProvider>();
            _thingActivator = Substitute.For<IThingActivator>();
            _wsUrlBuilder = Substitute.For<IWsUrlBuilder>();
            _descriptor = Substitute.For<IDescriptor<Thing>>();

            _service.GetService(typeof(IThingActivator))
                .Returns(_thingActivator);
            
            _service.GetService(typeof(ILogger<GetThings>))
                .Returns(_logger);
            
            _service.GetService(typeof(IWsUrlBuilder))
                .Returns(_wsUrlBuilder);
            
            _service.GetService(typeof(IDescriptor<Thing>))
                .Returns(_descriptor);
            
            _service.GetService(typeof(IHttpBodyWriter))
                .Returns(_writer);

            _response = Substitute.For<HttpResponse>();
            _httpContext = Substitute.For<HttpContext>();

            _httpContext.RequestServices
                .Returns(_service);

            _httpContext.Response
                .Returns(_response);
        }
        
        [Fact]
        public async Task Invoke_Should_Ok_When_ThereIsNoThing()
        {
            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            var cancel = CancellationToken.None;

            _httpContext.RequestAborted
                .Returns(cancel);
            
            await Invoke(_httpContext);
            code.Should().Be((int)HttpStatusCode.OK);

             await _writer
                .Received(1)
                .WriteAsync(Arg.Is<LinkedList<IDictionary<string, object>>>(dic => dic.Count == 0), cancel);
        }
        
        [Fact]
        public async Task Invoke_Should_Ok_When_ThereIsThing()
        {
            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            var thing = _fixture.Create<Thing>();
            _thingActivator
                .GetEnumerator()
                .Returns(new List<Thing>
                {
                    thing
                }.GetEnumerator());

            var descriptor = _fixture.Create<Dictionary<string, object>>();
            descriptor.TryAdd("links", new List<IDictionary<string,object>>());
            
            _descriptor.CreateDescription(thing)
                .Returns(descriptor);

            _wsUrlBuilder.Build(Arg.Any<HttpRequest>(), thing.Name)
                .Returns(_fixture.Create<string>());

            var cancel = CancellationToken.None;

            _httpContext.RequestAborted
                .Returns(cancel);
            
            await Invoke(_httpContext);
            code.Should().Be((int)HttpStatusCode.OK);
            
            _descriptor
                .Received(1)
                .CreateDescription(thing);

            _wsUrlBuilder
                .Received(1)
                .Build(Arg.Any<HttpRequest>(), thing.Name);
            
            await _writer
                .Received(1)
                .WriteAsync(Arg.Is<LinkedList<IDictionary<string, object>>>(dic => dic.Count == 1), cancel);
        }
    }
}
