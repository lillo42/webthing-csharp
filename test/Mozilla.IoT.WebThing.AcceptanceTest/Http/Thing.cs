using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class Thing
    {
        [Fact]
        public async Task GetAll()
        {
            var host = await Program.GetHost();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync("/things");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCount(1);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse(@"
[
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""https://iot.mozilla.org/things/Lamp"",
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
                ""type"": ""boolean"",
                ""links"": [
                    {
                        ""href"": ""/things/Lamp/properties/on""
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
                ""type"": ""integer"",
                ""links"": [
                    {
                        ""href"": ""/things/Lamp/properties/brightness""
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
                        ""href"": ""/things/Lamp/actions/fade""
                    }
                ]
            },
            ""longRun"":{
                ""input"": {
                    ""type"": ""object"",
                    ""properties"": {}
                },
                ""links"": [
                    {
                        ""href"": ""/things/Lamp/actions/longRun""
                    }
                ]
            }
        },
        ""events"": {
            ""overheated"": {
                ""title"": ""Overheated"",
                ""description"": ""The lamp has exceeded its safe operating temperature"",
                ""@type"": ""OverheatedEvent"",
                ""type"": ""integer"",
                ""links"": [
                    {
                        ""href"": ""/things/Lamp/events/overheated""
                    }
                ]
            },
            ""otherEvent"": {
                ""title"": ""OtherEvent"",
                ""type"": ""string"",
                ""links"": [{
                    ""href"": ""/things/Lamp/events/otherEvent""
                }]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/Lamp/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/Lamp/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/Lamp/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://iot.mozilla.org:443/things/Lamp""
            }
        ]
    }
]"));
        }
        
        [Fact]
        public async Task Get()
        {
            var host = await Program.GetHost();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync("/things/Lamp");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse(@"
{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""id"": ""https://iot.mozilla.org/things/Lamp"",
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
            ""type"": ""boolean"",
            ""links"": [
                {
                    ""href"": ""/things/Lamp/properties/on""
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
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/Lamp/properties/brightness""
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
                    ""href"": ""/things/Lamp/actions/fade""
                }
            ]
        },
       ""longRun"":{
            ""input"": {
                ""type"": ""object"",
                ""properties"": {}
            },
            ""links"": [
                {
                    ""href"": ""/things/Lamp/actions/longRun""
                }
            ]
        }
    },
    ""events"": {
        ""overheated"": {
            ""title"": ""Overheated"",
            ""description"": ""The lamp has exceeded its safe operating temperature"",
            ""@type"": ""OverheatedEvent"",
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/Lamp/events/overheated""
                }
            ]
        },
        ""otherEvent"": {
            ""title"": ""OtherEvent"",
            ""type"": ""string"",
            ""links"": [{
                ""href"": ""/things/Lamp/events/otherEvent""
            }]
        }
    },
    ""links"": [
        {
            ""rel"": ""properties"",
            ""href"": ""/things/Lamp/properties""
        },
        {
            ""rel"": ""actions"",
            ""href"": ""/things/Lamp/actions""
        },
        {
            ""rel"": ""events"",
            ""href"": ""/things/Lamp/events""
        },
        {
            ""rel"": ""alternate"",
            ""href"": ""ws://iot.mozilla.org:443/things/Lamp""
        }
    ]
}
"));
        }
        
        [Fact]
        public async Task GetInvalid()
        {
            var fixture = new Fixture();
            var host = await Program.GetHost();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync($"/things/{fixture.Create<string>()}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
