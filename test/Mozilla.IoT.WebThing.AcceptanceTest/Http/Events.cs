using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class Events
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly HttpClient _client;
        
        public Events()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }
        
        #region GET

        [Fact]
        public async Task GetAll()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.GetAsync("/things/event/events", source.Token)
                .ConfigureAwait(false);

            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be("application/json");

            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);

            json.Type.Should().Be(JTokenType.Array);
            
            if (((JArray)json).Count == 0)
            {
                await Task.Delay(3_000).ConfigureAwait(false);
            
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);
            
                response = await _client.GetAsync("/things/event/events", source.Token)
                    .ConfigureAwait(false);
            
                response.IsSuccessStatusCode.Should().BeTrue();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
                message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                json = JToken.Parse(message);
                
                json.Type.Should().Be(JTokenType.Array);
                ((JArray)json).Should().HaveCountGreaterOrEqualTo(1);
            }


            var obj = ((JArray)json)[0] as JObject;
            obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Value<int>().Should().Be(0);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);

        }
        
        [Fact]
        public async Task GetEvent()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync("/things/event/events/overheated", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            
            if (((JArray)json).Count == 0)
            {
                await Task.Delay(3_000).ConfigureAwait(false);
            
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);
            
                response = await _client.GetAsync("/things/event/events", source.Token)
                    .ConfigureAwait(false);
            
                response.IsSuccessStatusCode.Should().BeTrue();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
                message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                json = JToken.Parse(message);
                
                json.Type.Should().Be(JTokenType.Array);
                ((JArray)json).Should().HaveCountGreaterOrEqualTo(1);
            }

            var obj = ((JArray)json)[0] as JObject;
            obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Value<int>().Should().Be(0);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);
        }
        
        [Fact]
        public async Task GetInvalidEvent()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync("/things/event/events/aaaaa", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        }
        #endregion

    }
}
