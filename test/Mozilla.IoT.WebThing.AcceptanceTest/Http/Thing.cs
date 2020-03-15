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
        
        [Fact(Skip = "to fixes")]
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
            ((JArray)json).Should().HaveCount(1);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse($@"[{LAMP}]"));
        }
        
        [Theory(Skip = "to fixes")]
        [InlineData("lamp", LAMP)]
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
        
        [Fact(Skip = "to fixes")]
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
            ((JArray)json).Should().HaveCount(1);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse($@"[{LAMP_USING_ADAPTER}]"));
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
                    .BeEquivalentTo(JToken.Parse(LAMP_USING_ADAPTER));
        }

        


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
                ""input"": {
                    ""type"": ""object"",
                    ""properties"": {}
                },
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
        
         private const string LAMP_USING_ADAPTER = @"
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
                ""input"": {
                    ""type"": ""object"",
                    ""properties"": {}
                },
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
