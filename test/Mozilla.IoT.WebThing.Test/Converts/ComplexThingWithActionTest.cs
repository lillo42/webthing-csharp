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
    public class ComplexThingWithActionTest
    {
        private readonly Fixture _fixture;
        private readonly ThingConverterFactory _generate;
        private readonly JsonSerializerOptions _option;
        
        public ComplexThingWithActionTest()
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
            var thing = new ThingWithAction();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithAction"] =  _generate.Create(thing, _option)
                });
            
            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing, _option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""{id}ThingWithAction"",
                ""actions"": {{ 
                    ""fade"": {{
                        ""input"": {{
                            ""type"": ""object"",
                            ""properties"": {{
                                ""level"": {{ 
                                    ""type"": ""integer""
                                }},
                                ""duration"": {{ 
                                    ""type"": ""integer""
                                }}
                            }}
                        }},
                        ""links"": [{{
                            ""href"": ""/things/ThingWithAction/events/fade""
                        }}]
                    }}
                }}
            }}"));
        }
        
        [Fact]
        public void ThingWithActionAttribute_Convert()
        {
            var id = _fixture.Create<string>();
            var thing = new ThingWithActionAttribute();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithActionAttribute"] =  _generate.Create(thing, _option)
                });
            
            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing, _option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""{id}ThingWithActionAttribute"",
                ""actions"": {{ 
                    ""fade"": {{
                        ""@type"": ""FadeAction"",
                        ""title"": ""Fade"",
                        ""description"": ""Fade the lamp to a given level"",
                        ""input"": {{
                            ""type"": ""object"",
                            ""properties"": {{
                                ""level"": {{ 
                                    ""type"": ""integer"",
                                    ""minimum"": 0,
                                    ""maximum"": 100
                                }},
                                ""duration"": {{ 
                                    ""type"": ""integer"",
                                    ""minimum"": 0,
                                    ""unit"": ""milliseconds"",
                                }}
                            }}
                        }},
                        ""links"": [{{
                            ""href"": ""/things/ThingWithActionAttribute/events/fade""
                        }}]
                    }}
                }}
            }}"));
        }
        
        public class ThingWithAction : Thing
        {
            public override string Name => "ThingWithAction";

            public void Fade(int level, int duration)
            {
                
            }
        }

        public class ThingWithActionAttribute : Thing
        {
            public override string Name => "ThingWithActionAttribute";

            [ThingAction(Type = new[] {"FadeAction"}, Title = "Fade", Description = "Fade the lamp to a given level")]
            public void Fade(
                [ThingParameter(Minimum = 0, Maximum = 100)]
                int level,
                [ThingParameter(Minimum = 0, Unit = "milliseconds")]
                int duration)
            {

            }

            [ThingAction(Ignore = true)]
            public void Test()
            {
                
            }
        }
    }
}
