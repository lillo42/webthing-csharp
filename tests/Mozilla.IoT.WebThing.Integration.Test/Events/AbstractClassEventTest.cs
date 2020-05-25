using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Events
{
    public abstract class AbstractClassEventTest<T> : AbstractEventTest
        where T : class
    {
        [Fact]
        public void EmitEvent()
        {
            var thing = new EventThing();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;

            context.Actions.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Events.Should().NotBeEmpty();
            context.Events.Should().HaveCount(1);

            context.Events.Should().ContainKeys(nameof(EventThing.Event));
            
            var @eventCounter = 0;
            var value = Fixture.Create<T>();

            thing.Event += OnEvent;
            
            thing.Invoke(value);
            thing.Invoke(null);
            
            eventCounter.Should().Be(2);

            context.Events[nameof(EventThing.Event)].Count.Should().Be(eventCounter);

            context.Events[nameof(EventThing.Event)].TryDequeue(out var @event).Should().BeTrue();
            @event.Data.Should().Be(value);
            context.Events[nameof(EventThing.Event)].TryDequeue(out  @event).Should().BeTrue();
            @event.Data.Should().Be(null);
            context.Events[nameof(EventThing.Event)].TryDequeue(out @event).Should().BeFalse();
            
            void OnEvent(object sender, T e)
            {
                sender.Should().NotBeNull();
                sender.Should().Be(thing);
                e?.Should().Be(value);
                eventCounter++;
            }
        }
        
         [Fact]
        public void MaxSize()
        {
            var thing = new EventThing();
            var context = Factory.Create(thing, new ThingOption
            {
                MaxEventSize = 2
            });
            
            thing.ThingContext = context;

            context.Actions.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Events.Should().NotBeEmpty();
            context.Events.Should().HaveCount(1);

            context.Events.Should().ContainKeys(nameof(EventThing.Event));
             
            var values = new List<T>(3);

            for (var i = 0; i < 3; i++)
            {
                values.Add(Fixture.Create<T>());
                thing.Invoke(values[i]);
            }

            context.Events[nameof(EventThing.Event)].Count.Should().Be(2);

            context.Events[nameof(EventThing.Event)].TryDequeue(out var value).Should().BeTrue();
            value.Data.Should().Be(values[1]);
            context.Events[nameof(EventThing.Event)].TryDequeue(out value).Should().BeTrue();
            value.Data.Should().Be(values[2]);
            context.Events[nameof(EventThing.Event)].TryDequeue(out value).Should().BeFalse();
        }

        public class EventThing : Thing
        {
            public override string Name => "event-thing";

            public event EventHandler<T> Event;

            [ThingAction(Ignore = true)]
            public void Invoke(T value)
                => Event?.Invoke(this, value);
        }
    }
}
