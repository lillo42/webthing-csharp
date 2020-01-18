using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public ComplexThingConvertTest()
        {
            _fixture = new Fixture();
            _generate = new ThingConverterFactory();
        }

        [Fact]
        public void ThingWithProperty()
        {
            var id = _fixture.Create<string>();
            var thing = new ThingWithProperty();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithProperty"] = _generate.Create(thing)
                });

            var result = JsonSerializer.Serialize(thing,
                new JsonSerializerOptions {IgnoreNullValues = true, WriteIndented = true, Converters = {convert}});

            var actual = JToken.Parse(result);

            actual.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""@type"": null,
                ""Id"": ""{id}ThingWithProperty"",
                ""Title"": null,
                ""Description"": null,
                ""Properties"": {{
                    ""Text"": {{
                        ""ReadOnly"": false,
                        ""Type"": ""string"",
                        ""Links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Text""
                        }}]
                    }},
                    ""IsValid"": {{
                        ""ReadOnly"": false,
                        ""Type"": ""boolean"",
                        ""Links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/IsValid""
                        }}]
                    }},
                    ""Number"": {{
                        ""ReadOnly"": false,
                        ""Type"": ""integer"",
                        ""Links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Number""
                        }}]
                    }},
                    ""Percent"": {{
                        ""ReadOnly"": true,
                        ""Type"": ""number"",
                        ""Links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Percent""
                        }}]
                    }}
                }}
            }}"));
        }
        
        
        [Fact]
        public void ThingWithAttribute()
        {
            var id = _fixture.Create<string>();
            var thing = new ThingWithAttribute();
            var convert = new ThingConverter(id,
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithAttribute"] = _generate.Create(thing)
                });

            var result = JsonSerializer.Serialize(thing,
                new JsonSerializerOptions {IgnoreNullValues = true, WriteIndented = true, Converters = {convert}});

            var actual = JToken.Parse(result);

            actual.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""@type"": null,
                ""Id"": ""{id}ThingWithProperty"",
                ""Title"": null,
                ""Description"": null,
                ""Properties"": {{
                    ""Test"": {{
                        ""ReadOnly"": false,
                        ""Type"": ""number"",
                        ""Links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Test""
                        }}]
                    }},
                    ""Foo"": {{
                        ""ReadOnly"": false,
                        ""Title"": ""Some Title"",
                        ""Description"": ""Some description"",
                        ""@type"": ""Light"",
                        ""@enum"": [ ""Test1"", ""Test2""],
                        ""Type"": ""string"",
                        ""Links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/IsValid""
                        }}]
                    }},
                    ""Bar"": {{
                        ""Title"": null,
                        ""Description"": null,
                        ""@type"": [ ""Light"", ""OnOffSwitch""],
                        ""@enum"": null,
                        ""ReadOnly"": true,
                        ""Type"": ""boolean"",
                        ""Links"": [{{
                            ""href"": ""/things/ThingWithProperty/properties/Bar""
                        }}]
                    }}
                }}
            }}"));
        }
    }

    public class ThingWithProperty : Thing
    {
        public override string Name => "ThingWithProperty";

        public string Text { get; set; }
        public bool IsValid { get; set; }
        public double Percent => 0.5;
        public int Number { get; set; }
    }

    public class ThingWithAttribute : Thing
    {
        public override string Name => "ThingWithAttribute";

        [JsonIgnore] 
        public string Information { get; set; }

        [JsonPropertyName("Test")] 
        public double Lights { get; set; }

        [JsonPropertyInformation(Title = "Some Title", Description = "Some description", Type = new[] { "Light"}, Enum = new object[]{ "Test1", "Test2"})]
        public string Foo { get; set; }

        [JsonPropertyInformation(IsReadOnly = true, Type = new[]{"Light", "OnOffSwitch"})]
        public bool Bar { get; set; }
    }
    
}
