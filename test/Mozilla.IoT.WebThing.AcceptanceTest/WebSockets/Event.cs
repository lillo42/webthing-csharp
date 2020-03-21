using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.WebSockets
{
    public class Event
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly WebSocketClient _webSocketClient;
        private readonly HttpClient _client;
        private readonly Uri _uri;

        public Event()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
            _webSocketClient = host.GetTestServer().CreateWebSocketClient();

            _uri = new UriBuilder(_client.BaseAddress) {Scheme = "ws", Path = "/things/lamp"}.Uri;
        }

        [Theory]
        [InlineData("overheated")]
        public async Task EventSubscription(string @event)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var socket = await _webSocketClient.ConnectAsync(_uri, source.Token)
                .ConfigureAwait(false);

            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""addEventSubscription"",
    ""data"": {{
        ""{@event}"": {{}}
    }}
}}"), WebSocketMessageType.Text, true,
                    source.Token)
                .ConfigureAwait(false);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var segment = new ArraySegment<byte>(new byte[4096]);
            var result = await socket.ReceiveAsync(segment,source.Token)
                .ConfigureAwait(false);

            result.MessageType.Should().Be(WebSocketMessageType.Text);
            result.EndOfMessage.Should().BeTrue();
            result.CloseStatus.Should().BeNull();

            var json = JToken.Parse(Encoding.UTF8.GetString(segment.Slice(0, result.Count)));
            json.Type.Should().Be(JTokenType.Object);

            var obj = (JObject)json;
            
            obj.GetValue("messageType", StringComparison.OrdinalIgnoreCase).Type.Should()
                .Be(JTokenType.String);
            
            obj.GetValue("messageType", StringComparison.OrdinalIgnoreCase).Value<string>().Should()
                .Be("event");

            ((JObject)obj.GetValue("data", StringComparison.OrdinalIgnoreCase))
                .GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);


            var overheated = ((JObject)((JObject)obj.GetValue("data", StringComparison.OrdinalIgnoreCase))
                .GetValue("overheated", StringComparison.OrdinalIgnoreCase));
            
            overheated
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            overheated
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/lamp/events/{@event}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCountGreaterOrEqualTo(1);

            obj = ((JArray)json).Last() as JObject;
            obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Value<int>().Should().Be(0);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);

        }
    }
}
