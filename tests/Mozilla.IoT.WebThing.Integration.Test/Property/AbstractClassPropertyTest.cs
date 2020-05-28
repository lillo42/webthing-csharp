using System.Reflection;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Property
{
    public abstract class AbstractClassPropertyTest<T> : AbstractPropertyTest<T>
    {
        [Fact]
        public void ValidProperty()
        {
            var thing = new PropertyThing();
            var context = Factory.Create(thing, new ThingOption());
        
            thing.ThingContext = context;
        
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(1);
            context.Properties.Should().ContainKey(nameof(PropertyThing.Value));

            var value = Fixture.Create<T>();
            var jsonElement = CreateJson(value);
        
            context.Properties[nameof(PropertyThing.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().Be(value);
            context.Properties[nameof(PropertyThing.Value)].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().Be(value);
        
            context.Properties[nameof(PropertyThing.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().Be(value);
            context.Properties[nameof(PropertyThing.Value)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().Be(value);
        
            jsonElement =  JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
            context.Properties[nameof(PropertyThing.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().BeNull();
            context.Properties[nameof(PropertyThing.Value)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().BeNull();
        }
        
        [Fact]
        public void Serialize()
        {
            var type = typeof(T).ToJsonType().ToString().ToLower();
            TestResponseProperty<PropertyThing>(string.Format(RESPONSE, type,
                typeof(T).IsEnum
                    ? $@" ""enum"": [""{string.Join(@""" , """, typeof(T).GetEnumNames())}""] "
                    : string.Empty));
        }
        
        [Fact]
        public void InvalidValidProperty()
        {
            var thing = new PropertyThing();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().ContainKey(nameof(PropertyThing.Value));

            var value = Fixture.Create<T>();
            var jsonElement = CreateInvalidJson();
            
            var defaultValue = Fixture.Create<T>();
            thing.Value = defaultValue;
            foreach (var element in jsonElement)
            {
                context.Properties[nameof(PropertyThing.Value)].TrySetValue(element).Should().Be(SetPropertyResult.InvalidValue);
                thing.Value.Should().NotBe(value);
                thing.Value.Should().Be(defaultValue);
                context.Properties[nameof(PropertyThing.Value)].TryGetValue(out var getValue).Should().BeTrue();
                getValue.Should().Be(defaultValue);
            }
        }
        
        public class PropertyThing : Thing
        {
            public override string Name => "property-thing";
            
            public T Value { get; set; }
        }
        
        private const string RESPONSE = @"{{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {{
        ""nosec_sc"": {{
            ""scheme"": ""nosec""
        }}
    }},
    ""properties"": {{
        ""value"": {{
            ""type"": ""{0}"",
            {1}
            ""links"": [
            {{
                ""href"": ""/things/property-thing/properties/value"",
                ""rel"": ""property""
            }}
        ]
        }}
    }},
    ""links"": [
    {{
        ""rel"": ""properties"",
        ""href"": ""/things/property-thing/properties""
    }},
    {{
        ""rel"": ""actions"",
        ""href"": ""/things/property-thing/actions""
    }},
    {{
        ""rel"": ""events"",
        ""href"": ""/things/property-thing/events""
    }}
  ]
}}";
        
    }
}
