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
    public class ComplexThingConvertTest
    {
        private readonly Fixture _fixture;
        private readonly ThingConverterFactory _generate;
        private readonly JsonSerializerOptions _option;

        public ComplexThingConvertTest()
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
        public void ThingWithProperty_Convert()
        {
            var id = _fixture.Create<string>();
            var thing = new ThingWithProperty();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithProperty"] = _generate.Create(thing, _option)
                });

            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing, _option);

            var actual = JToken.Parse(result);

            actual.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""{id}ThingWithProperty"",
                ""properties"": {{
                    ""text"": {{
                        ""readOnly"": false,
                        ""type"": ""string"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Text""
                        }}]
                    }},
                    ""isValid"": {{
                        ""readOnly"": false,
                        ""type"": ""boolean"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/IsValid""
                        }}]
                    }},
                    ""number"": {{
                        ""readOnly"": false,
                        ""type"": ""integer"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Number""
                        }}]
                    }},
                    ""percent"": {{
                        ""readOnly"": true,
                        ""type"": ""number"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Percent""
                        }}]
                    }}
                }}
            }}"));
        }
        
        
        [Fact]
        public void ThingWithAttribute_Convert()
        {
            var id = _fixture.Create<string>();
            var thing = new ThingWithPropertiesAttribute();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithAttribute"] = _generate.Create(thing, _option)
                });

            _option.Converters.Clear();
            _option.Converters.Add(convert);
            
            var result = JsonSerializer.Serialize(thing,
                new JsonSerializerOptions {IgnoreNullValues = true, WriteIndented = true, Converters = {convert}});

            var actual = JToken.Parse(result);

            actual.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""{id}ThingWithAttribute"",
                ""properties"": {{
                    ""test"": {{
                        ""readOnly"": false,
                        ""type"": ""number"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/test""
                        }}]
                    }},
                    ""foo"": {{
                        ""readOnly"": false,
                        ""title"": ""Some Title"",
                        ""description"": ""Some description"",
                        ""@type"": ""Light"",
                        ""@enum"": [ ""Test1"", ""Test2""],
                        ""type"": ""string"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/foo""
                        }}]
                    }},
                    ""bar"": {{
                        ""@type"": [ ""Light"", ""OnOffSwitch""],
                        ""readOnly"": true,
                        ""type"": ""boolean"",
                        ""links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/bar""
                        }}]
                    }}
                }}
            }}"));
        }

    public class ThingWithProperty : Thing
    {
        public override string Name => "ThingWithProperty";

        public string Text { get; set; }
        public bool IsValid { get; set; }
        public double Percent => 0.5;
        public int Number { get; set; }
    }

    public class ThingWithPropertiesAttribute : Thing
    {
        public override string Name => "ThingWithAttribute";

        [ThingProperty(Ignore = true)] 
        public string Information { get; set; }

        [ThingProperty(Name = "Test")] 
        public double Lights { get; set; }

        [ThingProperty(Title = "Some Title", Description = "Some description", Type = new[] { "Light"}, Enum = new object[]{ "Test1", "Test2"})]
        public string Foo { get; set; }

        [ThingProperty(IsReadOnly = true, Type = new[]{"Light", "OnOffSwitch"})]
        public bool Bar { get; set; }
    }
    }
}
