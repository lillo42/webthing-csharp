using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Accepted.Test.Startups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Accepted.Test
{
    public class SingleTest
    {
        private readonly Fixture _fixture;
        private readonly TestServer _server;
        private readonly HttpClient _httpClient;
        private readonly WebSocketClient _webSocketClient;

        public SingleTest()
        {
            _fixture = new Fixture();
            _server = new TestServer(WebHost.CreateDefaultBuilder<SingleStartup>(new string[0])
                .ConfigureServices(service => service.AddLogging())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                }));
            _httpClient = _server.CreateClient();
            _webSocketClient = _server.CreateWebSocketClient();
        }

        
        #region Thing
        
        [Fact]
        public async Task GetThing_Should_Ok_When_HaveThing()
        {
            var responseMessage = await _httpClient.GetAsync("/");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            JObject.Parse(json).Should().BeEquivalentTo(JObject.Parse(@"{
                ""name"": ""My Lamp"",
                ""href"": ""/"",
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""@type"": [ ""OnOffSwitch"", ""Light"" ],
                ""properties"": {
                    ""on"": {
                        ""@type"": ""OnOffProperty"",
                        ""type"": ""OnOffProperty"",
                        ""title"": ""On/Off"",
                        ""type"": ""boolean"",
                        ""description"": ""Whether the lamp is turned on"",
                        ""links"":[{
                            ""rel"": ""property"",
                            ""href"": ""/properties/on""
                        }]
                    },
                    ""level"": { 
                        ""@type"": ""BrightnessProperty"",
                        ""title"": ""Brightness"",
                        ""type"": ""number"",
                        ""description"": ""The level of light from 0-100"",
                        ""minimum"": 0,
                        ""maximum"": 100,
                        ""unit"": ""percent"",
                        ""links"": [{
                            ""rel"": ""property"",
                            ""href"": ""/properties/level""
                        }]
                    }
                },
                ""actions"": {
                    ""fake"": {
                        ""links"": [{
                            ""rel"":""action"",
                            ""href"":""/actions/fake""
                        }]
                    }
                },
                ""events"": {},
                ""description"": ""A web connected lamp"",
                ""links"":[{
                    ""rel"": ""properties"",
                    ""href"": ""/properties""
                },{
                    ""rel"": ""actions"",
                    ""href"": ""/actions""
                },{
                    ""rel"": ""events"",
                    ""href"": ""/events""
                },{
                    ""rel"": ""alternate"",
                    ""href"": ""ws://localhost/""
                }]
            }"));
        }

        #endregion

        #region Property

        [Fact]
        public async Task GetProperties_Should_Return200_When_PropertyExist()
        {
            var responseMessage = await _httpClient.GetAsync("/properties");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            JObject.Parse(json).Should().BeEquivalentTo( JObject.Parse(@"{
                ""on"": true,
                ""level"": 0
            }"));
        }

        [Theory]
        [InlineData("on", @"{ ""on"": true }")]
        [InlineData("level", @"{ ""level"": 0 }")]
        public async Task GetProperty_Should_ReturnValue_When_PropertyExist(string property, string expectedJson)
        {
            var responseMessage = await _httpClient.GetAsync($"/properties/{property}");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            JObject.Parse(json).Should().BeEquivalentTo(JObject.Parse(expectedJson));
        }

        [Fact]
        public async Task GetProperty_Should_ReturnNotFound_When_PropertyNotExist()
        {
            var responseMessage = await _httpClient.GetAsync($"/properties/{_fixture.Create<string>()}");
            responseMessage.IsSuccessStatusCode.Should().BeFalse();
            responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("on", @"{ ""on"": false }")]
        public async Task PutProperty_Should_Return_When_PropertyExist(string property, string expectedJson)
        {
            var content = new StringContent(expectedJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMessage = await _httpClient.PutAsync($"/properties/{property}", content);
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            JObject.Parse(json).Should().BeEquivalentTo(JObject.Parse(expectedJson));
        }

        [Fact]
        public async Task PutProperty_Should_ReturnNotFound_When_PropertyNotExist()
        {
            string property = _fixture.Create<string>();
            var content = new StringContent($@"{{ ""{property}"": {_fixture.Create<int>()} }}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMessage = await _httpClient.PutAsync($"/properties/{property}", content);
            responseMessage.IsSuccessStatusCode.Should().BeFalse();
            responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("on", @"{ ""on"": true }")]
        [InlineData("level", @"{ ""level"": 30.0 }")]
        public async Task SetProperty_Should_ChangeProperty_When_PropertyExists(string property, string expectedJson)
        {
            var ws = await _webSocketClient.ConnectAsync(
                new Uri($"ws://{_server.BaseAddress.Host}:{_server.BaseAddress.Port}"), CancellationToken.None);
            var json = Encoding.UTF8.GetBytes(JObject.Parse($@"{{
                ""messageType"": ""setProperty"",
                ""data"": {expectedJson} 
            }}").ToString(Formatting.None));
            await ws.SendAsync(new ArraySegment<byte>(json), WebSocketMessageType.Text, true, CancellationToken.None);
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye bye", CancellationToken.None);
            await GetProperty_Should_ReturnValue_When_PropertyExist(property, expectedJson);
        }
        
        [Theory]
        [InlineData(@"{ ""on"": true }")]
        [InlineData(@"{ ""level"": 30.0 }")]
        public async Task PropertyStatus_Should_Notify_When_PropertyChange(string expectedJson)
        {
            var ws = await _webSocketClient.ConnectAsync(
                new Uri($"ws://{_server.BaseAddress.Host}:{_server.BaseAddress.Port}"), CancellationToken.None);
            var json = Encoding.UTF8.GetBytes(JObject.Parse($@"{{
                ""messageType"": ""setProperty"",
                ""data"": {expectedJson} 
            }}").ToString(Formatting.None));
            await ws.SendAsync(new ArraySegment<byte>(json), WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new ArraySegment<byte>(new byte[4096]);
            await ws.ReceiveAsync(buffer, CancellationToken.None);

            var actual = JToken.Parse(Encoding.UTF8.GetString(buffer.AsSpan(0, buffer.Count)));

            actual.Should().BeEquivalentTo(JToken.Parse($@"{{
                ""messageType"": ""propertyStatus"",
                ""data"": {expectedJson}
            }}"));
        }

        #endregion

        #region Action

        [Fact]
        public async Task GetActions()
        {
            await _httpClient.PostAsync($"/actions/fake", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));
            
            var getResponseMessage = await _httpClient.GetAsync("/actions");
            getResponseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await getResponseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            
            var array = JArray.Parse(json);
            array.Should().HaveCount(1);
            array[0]["fake"]["status"].Value<string>().Should().Be("pending");
        }
        
        [Fact]
        public async Task GetAction()
        {
            var responseMessage = await _httpClient.PostAsync($"/actions/fake", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));
            
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            
            var jObject = JObject.Parse(json);

            var getResponseMessage = await _httpClient.GetAsync(jObject["fake"]["href"].Value<string>());
            getResponseMessage.IsSuccessStatusCode.Should().BeTrue();
            string getJson = await getResponseMessage.Content.ReadAsStringAsync();
            getJson.Should().NotBeNullOrEmpty();

            var getAction = JObject.Parse(getJson);
            getAction.ContainsKey("fake").Should().BeTrue();
            
            var fake = getAction["fake"] as JObject;
            fake.Should().NotBeNull();

            fake.ContainsKey("href").Should().BeTrue("Should contain \"href\" ");
            fake["href"].Value<string>().Should().NotBeNullOrEmpty();
            fake.ContainsKey("status").Should().BeTrue("Should contain \"href\" ");
            fake["status"].Value<string>().Should().NotBeNullOrEmpty();
        }
        
        
        [Fact]
        public async Task PostActions()
        {
            var responseMessage = await _httpClient.PostAsync($"/actions", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));
            
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
            
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            
            var jObject = JObject.Parse(json);
            jObject.ContainsKey("fake").Should().BeTrue();
            
            var fake = jObject["fake"] as JObject;
            fake.Should().NotBeNull();

            fake.ContainsKey("href").Should().BeTrue("Should contain \"href\" ");
            fake["href"].Value<string>().Should().NotBeNullOrEmpty();
            fake.ContainsKey("status").Should().BeTrue("Should contain \"href\" ");
            fake["status"].Value<string>().Should().NotBeNullOrEmpty();
            
            var getResponseMessage = await _httpClient.GetAsync("/actions");
            getResponseMessage.IsSuccessStatusCode.Should().BeTrue();
            string getJson = await getResponseMessage.Content.ReadAsStringAsync();
            getJson.Should().NotBeNullOrEmpty();
            getJson.Should().NotBe("[]");
        }

        [Theory]
        [InlineData("fake", "{}")]
        public async Task PostAction(string action, string sendJson)
        {
            var responseMessage = await _httpClient.PostAsync($"/actions/{action}", new StringContent($@"{{
                ""{action}"": {sendJson}
            }}", Encoding.UTF8));
            
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
            
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            
            var jObject = JObject.Parse(json);
            jObject.ContainsKey(action).Should().BeTrue();
            
            var fake = jObject[action] as JObject;
            fake.Should().NotBeNull();

            fake.ContainsKey("href").Should().BeTrue("Should contain \"href\" ");
            fake["href"].Value<string>().Should().NotBeNullOrEmpty();
            fake.ContainsKey("status").Should().BeTrue("Should contain \"href\" ");
            fake["status"].Value<string>().Should().NotBeNullOrEmpty();
            
            var getResponseMessage = await _httpClient.GetAsync("/actions");
            getResponseMessage.IsSuccessStatusCode.Should().BeTrue();
            
            string getJson = await getResponseMessage.Content.ReadAsStringAsync();
            getJson.Should().NotBeNullOrEmpty();

            JArray array = JArray.Parse(getJson);
            array.Should().HaveCount(1);
            array[0]["fake"]["status"].Value<string>().Should().Be("pending");
        }
        
        [Fact]
        public async Task PostAndWaitActions()
        {
            var responseMessage = await _httpClient.PostAsync($"/actions", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));
            
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
            
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            
            var jObject = JObject.Parse(json);
            jObject.ContainsKey("fake").Should().BeTrue();
            
            var fake = jObject["fake"] as JObject;
            fake.Should().NotBeNull();

            fake.ContainsKey("href").Should().BeTrue("Should contain \"href\" ");
            fake["href"].Value<string>().Should().NotBeNullOrEmpty();
            fake.ContainsKey("status").Should().BeTrue("Should contain \"href\" ");
            fake["status"].Value<string>().Should().NotBeNullOrEmpty();


            await Task.Delay(3_000);
            
            var getResponseMessage = await _httpClient.GetAsync("/actions/fake");
            getResponseMessage.IsSuccessStatusCode.Should().BeTrue();
            string getJson = await getResponseMessage.Content.ReadAsStringAsync();
            getJson.Should().NotBeNullOrEmpty();
            getJson.Should().NotBe("[]");
            
            JArray array = JArray.Parse(getJson);
            array.Should().HaveCount(1);
            array[0]["fake"]["status"].Value<string>().Should().Be("completed");
        }

        [Fact]
        public async Task DeleteActions()
        {
            var responseMessage = await _httpClient.PostAsync($"/actions", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));
            
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
            
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            
            var jObject = JObject.Parse(json);
            jObject.ContainsKey("fake").Should().BeTrue();

            var deleteResult = await _httpClient.DeleteAsync(jObject["fake"]["href"].Value<string>());
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            var getResponseMessage = await _httpClient.GetAsync("/actions");
            getResponseMessage.IsSuccessStatusCode.Should().BeTrue();
            string getJson = await getResponseMessage.Content.ReadAsStringAsync();
            getJson.Should().NotBeNullOrEmpty();
            getJson.Should().Be("[]");

        }
        
        //TODO: WebSocket
        
        #endregion

        #region Event

        [Fact]
        public async Task GetEmptyEvents_Should_BeEmpty_When_NoEventOccur()
        {
            var responseMessage = await _httpClient.GetAsync("/events");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            json.Should().Be("[]");
        }
        
        [Fact]
        public async Task GetEvents_Should_ReturnOne_When_AnyEventOccur()
        {
            await _httpClient.PostAsync($"/actions", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));

            await Task.Delay(100);
            
            var responseMessage = await _httpClient.GetAsync("/events");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            JArray.Parse(json).Count.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task AddEventSubscription_Should_BeNotify_When_AnyEvent_AnyEventOCcur()
        {
            var ws = await _webSocketClient.ConnectAsync(
                new Uri($"ws://{_server.BaseAddress.Host}:{_server.BaseAddress.Port}"), CancellationToken.None);
            var json = Encoding.UTF8.GetBytes(JObject.Parse(@"{
                ""messageType"": ""addEventSubscription"",
                ""data"": {
                    ""fake"": {}
                } 
            }").ToString(Formatting.None));
            await ws.SendAsync(new ArraySegment<byte>(json), WebSocketMessageType.Text, true, CancellationToken.None);

            await _httpClient.PostAsync($"/actions", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));

            await Task.Delay(100);

            var buffer = new ArraySegment<byte>(new byte[4096]);
            var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
            string jsonResult = Encoding.UTF8.GetString(buffer.AsSpan(0, result.Count));
            jsonResult.Should().NotBeNullOrEmpty();
            var token = JToken.Parse(jsonResult);

            token["messageType"].Should().NotBeNull();

            if (token["messageType"].Value<string>() != "event")
            {
                buffer = new ArraySegment<byte>(new byte[4096]);
                result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                jsonResult = Encoding.UTF8.GetString(buffer.AsSpan(0, result.Count));
                jsonResult.Should().NotBeNullOrEmpty();
                token = JToken.Parse(jsonResult);
            }

            token["messageType"].Value<string>().Should().Be("event");
            token["data"].Should().NotBeNull();
            token["data"]["fake"].Should().NotBeNull();
        }   

        #endregion
    }
}
