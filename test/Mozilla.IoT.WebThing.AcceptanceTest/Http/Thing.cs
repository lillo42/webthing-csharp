using System;
using System.Net;
using System.Net.Http;
using System.Threading;
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
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly HttpClient _client;
        public Thing()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }
        
        [Fact(Skip = "To improve")]
        public async Task GetAll()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync("/things", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCount(5);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse(@"
[
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
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
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
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
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
            },
            ""longRun"": {
                ""links"": [
                    {
                        ""href"": ""/things/lamp/actions/longRun""
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
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/property"",
        ""properties"": {
            ""on"": {
                ""title"": ""On/Off"",
                ""description"": ""Whether the lamp is turned on"",
                ""readOnly"": false,
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/on""
                    }
                ]
            },
            ""brightness"": {
                ""title"": ""Brightness"",
                ""description"": ""The level of light from 0-100"",
                ""readOnly"": false,
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/brightness""
                    }
                ]
            },
            ""reader"": {
                ""readOnly"": true,
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/reader""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/property/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/property/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/property/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/property""
            }
        ]
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/event"",
        ""events"": {
            ""overheated"": {
                ""title"": ""Overheated"",
                ""description"": ""The lamp has exceeded its safe operating temperature"",
                ""@type"": ""OverheatedEvent"",
                ""type"": ""integer"",
                ""links"": [
                    {
                        ""href"": ""/things/event/events/overheated""
                    }
                ]
            },
            ""otherEvent"": {
                ""title"": ""OtherEvent"",
                ""type"": ""string"",
                ""links"": [
                    {
                        ""href"": ""/things/event/events/otherEvent""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/event/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/event/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/event/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/event""
            }
        ]
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/action"",
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
                        ""href"": ""/things/action/actions/fade""
                    }
                ]
            },
            ""longRun"": {
                ""links"": [
                    {
                        ""href"": ""/things/action/actions/longRun""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/action/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/action/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/action/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/action""
            }
        ]
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/web-socket-property"",
        ""properties"": {
            ""on"": {
                ""title"": ""On/Off"",
                ""description"": ""Whether the lamp is turned on"",
                ""readOnly"": false,
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/on""
                    }
                ]
            },
            ""brightness"": {
                ""title"": ""Brightness"",
                ""description"": ""The level of light from 0-100"",
                ""readOnly"": false,
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/brightness""
                    }
                ]
            },
            ""reader"": {
                ""readOnly"": true,
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/reader""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/web-socket-property/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/web-socket-property/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/web-socket-property/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/web-socket-property""
            }
        ]
    }
]
"));
        }
        
        [Theory]
        [InlineData("lamp", LAMP)]
        [InlineData("property", PROPERTY)]
        [InlineData("event", EVENT)]
        [InlineData("action", ACTION)]
        [InlineData("web-socket-property", WEB_SOCKET_PROPERTY)]
        public async Task Get(string thing, string expected)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{thing}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse(expected));
        }
        
        [Fact]
        public async Task GetInvalid()
        {
            var fixture = new Fixture();
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{fixture.Create<string>()}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact(Skip = "To improve")]
        public async Task GetAllWhenUseThingAdapter()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var host = await Program.CreateHostBuilder(null, opt => opt.UseThingAdapterUrl = true)
                .StartAsync(source.Token)
                .ConfigureAwait(false);

            var client = host.GetTestServer().CreateClient();
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            var response = await client.GetAsync("/things", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCount(5);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse(@"
[
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""lamp"",
        ""href"": ""/things/lamp"",
        ""base"": ""http://localhost/things/lamp"",
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
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
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
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
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
            },
            ""longRun"": {
                ""links"": [
                    {
                        ""href"": ""/things/lamp/actions/longRun""
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
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""property"",
        ""href"": ""/things/property"",
        ""base"": ""http://localhost/things/property"",
        ""properties"": {
            ""on"": {
                ""title"": ""On/Off"",
                ""description"": ""Whether the lamp is turned on"",
                ""readOnly"": false,
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/on""
                    }
                ]
            },
            ""brightness"": {
                ""title"": ""Brightness"",
                ""description"": ""The level of light from 0-100"",
                ""readOnly"": false,
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/brightness""
                    }
                ]
            },
            ""reader"": {
                ""readOnly"": true,
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/reader""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/property/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/property/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/property/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/property""
            }
        ]
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""event"",
        ""href"": ""/things/event"",
        ""base"": ""http://localhost/things/event"",
        ""events"": {
            ""overheated"": {
                ""title"": ""Overheated"",
                ""description"": ""The lamp has exceeded its safe operating temperature"",
                ""@type"": ""OverheatedEvent"",
                ""type"": ""integer"",
                ""links"": [
                    {
                        ""href"": ""/things/event/events/overheated""
                    }
                ]
            },
            ""otherEvent"": {
                ""title"": ""OtherEvent"",
                ""type"": ""string"",
                ""links"": [
                    {
                        ""href"": ""/things/event/events/otherEvent""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/event/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/event/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/event/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/event""
            }
        ]
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""action"",
        ""href"": ""/things/action"",
        ""base"": ""http://localhost/things/action"",
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
                        ""href"": ""/things/action/actions/fade""
                    }
                ]
            },
            ""longRun"": {
                ""links"": [
                    {
                        ""href"": ""/things/action/actions/longRun""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/action/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/action/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/action/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/action""
            }
        ]
    },
    {
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""web-socket-property"",
        ""href"": ""/things/web-socket-property"",
        ""base"": ""http://localhost/things/web-socket-property"",
        ""properties"": {
            ""on"": {
                ""title"": ""On/Off"",
                ""description"": ""Whether the lamp is turned on"",
                ""readOnly"": false,
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/on""
                    }
                ]
            },
            ""brightness"": {
                ""title"": ""Brightness"",
                ""description"": ""The level of light from 0-100"",
                ""readOnly"": false,
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/brightness""
                    }
                ]
            },
            ""reader"": {
                ""readOnly"": true,
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/reader""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/web-socket-property/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/web-socket-property/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/web-socket-property/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/web-socket-property""
            }
        ]
    }
]
"));
        }
        
        [Fact]
        public async Task GetWhenUseThingAdapter()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
                
            var host = await Program.CreateHostBuilder(null, opt => opt.UseThingAdapterUrl = true)
                .StartAsync(source.Token)
                .ConfigureAwait(false);

            var client = host.GetTestServer().CreateClient();
            
            var response = await client.GetAsync("/things/Lamp", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse(@"
{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""id"": ""lamp"",
    ""href"": ""/things/lamp"",
    ""base"": ""http://localhost/things/lamp"",
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
            ""type"": ""boolean"",
            ""@type"": ""OnOffProperty"",
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
            ""type"": ""integer"",
            ""@type"": ""BrightnessProperty"",
            ""minimum"": 0,
            ""maximum"": 100,
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
        },
        ""longRun"": {
            ""links"": [
                {
                    ""href"": ""/things/lamp/actions/longRun""
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


        private const string WEB_SOCKET_PROPERTY = @"{
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/web-socket-property"",
        ""properties"": {
            ""on"": {
                ""title"": ""On/Off"",
                ""description"": ""Whether the lamp is turned on"",
                ""readOnly"": false,
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/on""
                    }
                ]
            },
            ""brightness"": {
                ""title"": ""Brightness"",
                ""description"": ""The level of light from 0-100"",
                ""readOnly"": false,
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/brightness""
                    }
                ]
            },
            ""reader"": {
                ""readOnly"": true,
                ""links"": [
                    {
                        ""href"": ""/things/web-socket-property/properties/reader""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/web-socket-property/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/web-socket-property/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/web-socket-property/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/web-socket-property""
            }
        ]
    }";
        
        private const string ACTION = @"{
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/action"",
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
                        ""href"": ""/things/action/actions/fade""
                    }
                ]
            },
            ""longRun"": {
                ""links"": [
                    {
                        ""href"": ""/things/action/actions/longRun""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/action/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/action/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/action/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/action""
            }
        ]
    }";
        private const string EVENT = @"{
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/event"",
        ""events"": {
            ""overheated"": {
                ""title"": ""Overheated"",
                ""description"": ""The lamp has exceeded its safe operating temperature"",
                ""@type"": ""OverheatedEvent"",
                ""type"": ""integer"",
                ""links"": [
                    {
                        ""href"": ""/things/event/events/overheated""
                    }
                ]
            },
            ""otherEvent"": {
                ""title"": ""OtherEvent"",
                ""type"": ""string"",
                ""links"": [
                    {
                        ""href"": ""/things/event/events/otherEvent""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/event/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/event/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/event/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/event""
            }
        ]
    }";
        
        private const string PROPERTY = @"{
        ""@context"": ""https://iot.mozilla.org/schemas"",
        ""id"": ""http://localhost/things/property"",
        ""properties"": {
            ""on"": {
                ""title"": ""On/Off"",
                ""description"": ""Whether the lamp is turned on"",
                ""readOnly"": false,
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/on""
                    }
                ]
            },
            ""brightness"": {
                ""title"": ""Brightness"",
                ""description"": ""The level of light from 0-100"",
                ""readOnly"": false,
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/brightness""
                    }
                ]
            },
            ""reader"": {
                ""readOnly"": true,
                ""links"": [
                    {
                        ""href"": ""/things/property/properties/reader""
                    }
                ]
            }
        },
        ""links"": [
            {
                ""rel"": ""properties"",
                ""href"": ""/things/property/properties""
            },
            {
                ""rel"": ""actions"",
                ""href"": ""/things/property/actions""
            },
            {
                ""rel"": ""events"",
                ""href"": ""/things/property/events""
            },
            {
                ""rel"": ""alternate"",
                ""href"": ""ws://localhost/things/property""
            }
        ]
    }";

        private const string LAMP = @"
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
                ""type"": ""boolean"",
                ""@type"": ""OnOffProperty"",
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
                ""type"": ""integer"",
                ""@type"": ""BrightnessProperty"",
                ""minimum"": 0,
                ""maximum"": 100,
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
            },
            ""longRun"": {
                ""links"": [
                    {
                        ""href"": ""/things/lamp/actions/longRun""
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
    }";
    }
}
