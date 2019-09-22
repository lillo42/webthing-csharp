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

namespace Mozilla.IoT.WebThing.Test.Endpoints
{
    public class GetActionTest
    {
        private readonly Fixture _fixture;
        private readonly HttpContext _httpContext;
        private readonly HttpResponse _response;
        private readonly IHttpRouteValue _routeValue;
        private readonly IServiceProvider _service;

        private readonly ILogger<GetAction> _logger;
        private readonly IThingActivator _thingActivator;

        public GetActionTest()
        {
            _fixture = new Fixture();

            _logger = Substitute.For<ILogger<GetAction>>();
            _thingActivator = Substitute.For<IThingActivator>();
            _service = Substitute.For<IServiceProvider>();
            _routeValue = Substitute.For<IHttpRouteValue>();

            _service.GetService(typeof(IThingActivator))
                .Returns(_thingActivator);

            _service.GetService(typeof(ILogger<GetAction>))
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
            
            _routeValue.GetValue<string>("name")
                .Returns(_fixture.Create<string>());

            await GetAction.Invoke(_httpContext);
            code.Should().Be((int)HttpStatusCode.NotFound);

            _thingActivator
                .Received(1)
                .CreateInstance(_service, thing);
        }
        
        [Fact]
        public async Task Invoke_Should_Return404_When_ActionDoesNotContainsAction()
        {
            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            var thingId = _fixture.Create<string>();
            var thing = Substitute.For<Thing>();

            _thingActivator.CreateInstance(_service, thingId)
                .Returns(thing);

            thing.Actions
                .Returns(new ActionCollection());

            _routeValue.GetValue<string>("thing")
                .Returns(thingId);
            
            _routeValue.GetValue<string>("name")
                .Returns(_fixture.Create<string>());
            
            await GetAction.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.NotFound);

            _thingActivator
                .Received(1)
                .CreateInstance(_service, thingId);
        }
        
        [Fact]
        public async Task Invoke_Should_Return200_When_ActionContainsAction()
        {
            int code = default;
            _response.StatusCode = Arg.Do<int>(args => code = args);

            var thingId = _fixture.Create<string>();
            var thing = _fixture.Create<Thing>();

            _thingActivator.CreateInstance(_service, thingId)
                .Returns(thing);

            var actionName = _fixture.Create<string>();
            var action = Substitute.For<Action>();
            var actionId = _fixture.Create<string>();

            action.Id.Returns(actionId);
            action.Name.Returns(actionName);
            
            thing.Actions.Add(action);
            
            _routeValue.GetValue<string>("thing")
                .Returns(thingId);
            
            _routeValue.GetValue<string>("name")
                .Returns(actionName);

            var descriptor = Substitute.For<IDescriptor<Action>>();

            descriptor.CreateDescription(action)
                .Returns(_fixture.Create<Dictionary<string, object>>());

            _service.GetService(typeof(IDescriptor<Action>))
                .Returns(descriptor);

            var cancel = CancellationToken.None;

            _httpContext.RequestAborted
                .Returns(cancel);
            
            var writer = Substitute.For<IHttpBodyWriter>();

            writer.WriteAsync(Arg.Any<LinkedList<Dictionary<string, object>>>(), cancel)
                .Returns(new ValueTask());

            _service.GetService(typeof(IHttpBodyWriter))
                .Returns(writer);

            await GetAction.Invoke(_httpContext);

            code.Should().Be((int)HttpStatusCode.OK);

            _thingActivator
                .Received(1)
                .CreateInstance(_service, thingId);

            descriptor
                .Received(1)
                .CreateDescription(action);

            await writer
                .Received(1)
                .WriteAsync(Arg.Any<LinkedList<Dictionary<string, object>>>(), cancel);
        }
    }
}
