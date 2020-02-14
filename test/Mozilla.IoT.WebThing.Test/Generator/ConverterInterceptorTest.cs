using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions.Json;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Converter;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator
{
    public class ConverterInterceptorTest
    {
        private readonly Fixture _fixture;
        private readonly LampThing _thing;
        private readonly ConverterInterceptorFactory _factory;

        public ConverterInterceptorTest()
        {
            _fixture = new Fixture();
            _thing = new LampThing();
            _factory = new ConverterInterceptorFactory(_thing, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            });
        }

        [Fact]
        public void Serialize()
        {
            CodeGeneratorFactory.Generate(_thing, new[] {_factory});
            _thing.Prefix = new Uri("http://localhost/");
            _thing.ThingContext = new Context(_factory.Create(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                new Dictionary<string, ActionContext>());
            
            var value = JsonSerializer.Serialize(_thing,
                new JsonSerializerOptions {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true,
                    Converters = { new ThingConverter() }
                });

            JToken.Parse(value)
                .Should()
                .BeEquivalentTo(JToken.Parse(@"
{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""id"": ""http://localhost/things/lamp"",
    ""title"": ""My Lamp"",
    ""description"": ""A web connected lamp"",
    ""@type"": [
        ""Light"",
        ""OnOffSwitch""
    ],
    ""properties"": {
        ""on"": {
            ""title"": ""On/Off"",
            ""description"": ""Whether the lamp is turned on"",
            ""readOnly"": false,
            ""@type"": ""OnOffProperty"",
            ""readOnly"": false,
            ""type"": ""boolean"",
            ""links"": [
                {
                    ""href"": ""/things/lamp/properties/on""
                }
            ]
        },
        ""brightness"": {
            ""title"": ""Brightness"",
            ""description"": ""The level of light from 0-100"",
            ""readOnly"": false,
            ""@type"": ""BrightnessProperty"",
            ""minimum"": 0,
            ""maximum"": 100,
            ""readOnly"": false,
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/lamp/properties/brightness""
                }
            ]
        }
    },
    ""actions"": {
        ""fade"": {
            ""title"": ""Fade"",
            ""description"": ""Fade the lamp to a given level"",
            ""@type"": ""FadeAction"",
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
                        ""unit"": ""milliseconds"",
                        ""minimum"": 0
                    }
                }
            },
            ""links"": [
                {
                    ""href"": ""/things/lamp/actions/fade""
                }
            ]
        }
    },
    ""events"": {
        ""overheated"": {
            ""title"": ""Overheated"",
            ""description"": ""The lamp has exceeded its safe operating temperature"",
            ""@type"": ""OverheatedEvent"",
            ""type"": ""number"",
            ""links"": [
                {
                    ""href"": ""/things/lamp/events/overheated""
                }
            ]
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
            ""href"": ""ws://localhost/things/lamp""
        }
    ]
}
"));
        }
        public class LampThing : Thing
        {
            public override string Name => "Lamp";
            public override string? Title => "My Lamp";
            public override string? Description => "A web connected lamp";
            public override string[]? Type { get; } = new[] { "Light", "OnOffSwitch" };

            [ThingProperty(Ignore = true)]
            public object Ignore { get; set; }
            
            [ThingProperty(Type = new []{ "OnOffProperty" }, Title = "On/Off", Description = "Whether the lamp is turned on")]
            public bool On { get; set; }
        
            [ThingProperty(Type = new []{ "BrightnessProperty" },Title = "Brightness",
                Description = "The level of light from 0-100", Minimum = 0, Maximum = 100)]
            public int Brightness { get; set; }

            [ThingEvent(Ignore = true)]
            public event EventHandler<double> OnIgnore;
            
            [ThingEvent(Title = "Overheated", 
                Type = new [] {"OverheatedEvent"},
                Description = "The lamp has exceeded its safe operating temperature")]
            public event EventHandler<double> Overheated;


            [ThingAction(Name = "fade", Title = "Fade", Type = new []{"FadeAction"},
                Description = "Fade the lamp to a given level")]
            public void Fade(
                [ThingParameter(Minimum = 0, Maximum = 100)]int level,
                [ThingParameter(Minimum = 0, Unit = "milliseconds")]int duration)
            {
                Console.WriteLine("Fade executed....");
            }
            
            [ThingAction(Ignore = true)]
            public void IgnoreA(
                [ThingParameter(Minimum = 0, Maximum = 100)]int level,
                [ThingParameter(Minimum = 0, Unit = "milliseconds")]int duration)
            {
                Console.WriteLine("Fade executed....");
            }
        }
    }
}
