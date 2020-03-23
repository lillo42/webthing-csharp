using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Factory
{
    public class ThingContextFactoryTest
    {
        private readonly ThingContextFactory _factory;
        private readonly IEventBuilder _event;
        
        public ThingContextFactoryTest()
        {
            _event = Substitute.For<IEventBuilder>();
            _factory = new ThingContextFactory(_event);
        }

        [Fact]
        public void CreateWithEvent()
        {
            var thing = new EventThing();
            var option = new ThingOption();
            
            _event
                .SetThing(thing)
                .Returns(_event);
            
            _event
                .SetThingOption(option)
                .Returns(_event);
            
            _event
                .SetThingType(thing.GetType())
                .Returns(_event);

            _event
                .Build()
                .Returns(new Dictionary<string, EventCollection>());

            
            var context = _factory.Create(thing, option);

            context.Should().NotBeNull();

            _event
                .Received(1)
                .SetThing(thing);
            
            _event
                .Received(1)
                .SetThingOption(option);
            
            _event
                .Received(1)
                .SetThingType(thing.GetType());
            
            _event
                .Received(1)
                .Build();
            
            _event
                .Received(1)
                .Add(Arg.Any<EventInfo>(), Arg.Any<ThingEventAttribute>());
        }
        
        public class EventThing : Thing
        {
            public delegate void NotValidHandler(object? sender);
            public override string Name => "event-thing";

            public event EventHandler<int> Int;
            
            [ThingEvent(Ignore = true)]
            public event EventHandler<string> Ignore;

            
            public event NotValidHandler NotValid;
        }
    }
    
}
