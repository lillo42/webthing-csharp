using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Events
{
    public abstract class AbstractStructEventTest<T> : AbstractEventTest
        where T : struct
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
            context.Events.Should().HaveCount(2);

            context.Events.Should().ContainKeys(nameof(EventThing.Event), nameof(EventThing.NullableEvent));
            
            var @eventCounter = 0;
            var value = Fixture.Create<T>();
            
            var @nullableEventCounter = 0;
            var nullableValue = Fixture.Create<T?>();

            thing.Event += OnEvent;
            thing.NullableEvent += OnNullableEvent;
            
            thing.Invoke(value);
            
            thing.Invoke(nullableValue);
            thing.Invoke(null);


            eventCounter.Should().Be(1);
            nullableEventCounter.Should().Be(2);

            context.Events[nameof(EventThing.Event)].Count.Should().Be(eventCounter);
            context.Events[nameof(EventThing.NullableEvent)].Count.Should().Be(nullableEventCounter);

            context.Events[nameof(EventThing.Event)].TryDequeue(out var @event).Should().BeTrue();
            @event.Data.Should().Be(value);
            context.Events[nameof(EventThing.Event)].TryDequeue(out @event).Should().BeFalse();
            
            
            context.Events[nameof(EventThing.NullableEvent)].TryDequeue(out @event).Should().BeTrue();
            @event.Data.Should().Be(nullableValue);
            context.Events[nameof(EventThing.NullableEvent)].TryDequeue(out @event).Should().BeTrue();
            @event.Data.Should().BeNull();
            context.Events[nameof(EventThing.NullableEvent)].TryDequeue(out @event).Should().BeFalse();
            
            void OnEvent(object sender, T e)
            {
                sender.Should().NotBeNull();
                sender.Should().Be(thing);
                e.Should().Be(value);
                eventCounter++;
            }

            void OnNullableEvent(object sender, T? e)
            {
                sender.Should().NotBeNull();
                sender.Should().Be(thing);

                e?.Should().Be(nullableValue);

                nullableEventCounter++;
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
            context.Events.Should().HaveCount(2);

            context.Events.Should().ContainKeys(nameof(EventThing.Event), nameof(EventThing.NullableEvent));
             
            var values = new List<T>(3);
            var nullableValues = new List<T?>(3);

            for (var i = 0; i < 3; i++)
            {
                values.Add(Fixture.Create<T>());
                nullableValues.Add(Fixture.Create<T?>());
                
                thing.Invoke(values[i]);
                thing.Invoke(nullableValues[i]);
            }

            context.Events[nameof(EventThing.Event)].Count.Should().Be(2);
            context.Events[nameof(EventThing.NullableEvent)].Count.Should().Be(2);

            context.Events[nameof(EventThing.Event)].TryDequeue(out var value).Should().BeTrue();
            value.Data.Should().Be(values[1]);
            context.Events[nameof(EventThing.Event)].TryDequeue(out value).Should().BeTrue();
            value.Data.Should().Be(values[2]);
            context.Events[nameof(EventThing.Event)].TryDequeue(out value).Should().BeFalse();

            context.Events[nameof(EventThing.NullableEvent)].TryDequeue(out value).Should().BeTrue();
            value.Data.Should().Be(nullableValues[1]);
            context.Events[nameof(EventThing.NullableEvent)].TryDequeue(out value).Should().BeTrue();
            value.Data.Should().Be(nullableValues[2]);
            context.Events[nameof(EventThing.NullableEvent)].TryDequeue(out value).Should().BeFalse();
        }

        public class EventThing : Thing
        {
            public override string Name => "event-thing";

            public event EventHandler<T> Event;
            public event EventHandler<T?> NullableEvent;

            [ThingAction(Ignore = true)]
            public void Invoke(T value)
            {
                try
                {
                    Event?.Invoke(this, value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            [ThingAction(Ignore = true)]
            public void Invoke(T? value)
                => NullableEvent?.Invoke(this, value);
        }
    }
}
