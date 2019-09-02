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
        private readonly BufferBlock<Action> _buffer;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory; 
        private readonly ActionExecutorHostedService _executor;
        
        public ActionExecutorHostedServiceTest()
        {
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

            await _executor.StopAsync(source.Token);
        }
        
        [Fact]
        public async Task ExecuteAsyncAndWait()
        {
            var source = new CancellationTokenSource();
            _executor.StartAsync(source.Token);
//                .ConfigureAwait(false);

            _buffer.Post(new CustomAction{ Delay = 3_000});
            _buffer.Post(new CustomAction{ Delay = 3_000});
            _buffer.Post(new CustomAction{ Delay = 3_000});
            _buffer.Post(new CustomAction{ Delay = 3_000});
            _buffer.Post(new CustomAction{ Delay = 3_000});
            
            source.Cancel();
            await Task.Delay(1_000);
            await _executor.StopAsync(source.Token);
//                .ConfigureAwait(false);
            await Task.Delay(5_000);
        }
        
        private class CustomAction : Action
        {
            public int Delay { get; set; } = 10;
            protected override async ValueTask ExecuteAsync(CancellationToken cancellation) 
                => await Task.Delay(Delay);
        }
    }
}
