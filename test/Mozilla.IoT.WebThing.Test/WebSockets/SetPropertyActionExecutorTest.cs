using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Exceptions;
using Mozilla.IoT.WebThing.WebSockets;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.WebSockets
{
    public class SetPropertyActionExecutorTest
    {
        private readonly Fixture _fixture;

        public SetPropertyActionExecutorTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Action()
        {
            var executor = new SetPropertyActionExecutor();
            executor.Action.Should().Be("setProperty");
        }
        
        [Fact]
        public async Task ExecuteAsync_Not_Token()
        {
            var executor = new SetPropertyActionExecutor();

            Thing thing = _fixture.Create<Thing>();

            await executor.ExecuteAsync(thing, Substitute.For<WebSocket>(), new JObject(), CancellationToken.None);
        }
        
        [Fact]
        public async Task ExecuteAsync_Throw()
        {
            var executor = new SetPropertyActionExecutor();

            Thing thing = Substitute.For<Thing>();

            thing
                .When(x=> x.SetProperty(Arg.Any<string>(), Arg.Any<object>()))
                .Do(x => throw new PropertyException());

            WebSocket ws = Substitute.For<WebSocket>();
            
             await executor.ExecuteAsync(thing, ws, JObject.Parse($@"{{
                ""{_fixture.Create<string>()}"": {_fixture.Create<int>()}
            }}"), CancellationToken.None);

            await ws.Received(1)
                 .SendAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<WebSocketMessageType>(), Arg.Any<bool>(),
                     Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task ExecuteAsync()
        {
            var executor = new SetPropertyActionExecutor();

            Thing thing = Substitute.For<Thing>();

            thing.SetProperty(Arg.Any<string>(), Arg.Any<object>());

            WebSocket ws = Substitute.For<WebSocket>();
            
            await executor.ExecuteAsync(thing, ws, JObject.Parse($@"{{
                ""{_fixture.Create<string>()}"": {_fixture.Create<int>()}
            }}"), CancellationToken.None);

            thing.Received(1)
                .SetProperty(Arg.Any<string>(), Arg.Any<object>());
            
            await ws.DidNotReceive()
                .SendAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<WebSocketMessageType>(), Arg.Any<bool>(),
                    Arg.Any<CancellationToken>());
        }
    }
}
