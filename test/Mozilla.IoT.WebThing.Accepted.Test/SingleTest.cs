using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public async Task Get()
        {
            var responseMessage = await _httpClient.GetAsync("/");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            var expected = JObject.Parse(@"{
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
            }");

            JToken.DeepEquals(JObject.Parse(json), expected).Should().BeTrue();
        }

        #endregion

        #region Property

        [Fact]
        public async Task GetProperties()
        {
            var responseMessage = await _httpClient.GetAsync("/properties");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            var expected = JObject.Parse(@"{
                ""on"": true,
                ""level"": 0
            }");

            JToken.DeepEquals(JObject.Parse(json), expected).Should().BeTrue();
        }

        [Theory]
        [InlineData("on", @"{ ""on"": true }")]
        [InlineData("level", @"{ ""level"": 0 }")]
        public async Task GetProperty(string property, string expectedJson)
        {
            var responseMessage = await _httpClient.GetAsync($"/properties/{property}");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            var expected = JObject.Parse(expectedJson);
            JToken.DeepEquals(JObject.Parse(json), expected).Should().BeTrue();
        }

        [Fact]
        public async Task GetProperty_NotFound()
        {
            var responseMessage = await _httpClient.GetAsync($"/properties/{_fixture.Create<string>()}");
            responseMessage.IsSuccessStatusCode.Should().BeFalse();
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [Theory]
        [InlineData("on", @"{ ""on"": false }")]
        public async Task PutProperty(string property, string expectedJson)
        {
            var content = new StringContent(expectedJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMessage = await _httpClient.PutAsync($"/properties/{property}", content);
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            var expected = JObject.Parse(expectedJson);
            JToken.DeepEquals(JObject.Parse(json), expected).Should().BeTrue();
        }

        [Fact]
        public async Task PutProperty_NotFound()
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
        [InlineData("level", @"{ ""level"": 0.0 }")]
        public async Task SetProperty(string property, string expectedJson)
        {
            var ws = await _webSocketClient.ConnectAsync(
                new Uri($"ws://{_server.BaseAddress.Host}:{_server.BaseAddress.Port}"), CancellationToken.None);
            var json = Encoding.UTF8.GetBytes(JObject.Parse($@"{{
                ""messageType"": ""setProperty"",
                ""data"": {expectedJson} 
            }}").ToString(Formatting.None));
            await ws.SendAsync(new ArraySegment<byte>(json), WebSocketMessageType.Text, true, CancellationToken.None);
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye bye", CancellationToken.None);
            await GetProperty(property, expectedJson);
        }

//        [Fact]
//        public async Task SetProperty_NotFound()
//        {
//            var ws = await _webSocketClient.ConnectAsync(
//                new Uri($"ws://{_server.BaseAddress.Host}:{_server.BaseAddress.Port}"), CancellationToken.None);
//            var json = Encoding.UTF8.GetBytes(JObject.Parse($@"{{
//                ""messageType"": ""setProperty"",
//                ""data"": {{
//                    ""{_fixture.Create<string>()}"": {_fixture.Create<int>()}
//                }} 
//            }}").ToString(Formatting.None));
//            await ws.SendAsync(new ArraySegment<byte>(json), WebSocketMessageType.Text, true, CancellationToken.None);
//
//            var source = new CancellationTokenSource();
//            source.CancelAfter(TimeSpan.FromMinutes(1));
//            var result = await ws.ReceiveAsync(new ArraySegment<byte>(new byte[4096]), source.Token);
//            result.Should().NotBeNull();
//            result.CloseStatus.Should().BeNull();
//            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye bye", CancellationToken.None);
//        }

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
            array[0]["status"].Value<string>().Should().Be("pending");
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
            array[0]["status"].Value<string>().Should().Be("pending");
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
            array[0]["status"].Value<string>().Should().Be("completed");
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
        public async Task GetEmptyEvents()
        {
            var responseMessage = await _httpClient.GetAsync("/events");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
            json.Should().Be("[]");
        }
        
        [Fact]
        public async Task GetEvents()
        {
            await _httpClient.PostAsync($"/actions", new StringContent(@"{
                ""fake"": {}
            }", Encoding.UTF8));

            await Task.Delay(3_000)
                .ConfigureAwait(false);
            
            var responseMessage = await _httpClient.GetAsync("/events");
            responseMessage.IsSuccessStatusCode.Should().BeTrue();
            string json = await responseMessage.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
//            json.Should().NotBe("[]");
//            
//            JArray array = JArray.Parse(json);
//            array.Should().NotBeNullOrEmpty();
//            array.Should().HaveCount(1);

        }

        #endregion
    }
}
