using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Actions
{
    public class ThingActionInformationTest
    {
        private readonly ILogger<ThingActionInformation> _logger;
        private readonly IServiceProvider _provider;
        private readonly Thing _thing;
        private readonly Fixture _fixture;

        public ThingActionInformationTest()
        {
            _fixture = new Fixture();

            _thing = Substitute.For<Thing>();
            _logger = Substitute.For<ILogger<ThingActionInformation>>();
            _provider = Substitute.For<IServiceProvider>();

            _thing.Name.Returns(_fixture.Create<string>());
            
            _provider.GetService(typeof(ILogger<ThingActionInformation>))
                .Returns(_logger);
        }
        
        [Fact]
        public async Task Execute_Should_Run()
        {
            var counter = 0;
            
            var info = new Fake();
            info.StatusChanged += (_, __) => counter++;
            info.TimeCompleted.Should().BeNull();

            await info.ExecuteAsync(_thing, _provider);

            info.Status.Should().Be(ActionStatus.Completed);
            info.TimeCompleted.Should().NotBeNull();
            counter.Should().Be(3);
        }
        
        [Fact]
        public async Task Execute_Should_Run_When_InternalExecutionThrow()
        {
            var counter = 0;
            
            var info = new Fake
            {
                ShouldThrow = true
            };
            
            info.StatusChanged += (_, __) => counter++;
            info.TimeCompleted.Should().BeNull();

            await info.ExecuteAsync(_thing, _provider);

            info.Status.Should().Be(ActionStatus.Completed);
            info.TimeCompleted.Should().NotBeNull();
            counter.Should().Be(3);
        }
        
        public class Fake : ThingActionInformation
        {
            public bool ShouldThrow { get; set; }
            protected override ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider)
            {
                thing.Should().NotBeNull();
                provider.Should().NotBeNull();

                if (ShouldThrow)
                {
                    throw new Exception();
                }
                
                return new ValueTask();
            }

            public override string GetActionName() => "fake";
        }
    }
}
