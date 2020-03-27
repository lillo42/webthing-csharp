using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Properties;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class EventBuilderTest
    {
        private readonly EventBuilder _builder;
        private readonly EventThing _thing;
        private readonly Fixture _fixture;
        
        public EventBuilderTest()
        {
            _builder = new EventBuilder();
            _thing = new EventThing();
            _fixture = new Fixture();
        }

        [Fact]
        public void TryAddWhenSetThingTypeIsNotCalled() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Add(Substitute.For<EventInfo>(), null));

        [Fact]
        public void TryAddWhenSetThingOptionIsNotCalled()
        {
            _builder.SetThingType(_thing.GetType());
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<EventInfo>(), null));
        }
        
        [Fact]
        public void TryBuildWhenIsNotSetSetThing() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Build());

        [Fact]
        public void TryBuildWhenIsNotSetThingType()
        {
            _builder.SetThing(_thing);
            Assert.Throws<InvalidOperationException>(() => _builder.Build());
        }

        [Fact]
        public void BuildEventsAndInvokeInt()
        {
            _builder
                .SetThing(_thing)
                .SetThingType(_thing.GetType())
                .SetThingOption(new ThingOption());
            
            Visit();

            var events = _builder.Build();
            events.Should().NotBeEmpty();
            events.Should().HaveCount(2);
            events.Should().ContainKey(nameof(EventThing.Int));

            _thing.ThingContext = new ThingContext(
                new Dictionary<string, object>(), 
                events,
                new Dictionary<string, ActionCollection>(),
                new Dictionary<string, IProperty>());
            
            var value = _fixture.Create<int>();
            _thing.Invoke(value);
            

            var array = events[nameof(EventThing.Int)].ToArray();
            array.Should().NotBeEmpty();
            array.Should().HaveCount(1);
            array[0].Data.Should().Be(value);
        }
        
        [Fact]
        public void BuildEventsWithCustomNameAndInvokeInt()
        {
            _builder
                .SetThing(_thing)
                .SetThingType(_thing.GetType())
                .SetThingOption(new ThingOption());
            
            Visit();

            var events = _builder.Build();
            events.Should().NotBeEmpty();
            events.Should().HaveCount(2);
            events.Should().ContainKey("test");

            _thing.ThingContext = new ThingContext(
                new Dictionary<string, object>(), 
                events,
                new Dictionary<string, ActionCollection>(),
                new Dictionary<string, IProperty>());
            
            var value = _fixture.Create<string>();
            _thing.Invoke(value);

            var array = events["test"].ToArray();
            array.Should().NotBeEmpty();
            array.Should().HaveCount(1);
            array[0].Data.Should().Be(value);
        }
        
        private void Visit()
        {
            var events = _thing.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance);

            foreach (var @event in events)
            {
                var args = @event.EventHandlerType!.GetGenericArguments();
                if (args.Length > 1)
                {
                    continue;
                }

                if ((args.Length == 0 && @event.EventHandlerType != typeof(EventHandler))
                    || (args.Length == 1 && @event.EventHandlerType != typeof(EventHandler<>).MakeGenericType(args[0])))
                {
                    continue;
                }
                
                _builder.Add(@event, @event.GetCustomAttribute<ThingEventAttribute>());
            }
        }
        
        public class EventThing : Thing
        {
            public override string Name => "event-thing";

            public event EventHandler<int> Int;
            
            [ThingEvent(Name = "test")]
            public event EventHandler<string> String;

            public void Invoke(int value)
                => Int?.Invoke(this, value);

            public void Invoke(string value)
                => String?.Invoke(this, value);
        }
    }
}
