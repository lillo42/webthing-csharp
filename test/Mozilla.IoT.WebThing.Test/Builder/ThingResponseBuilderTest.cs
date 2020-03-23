using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class ThingResponseBuilderTest
    {
        private readonly EventThing _eventThing;
        private readonly ThingResponseBuilder _builder;

        public ThingResponseBuilderTest()
        {
            _builder = new ThingResponseBuilder();
            _eventThing = new EventThing();
        }
        
        [Fact]
        public void TryAddWhenSetThingIsNotCalled() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Add(Substitute.For<EventInfo>(), null));

        
        [Fact]
        public void TryAddWhenSetThingTypeIsNotCalled()
        {
            _builder.SetThing(_eventThing);
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<EventInfo>(), null));
        }
        
        [Fact]
        public void TryAddWhenSetThingOptionIsNotCalled()
        {
            _builder
                .SetThing(_eventThing)
                .SetThingType(_eventThing.GetType());
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<EventInfo>(), null));
        }
        
        [Fact]
        public void TryBuildWhenSetThingIsNotCalled() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Build());
        
        [Fact]
        public void TryBuildWhenSetThingTypeIsNotCalled()
        {
            _builder.SetThing(_eventThing);
            Assert.Throws<InvalidOperationException>(() => _builder.Build());
        }

        [Fact]
        public void BuildWithEvent()
        {
            _builder
                .SetThing(_eventThing)
                .SetThingOption(new ThingOption())
                .SetThingType(_eventThing.GetType());
            
            Visit(_eventThing.GetType());
            
            var response = _builder.Build();
            
            response.Should().NotBeNull();
            
            JToken.Parse(JsonConvert.SerializeObject(response))
                .Should().BeEquivalentTo(JToken.Parse(@"
{
    ""Events"": {
        ""Int"": {
            ""Link"": [
                {
                    ""Href"": ""/thing/event-thing/events/int"",
                    ""Rel"": ""event""
                }
            ]
        },
        ""String"": {
            ""Title"": ""Bar"",
            ""Description"": ""Foo"",
            ""Unit"": ""<type>"",
            ""Type"": null,
            ""Link"": [
                {
                    ""Href"": ""/thing/event-thing/events/string"",
                    ""Rel"": ""event""
                }
            ]
        }
    },
    ""Id"": null,
    ""Context"": ""https://iot.mozilla.org/schemas"",
    ""Title"": null,
    ""Description"": null,
    ""Links"": []
}"));
        }


        private void Visit(Type thingType)
        {
            var events = thingType.GetEvents(BindingFlags.Public | BindingFlags.Instance);

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
            
            [ThingEvent(Name = "test", Description = "Foo", Title = "Bar", Unit = "<type>")]
            public event EventHandler<string> String;
        }
    }
}
