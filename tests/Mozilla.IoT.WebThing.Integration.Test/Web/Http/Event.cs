using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Web.Http
{
    public class Event  : IClassFixture<TestHost>
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private const string s_baseUrl = "/things/event-thing/events";
        
        private readonly HttpClient _client;
        private readonly Fixture _fixture;

        public Event(TestHost testHost)
        {
            _fixture = new Fixture();

            var host = testHost.Host;
            _client = host.GetTestClient();
        }

        #region Not Found

        [Fact]
        public async Task GetEvent_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{_fixture.Create<string>()}/events/level", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetEvent_Should_ReturnNotFound_When_EventNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"{s_baseUrl}/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetEvents_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{_fixture.Create<string>()}/events", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Ok

        [Fact]
        public async Task GetLevelEvent_Should_ReturnOk()
        {
            await Task.Delay(3_000);
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"{s_baseUrl}/level", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Array);
            json.Count().Should().BeGreaterOrEqualTo(1);
            json[0]["level"].Type.Should().Be(JTokenType.Object);
            json[0]["level"]["data"].Type.Should().Be(JTokenType.Integer);
            json[0]["level"]["timestamp"].Type.Should().Be(JTokenType.Date);
        }
        
        
        [Fact]
        public async Task GetFooEvent_Should_ReturnOk()
        {
            await Task.Delay(3_000);
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"{s_baseUrl}/info", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Array);
            json.Count().Should().BeGreaterOrEqualTo(1);
            json[0]["info"].Type.Should().Be(JTokenType.Object);
            json[0]["info"]["data"].Type.Should().Be(JTokenType.String);
            json[0]["info"]["timestamp"].Type.Should().Be(JTokenType.Date);
        }
        
        [Fact]
        public async Task GetLevelEvents_Should_ReturnOk()
        {
            await Task.Delay(3_000);
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"{s_baseUrl}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Array);
            json.Count().Should().BeGreaterOrEqualTo(2);

            var level = GetValue("level"); 
            
            level.Type.Should().Be(JTokenType.Object);
            level["data"].Type.Should().Be(JTokenType.Integer);
            level["timestamp"].Type.Should().Be(JTokenType.Date);
            
            var info =  GetValue("info");
            info.Type.Should().Be(JTokenType.Object);
            info["data"].Type.Should().Be(JTokenType.String);
            info["timestamp"].Type.Should().Be(JTokenType.Date);

            JToken GetValue(string name)
            {
                for (var i = 0; i < json.Count(); i++)
                {
                    if (json[i][name] != null)
                    {
                        return json[i][name];
                    }
                }

                return null;
            }
        }
        #endregion
    }
}
