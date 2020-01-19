using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Converts
{
    public class SimpleThingConverterTest
    {
        private readonly ThingConverterFactory _generate;
        private readonly JsonSerializerOptions _option;
        public SimpleThingConverterTest()
        {
            _generate = new ThingConverterFactory();
            _option = new JsonSerializerOptions 
            {
                IgnoreNullValues =  true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        [Fact]
        public void ThingWithoutProperties_Converter()
        {
            var thing = new ThingWithoutProperties {Prefix = new Uri("https://mywebthingserver.com/")};
            var convert = new ThingConverter(
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithoutProperties"] = _generate.Create(thing, _option)
                });
            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing, _option);

            var token = JToken.Parse(result);

            token.Should().BeEquivalentTo(JToken.Parse(@"{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""@type"": [
                    ""Light"",
                    ""OnOffSwitch""
                ],
                ""Id"": ""https://mywebthingserver.com/things/ThingWithoutProperties"",
                ""Title"": ""Custom Title"",
                ""Description"": ""Custom Description"",
                ""links"": [{
                        ""rel"": ""properties"",
                        ""href"": ""/things/ThingWithoutProperties/properties""
                    }, {
                        ""rel"": ""actions"",
                        ""href"": ""/things/ThingWithoutProperties/actions""
                    },{
                        ""rel"": ""events"",
                        ""href"": ""/things/ThingWithoutProperties/events""
                    }, {
                        ""rel"": ""alternate"",
                        ""href"": ""wss://mywebthingserver.com/things/ThingWithoutProperties""
                }]
            }"));
        }
        
        [Fact]
        public void ThingWithoutPropertiesAndSingleType()
        {
            var thing = new ThingWithoutPropertiesSingleType {Prefix = new Uri("https://mywebthingserver.com/")};
            var convert = new ThingConverter(
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithoutPropertiesSingleType"] = _generate.Create(thing, _option)
                });
            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing,_option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse(@"{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""@type"": ""Light"",
                ""Id"": ""https://mywebthingserver.com/things/ThingWithoutPropertiesSingleType"",
                ""Title"": ""Custom Title"",
                ""Description"": ""Custom Description"",
                ""links"": [{
                        ""rel"": ""properties"",
                        ""href"": ""/things/ThingWithoutPropertiesSingleType/properties""
                    }, {
                        ""rel"": ""actions"",
                        ""href"": ""/things/ThingWithoutPropertiesSingleType/actions""
                    },{
                        ""rel"": ""events"",
                        ""href"": ""/things/ThingWithoutPropertiesSingleType/events""
                    }, {
                        ""rel"": ""alternate"",
                        ""href"": ""wss://mywebthingserver.com/things/ThingWithoutPropertiesSingleType""
                }]
            }"));
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
}
