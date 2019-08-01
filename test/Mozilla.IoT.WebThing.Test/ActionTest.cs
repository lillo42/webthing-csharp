using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test
{
    public class ActionTest
    {
        private readonly Action _action;
        private readonly ILogger _logger;

        public ActionTest()
        {
            _action = new TestAction();
            _logger = Substitute.For<ILogger>();
        }

        [Fact]
        public async Task ExecuteAsync_Should_ChangeStatusAndTimeCompleted_When_StartAsyncIsCalled()
        {
            _action.Status.Should().Be(Status.Created);
            _action.TimeCompleted.Should().BeNull();

            Task action =  _action.StartAsync(_logger, CancellationToken.None);
            
            _action.Status.Should().Be(Status.Pending);
            _action.TimeCompleted.Should().BeNull();
            ((TestAction) _action).Wait = false;

            await action;
            
            _action.Status.Should().Be(Status.Completed);
            _action.TimeCompleted.Should().NotBeNull();
        }

        private class TestAction : Action
        {
            internal volatile bool Wait = true;
            public override string Name => "test";

            protected override async Task ExecuteAsync(CancellationToken cancellation)
            {
                while (Wait)
                {
                    await Task.Delay(10, cancellation);
                }
            }
        }
    }
}
