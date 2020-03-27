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
        private readonly IThingResponseBuilder _response;
        private readonly IEventBuilder _event;
        private readonly IPropertyBuilder _property;
        
        public ThingContextFactoryTest()
        {
            _response = Substitute.For<IThingResponseBuilder>();
            _event = Substitute.For<IEventBuilder>();
            _property = Substitute.For<IPropertyBuilder>();
            
            _factory = new ThingContextFactory(_event, _property, _response);
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
            
            _property
                .DidNotReceive()
                .Add(Arg.Any<PropertyInfo>(), Arg.Any<Information>());
        }
        
        [Fact]
        public void CreateWithProperty()
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
            
            _property
                .DidNotReceive()
                .Add(Arg.Any<PropertyInfo>(), Arg.Any<Information>());
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
        
        public class PropertyThing : Thing
        {
            public override string Name => "property-thing";
            
            [ThingProperty(Name = "bool2",
                Title = "Boo Title",
                Description = "Bool test")]
            public bool Bool { get; set; }
            
            [ThingProperty(Name = "Guid2", IsReadOnly = true)]
            public Guid Guid { get; set; }
        }
    }
    
}
