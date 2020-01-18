using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Converts;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Converts
{
    public class SimpleThingConverterTest
    {
        private readonly Fixture _fixture;

        public SimpleThingConverterTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void ThingWithoutProperties()
        {
            var id = _fixture.Create<string>();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter> {["ThingWithoutProperties"] = new EmptyConvert()});

            var result = JsonSerializer.Serialize(new ThingWithoutProperties(),
                new JsonSerializerOptions {IgnoreNullValues = true, WriteIndented = true, Converters = {convert}});

            var token = JToken.Parse(result);

            token.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""@type"": [
                    ""Light"",
                    ""OnOffSwitch""
                ],
                ""Id"": ""{id}ThingWithoutProperties"",
                ""Title"": ""Custom Title"",
                ""Description"": ""Custom Description""
            }}"));
        }
        
        [Fact]
        public void ThingWithoutPropertiesAndSingleType()
        {
            var id = _fixture.Create<string>();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter> {["ThingWithoutPropertiesSingleType"] = new EmptyConvert()});

            var result = JsonSerializer.Serialize(new ThingWithoutPropertiesSingleType(),
                new JsonSerializerOptions {IgnoreNullValues = true, WriteIndented = true, Converters = {convert}});

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""@type"": ""Light"",
                ""Id"": ""{id}ThingWithoutPropertiesSingleType"",
                ""Title"": ""Custom Title"",
                ""Description"": ""Custom Description""
            }}"));
        }
    }

    
    public class ThingWithoutProperties : Thing
    {
        public override string Name => "ThingWithoutProperties";
        public override string? Title => "Custom Title";
        public override string? Description => "Custom Description";
        public override string[]? Type { get; } = new[] { "Light", "OnOffSwitch" };
    }
    
    public class ThingWithoutPropertiesSingleType : Thing
    {
        public override string Name => "ThingWithoutPropertiesSingleType";
        public override string? Title => "Custom Title";
        public override string? Description => "Custom Description";
        public override string[]? Type { get; } = new[] { "Light" };
    }

    public class EmptyConvert : IThingConverter
    {
        
        public void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options)
        {
            
        }
    }
}
