using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Background;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Background
{
    public class ActionExecutorHostedServiceTest
    {
        private readonly Fixture _fixture;
        private readonly BufferBlock<Action> _buffer;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory; 
        private readonly ActionExecutorHostedService _executor;
        
        public ActionExecutorHostedServiceTest()
        {
            _fixture = new Fixture();
            _buffer = new BufferBlock<Action>();
            _logger = Substitute.For<ILogger>();
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _loggerFactory.CreateLogger(Arg.Any<string>())
                .Returns(_logger);
            
            _executor = new ActionExecutorHostedService(_buffer, _loggerFactory);
        }

        [Fact]
        public async Task ExecuteAsync()
        {
            var source = new CancellationTokenSource();
            var execution = _executor.StartAsync(source.Token);

            _buffer.Post(new CustomAction());
            _buffer.Post(new CustomAction());
            _buffer.Post(new CustomAction());
            _buffer.Post(new CustomAction());
            _buffer.Post(new CustomAction());
            
            source.CancelAfter(TimeSpan.FromSeconds(1));

            await execution;
        }
        
        private class CustomAction : Action
        {
            protected override Task ExecuteAsync(CancellationToken cancellation) 
                => Task.Delay(10, cancellation);
        }
    }
}
