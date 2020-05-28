using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Actions
{
    public class ActionInfoTest
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ThingActionInformation> _logger;

        public ActionInfoTest()
        {
            _provider = Substitute.For<IServiceProvider>();
            _logger = Substitute.For<ILogger<ThingActionInformation>>();

            _provider.GetService(typeof(ILogger<ThingActionInformation>))
                .Returns(_logger);
        }

        [Fact]
        public void Execute()
        {
            var action = new VoidThingActionInformation();
            
            action.GetId().Should().NotBeEmpty();
            action.TimeCompleted.Should().BeNull();
            action.Status.Should().Be(ActionStatus.Created);
            action.GetActionName().Should().Be("void-action");
            
            action.ExecuteAsync(Substitute.For<Thing>(), _provider);
            
            action.TimeCompleted.Should().NotBeNull();
            action.Status.Should().Be(ActionStatus.Completed);

            action.Logs.Should().NotBeEmpty();
            action.Logs.Should().HaveCount(1);
            action.Logs.Should().BeEquivalentTo(new List<string>
            {
                nameof(VoidThingActionInformation)
            });
        }
        
        [Fact]
        public void ExecuteWithThrow()
        {
            var action = new VoidThingActionInformation();
            
            action.GetId().Should().NotBeEmpty();
            action.TimeCompleted.Should().BeNull();
            action.Status.Should().Be(ActionStatus.Created);
            action.GetActionName().Should().Be("void-action");
            
            action.ExecuteAsync(Substitute.For<Thing>(), _provider);
            
            action.TimeCompleted.Should().NotBeNull();
            action.Status.Should().Be(ActionStatus.Completed);

            action.Logs.Should().NotBeEmpty();
            action.Logs.Should().HaveCount(1);
            action.Logs.Should().BeEquivalentTo(new List<string>
            {
                nameof(VoidThingActionInformation)
            });
        }
        
        [Fact]
        public async Task ExecuteAsync()
        {
            var action = new LongRunningThingActionInformation();
            
            action.GetId().Should().NotBeEmpty();
            action.TimeCompleted.Should().BeNull();
            action.Status.Should().Be(ActionStatus.Created);
            action.GetActionName().Should().Be("long-running-action");
            
            var task = action.ExecuteAsync(Substitute.For<Thing>(), _provider);
            
            action.Logs.Should().BeEmpty();
            
            action.TimeCompleted.Should().BeNull();
            action.Status.Should().Be(ActionStatus.Pending);

            await task;
            
            action.TimeCompleted.Should().NotBeNull();
            action.Status.Should().Be(ActionStatus.Completed);

            action.Logs.Should().NotBeEmpty();
            action.Logs.Should().HaveCount(1);
            action.Logs.Should().BeEquivalentTo(new List<string>
            {
                nameof(LongRunningThingActionInformation)
            });
        }
        
        [Fact]
        public async Task Cancel()
        {
            var action = new LongRunningThingActionInformation();
            
            action.GetId().Should().NotBeEmpty();
            action.TimeCompleted.Should().BeNull();
            action.Status.Should().Be(ActionStatus.Created);
            action.GetActionName().Should().Be("long-running-action");
            
            var task = action.ExecuteAsync(Substitute.For<Thing>(), _provider);
            
            action.Logs.Should().BeEmpty();
            
            action.TimeCompleted.Should().BeNull();
            action.Status.Should().Be(ActionStatus.Pending);
            
            action.Cancel();

            await Task.Delay(200);
            action.TimeCompleted.Should().NotBeNull();
            action.Status.Should().Be(ActionStatus.Completed);

            action.Logs.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StatusChange()
        {
            var counter = 0;
            var action = new VoidThingActionInformation();
            
            action.GetId().Should().NotBeEmpty();
            action.TimeCompleted.Should().BeNull();
            action.Status.Should().Be(ActionStatus.Created);
            action.GetActionName().Should().Be("void-action");

            action.StatusChanged += OnStatusChange;
            
            await action.ExecuteAsync(Substitute.For<Thing>(), _provider);
            
            action.TimeCompleted.Should().NotBeNull();
            action.Status.Should().Be(ActionStatus.Completed);

            action.Logs.Should().NotBeEmpty();
            action.Logs.Should().HaveCount(1);
            action.Logs.Should().BeEquivalentTo(new List<string>
            {
                nameof(VoidThingActionInformation)
            });

            void OnStatusChange(object sender, EventArgs args)
            {
                ((ThingActionInformation)sender).Status.Should().Be((ActionStatus)counter++);
            }
        }

        public class VoidThingActionInformation : ThingActionInformation
        {
            public List<string> Logs { get; } = new List<string>();
            protected override ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider)
            {
                Logs.Add(nameof(VoidThingActionInformation));
                return new ValueTask();
            }

            public override string GetActionName()
                => "void-action";
        }
        
        public class LongRunningThingActionInformation : ThingActionInformation
        {
            public List<string> Logs { get; } = new List<string>();
            protected override async ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider)
            {
                await Task.Delay(3_000, Source.Token);
                Logs.Add(nameof(LongRunningThingActionInformation));
            }

            public override string GetActionName()
                => "long-running-action";
        }
        
        public class ExceptionThingActionInformation : ThingActionInformation
        {
            protected override ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider)
            {
                throw new NotImplementedException();
            }

            public override string GetActionName()
                => "Exception-Action";
        }
    }
}
