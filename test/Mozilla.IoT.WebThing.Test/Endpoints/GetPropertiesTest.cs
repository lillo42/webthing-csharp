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
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Descriptor;
using Mozilla.IoT.WebThing.Endpoints;
using NSubstitute;
using Xunit;
using static Mozilla.IoT.WebThing.Endpoints.GetProperties;

namespace Mozilla.IoT.WebThing.Test.Endpoints
{
    public class GetPropertiesTest
    {
        private readonly Fixture _fixture;
        private readonly HttpContext _httpContext;
        private readonly HttpResponse _response;
        private readonly IHttpRouteValue _routeValue;
        private readonly IServiceProvider _service;

        private readonly ILogger<GetProperties> _logger;
        private readonly IThingActivator _thingActivator;

        public GetPropertiesTest()
        {
            _fixture = new Fixture();

            _logger = Substitute.For<ILogger<GetProperties>>();
            _thingActivator = Substitute.For<IThingActivator>();
            _service = Substitute.For<IServiceProvider>();
            _routeValue = Substitute.For<IHttpRouteValue>();

            _service.GetService(typeof(IThingActivator))
                .Returns(_thingActivator);

            _service.GetService(typeof(ILogger<GetProperties>))
                .Returns(_logger);

            _service.GetService(typeof(IHttpRouteValue))
                .Returns(_routeValue);
            
            _response = Substitute.For<HttpResponse>();
            _httpContext = Substitute.For<HttpContext>();

            _httpContext.RequestServices
                .Returns(_service);

            _httpContext.Response
                .Returns(_response);
        }
        
        [Fact]
        public async Task Invoke_Should_ReturnNotFound_When_ThingNotExists()
        {
            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            var thing = _fixture.Create<string>();

            _thingActivator.CreateInstance(_service, thing)
                .Returns(null as Thing);

            _routeValue.GetValue<string>("thing")
                .Returns(thing);

            await Invoke(_httpContext);
            code.Should().Be((int)HttpStatusCode.NotFound);

            _thingActivator
                .Received(1)
                .CreateInstance(_service, thing);
        }
        
        [Fact]
        public async Task Invoke_Should_Return200_When_HaveEvent()
        {
            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            var thingId = _fixture.Create<string>();
            var thing = _fixture.Create<Thing>();

            _thingActivator.CreateInstance(_service, thingId)
                .Returns(thing);

            _routeValue.GetValue<string>("thing")
                .Returns(thingId);

            var property = Substitute.For<Property>();
            thing.Properties.Add(property);

            var cancel = CancellationToken.None;

            _httpContext.RequestAborted
                .Returns(cancel);
            
            var writer = Substitute.For<IHttpBodyWriter>();

            writer.WriteAsync(Arg.Any<LinkedList<Dictionary<string, object>>>(), cancel)
                .Returns(new ValueTask());

            _service.GetService(typeof(IHttpBodyWriter))
                .Returns(writer);

            await Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.OK);

            _thingActivator
                .Received(1)
                .CreateInstance(_service, thingId);

            await writer
                .Received(1)
                .WriteAsync(Arg.Any<Dictionary<string, object>>(), cancel);
        }
    }
}
