using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Intregration.Test.Factories
{
    public class EventTest : IThingContextFactoryTest
    {
        [Fact]
        public void CreateWithEventThing()
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
            context.Events.Should().NotContainKey(nameof(EventThing.Ignore));
            context.Events.Should().NotContainKey(nameof(EventThing.Ignore2));
            context.Events.Should().NotContainKey(nameof(EventThing.Ignore3));
            context.Events.Should().ContainKey(nameof(EventThing.Int));
            context.Events[nameof(EventThing.Int)].Should().NotBeNull();
            context.Events[nameof(EventThing.Int)].ToArray().Should().BeEmpty();
            context.Events.Should().ContainKey("other");
            context.Events["other"].Should().NotBeNull();
            context.Events["other"].ToArray().Should().BeEmpty();
            
            var @int = Fixture.Create<int>();
            thing.Invoke(@int);

            var array = context.Events[nameof(EventThing.Int)].ToArray();
            array.Should().NotBeEmpty();
            array.Should().HaveCount(1);
            array[0].Data.Should().Be(@int);
            
            var @string = Fixture.Create<string>();
            thing.Invoke(@string);

            array = context.Events["other"].ToArray();
            array.Should().NotBeEmpty();
            array.Should().HaveCount(1);
            array[0].Data.Should().Be(@string);
            
            @int = Fixture.Create<int>();
            thing.Invoke(@int);
            var @int2 = Fixture.Create<int>();
            thing.Invoke(@int2);

            array = context.Events[nameof(EventThing.Int)].ToArray();
            array.Should().NotBeEmpty();
            array.Should().HaveCount(2);
            array[0].Data.Should().Be(@int);
            array[1].Data.Should().Be(@int2);

            var message = JsonSerializer.Serialize(context.Response,
                new ThingOption().ToJsonSerializerOptions());

            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(@"
{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""events"": {
    ""int"": {
      ""link"": [
        {
          ""href"": ""/things/event-thing/events/int"",
          ""rel"": ""event""
        }
      ]
    },
    ""other"": {
      ""title"": ""Title other"",
      ""description"": ""Other Description"",
      ""unit"": ""Something"",
      ""link"": [
        {
          ""href"": ""/things/event-thing/events/other"",
          ""rel"": ""event""
        }
      ]
    }
  },
  ""links"": [
    {
      ""href"": ""properties"",
      ""rel"": ""/things/event-thing/properties""
    },
    {
      ""href"": ""events"",
      ""rel"": ""/things/event-thing/events""
    },
    {
      ""href"": ""actions"",
      ""rel"": ""/things/event-thing/actions""
    }
  ]
}
"));
        }
        
        public class EventThing : Thing
        {
            public delegate void Info();
            public override string Name => "event-thing";

            [ThingEvent(Ignore = true)]
            public event EventHandler<int> Ignore;

            public event Info Ignore2;
            
            internal event EventHandler<int> Ignore3;

            public event EventHandler<int> Int;
            
            [ThingEvent(Name = "Other", Title = "Title other", Description = "Other Description", Unit = "Something")]
            public event EventHandler<string> Something;
            
            internal void Invoke(int value)
                => Int?.Invoke(this, value);
            
            internal void Invoke(string value)
                => Something?.Invoke(this, value);
        }
    }
}
