using System;
using System.Reflection;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class EventBuilderTest
    {
        private readonly EventBuilder _builder;
        private readonly EventThing _thing;

        public EventBuilderTest()
        {
            _builder = new EventBuilder();
            _thing = new EventThing();
        }

        #region Add

        [Fact]
        public void Add_Should_Throw_When_SetThingTypeIsNotCalled() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Add(Substitute.For<EventInfo>(), null));
        
        [Fact]
        public void Add_Should_Throw_When_SetThingOptionIsNotCalled()
        {
            _builder.SetThingType(_thing.GetType());
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<EventInfo>(), null));
        }

        #endregion

        #region Build

        [Fact]
        public void Build_Should_Throw_When_IsNotSetSetThing() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Build());

        [Fact]
        public void Build_Should_Throw_When_IsNotSetThingType()
        {
            _builder.SetThing(_thing);
            Assert.Throws<InvalidOperationException>(() => _builder.Build());
        }
        
        [Fact]
        public void Build()
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
        }
        
        #endregion

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
