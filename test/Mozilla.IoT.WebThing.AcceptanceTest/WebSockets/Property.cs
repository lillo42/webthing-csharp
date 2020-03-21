using System;
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
    public class Property
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly WebSocketClient _webSocketClient;
        private readonly HttpClient _client;
        private readonly Uri _uri;
        public Property()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
            _webSocketClient = host.GetTestServer().CreateWebSocketClient();

            _uri =  new UriBuilder(_client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/lamp"
            }.Uri;
        }
        
        [Theory]
        [InlineData("on", true)]
        [InlineData("brightness", 10)]
        public async Task SetProperties(string property, object value)
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
    ""messageType"": ""setProperty"",
    ""data"": {{
        ""{property}"": {value.ToString()?.ToLower()}
    }}
}}"), WebSocketMessageType.Text, true,
                    source.Token)
                .ConfigureAwait(false);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var segment = new ArraySegment<byte>(new byte[4096]);
            var result = await socket.ReceiveAsync(segment, source.Token)
                .ConfigureAwait(false);

            result.MessageType.Should().Be(WebSocketMessageType.Text);
            result.EndOfMessage.Should().BeTrue();
            result.CloseStatus.Should().BeNull();

            var json = JToken.Parse(Encoding.UTF8.GetString(segment.Slice(0, result.Count)));
            
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"
{{
    ""messageType"": ""propertyStatus"",
    ""data"": {{
        ""{property}"": {value.ToString()?.ToLower()}
    }}
}}"));
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/lamp/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString()?.ToLower()}  }}"));
        }
        
        [Theory]
        [InlineData("brightness", -1, "Invalid property value")]
        [InlineData("brightness", 101, "Invalid property value")]
        public async Task SetPropertiesInvalidValue(string property, object value, string errorMessage)
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
    ""messageType"": ""setProperty"",
    ""data"": {{
        ""{property}"": {value.ToString()?.ToLower()}
    }}
}}"), WebSocketMessageType.Text, true,
                   source.Token)
                .ConfigureAwait(false);
            
            
            var segment = new ArraySegment<byte>(new byte[4096]);
            var result = await socket.ReceiveAsync(segment, source.Token)
                .ConfigureAwait(false);

            result.MessageType.Should().Be(WebSocketMessageType.Text);
            result.EndOfMessage.Should().BeTrue();
            result.CloseStatus.Should().BeNull();

            var json = JToken.Parse(Encoding.UTF8.GetString(segment.Slice(0, result.Count)));
            
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"
{{
    ""messageType"": ""error"",
    ""data"": {{
        ""message"": ""{errorMessage}"",
        ""status"": ""400 Bad Request""
    }}
}}"));
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/lamp/properties/{property}", source.Token)
                .ConfigureAwait(false);
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            json[property].Value<int>().Should().BeInRange(0, 10);
        }
    }
}