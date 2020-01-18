using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Converts
{
    public class ComplexThingWithEventConvertTest
    {
        private readonly Fixture _fixture;
        private readonly ThingConverterFactory _generate;
        private readonly JsonSerializerOptions _option;

        public ComplexThingWithEventConvertTest()
        {
            _fixture = new Fixture();
            _generate = new ThingConverterFactory();
            _option = new JsonSerializerOptions 
            {
                IgnoreNullValues =  true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        
        [Fact]
        public void ThingWithEvent_Convert()
        {
            var id = _fixture.Create<string>();
            var thing = new ThingWithEvent();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithEvent"] =  _generate.Create(thing, _option)
                });
            
            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing, _option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""{id}ThingWithEvent"",
                ""events"": {{ 
                    ""overheated"": {{
                        ""type"": ""number"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithEvent/events/overheated""
                        }}]
                    }},
                    ""Something"": {{
                        ""links"": [{{
                            ""href"": ""/things/ThingWithEvent/events/something""
                        }}]
                    }}
                }}
            }}"));
        }
        
        [Fact]
        public void ThingWithEventAttribute_Convert()
        {
            var id = _fixture.Create<string>();
            var thing = new ThingWithEventAttribute();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithEventAttribute"] =  _generate.Create(thing, _option)
                });
            
            _option.Converters.Clear();
            _option.Converters.Add(convert);

            var result = JsonSerializer.Serialize(thing,_option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""{id}ThingWithEventAttribute"",
                ""events"": {{ 
                    ""test"": {{
                        ""title"": ""Overheated"",
                        ""description"": ""Title"",
                        ""@type"": ""The lamp has exceeded its safe operating temperature"",
                        ""unit"": ""degree celsius"",
                        ""type"": ""number"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithEvent/events/test""
                        }}]
                    }}
                }}
            }}"));
        }
        
        public class ThingWithEvent : Thing
        {
            public override string Name => "ThingWithEvent";

            public event EventHandler<double> Overheated;
            
            public event EventHandler Something;
        }
        
        public class ThingWithEventAttribute : Thing
        {
            public override string Name => "ThingWithEventAttribute";

            [ThingEventInformation(Name = "Test", Title = "Overheated", Description = "The lamp has exceeded its safe operating temperature", 
                Type = new []{ "OverheatedEvent" }, Unit = "degree celsius")]
            public event EventHandler<double> Overheated;
            
            [ThingEventInformation(Ignore = true)]
            public event EventHandler Ignore;
        }
    }
}
