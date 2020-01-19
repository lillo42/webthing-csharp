using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Converts
{
    public class CompletedThingConvertTest
    {
        private readonly ThingConverterFactory _generate;
        private readonly JsonSerializerOptions _option;

        public CompletedThingConvertTest()
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
        public void Converter()
        {
            var thing = new LampThing
            {
                Prefix = new Uri("https://mywebthingserver.com/")
            };
            
            var convert = new ThingConverter(
                new Dictionary<string, IThingConverter>
                {
                    ["lamp"] =  _generate.Create(thing, _option)
                });
            
            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing, _option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse(@"{
              ""@context"": ""https://iot.mozilla.org/schemas"",
              ""@type"": [""Light"", ""OnOffSwitch""],
              ""id"": ""https://mywebthingserver.com/things/lamp"",
              ""title"":""My Lamp"",
              ""description"": ""A web connected lamp"",
              ""properties"": {
                ""on"": {
                  ""@type"": ""OnOffProperty"",
                  ""type"": ""boolean"",
                  ""title"": ""On/Off"",
                  ""description"": ""Whether the lamp is turned on"",
                  ""readOnly"": false,
                  ""links"": [{""href"": ""/things/lamp/properties/on""}]
                },
                ""brightness"" : {
                  ""@type"": ""BrightnessProperty"",
                  ""type"": ""integer"",
                  ""title"": ""Brightness"",
                  ""description"": ""The level of light from 0-100"",
                  ""minimum"" : 0,
                  ""maximum"" : 100,
                  ""readOnly"": false,
                  ""links"": [{""href"": ""/things/lamp/properties/brightness""}]
                }
              },
              ""actions"": {
                ""fade"": {
                  ""@type"": ""FadeAction"",
                  ""title"": ""Fade"",
                  ""description"": ""Fade the lamp to a given level"",
                  ""input"": {
                    ""type"": ""object"",
                    ""properties"": {
                      ""level"": {
                        ""type"": ""integer"",
                        ""minimum"": 0,
                        ""maximum"": 100
                      },
                      ""duration"": {
                        ""type"": ""integer"",
                        ""minimum"": 0,
                        ""unit"": ""milliseconds""
                      }
                    }
                  },
                  ""links"": [{""href"": ""/things/lamp/actions/fade""}]
                }
              },
              ""events"": {
                ""overheated"": {
                  ""title"": ""Overheated"",
                  ""@type"": ""OverheatedEvent"",
                  ""type"": ""number"",
                  ""unit"": ""degree celsius"",
                  ""description"": ""The lamp has exceeded its safe operating temperature"",
                  ""links"": [{""href"": ""/things/lamp/events/overheated""}]
                }
              },
              ""links"": [
                {
                  ""rel"": ""properties"",
                  ""href"": ""/things/lamp/properties""
                },
                {
                  ""rel"": ""actions"",
                  ""href"": ""/things/lamp/actions""
                },
                {
                  ""rel"": ""events"",
                  ""href"": ""/things/lamp/events""
                },
                {
                  ""rel"": ""alternate"",
                  ""href"": ""wss://mywebthingserver.com/things/lamp""
                }
              ]
        }"));
        }

        public class LampThing : Thing
        {
            public override string Name => "lamp";
            public override string? Title => "My Lamp";
            public override string? Description => "A web connected lamp";
            public override string[]? Type => new [] {"Light", "OnOffSwitch"};

            [ThingProperty(Type = new []{ "OnOffProperty" }, Title = "On/Off", 
                Description = "TWhether the lamp is turned on")]
            public bool On { get; set; }

            [ThingProperty(Type = new []{ "BrightnessProperty" },Title = "Brightness",
            Description = "The level of light from 0-100", Minimum = 0, Maximum = 100)]
            public int Brightness { get; set; }
            
            [ThingAction(Type = new[] {"FadeAction"}, Title = "Fade", Description = "Fade the lamp to a given level")]
            public void Fade(
                [ThingParameter(Minimum = 0, Maximum = 100)]
                int level,
                [ThingParameter(Minimum = 0, Unit = "milliseconds")]
                int duration)
            {

            }
            
            [ThingEvent(Type = new[] { "OverheatedEvent" },Title = "Overheated",Unit = "degree celsius",
                Description = "The lamp has exceeded its safe operating temperature")]
            public event EventHandler<double> Overheated;
        }
    }
}
