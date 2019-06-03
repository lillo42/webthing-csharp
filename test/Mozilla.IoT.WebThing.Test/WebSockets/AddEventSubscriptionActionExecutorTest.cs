using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.WebSockets;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.WebSockets
{
    public class AddEventSubscriptionActionExecutorTest
    {
        private readonly Fixture _fixture;

        public AddEventSubscriptionActionExecutorTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void CheckActionName()
        {
            var executor = new AddEventSubscriptionActionExecutor();
            executor.Action.Should().Be("addEventSubscription");
        }
        
        [Fact]
        public async Task ExecuteAsync_Not_Token()
        {
            var executor = new AddEventSubscriptionActionExecutor();

            Thing thing = _fixture.Create<Thing>();

            await executor.ExecuteAsync(thing, Substitute.For<WebSocket>(), new JObject(), CancellationToken.None);
        }
        
        [Fact]
        public async Task ExecuteAsync()
        {
            var executor = new AddEventSubscriptionActionExecutor();

            var thing = Substitute.For<Thing>();

            string @event = _fixture.Create<string>(); 
            thing.AddAvailableEvent(@event);
            var ws = Substitute.For<WebSocket>();
            
            thing.AddEventSubscriber(Arg.Any<string>(), Arg.Any<WebSocket>());
            
            await executor.ExecuteAsync(thing, ws, JObject.Parse($@"{{
                ""{_fixture.Create<string>()}"":{{
                    ""input"": {{
                        ""{_fixture.Create<string>()}"": {_fixture.Create<int>()}
                    }}
                }}
            }}"), CancellationToken.None);

            thing.Received(1)
                .AddEventSubscriber(Arg.Any<string>(), Arg.Any<WebSocket>());

        }
    }
}
