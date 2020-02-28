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

namespace Mozilla.IoT.WebThing.AcceptanceTest.WebScokets
{
    public class PropertyValidationType
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly WebSocketClient _webSocketClient;
        private readonly HttpClient _client;
        private readonly Uri _uri;
        
        public PropertyValidationType()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
            _webSocketClient = host.GetTestServer().CreateWebSocketClient();

            _uri =  new UriBuilder(_client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/web-socket-property-validation-type"
            }.Uri;
        }
        
        [Theory]
        [InlineData("numberByte", 1)]
        [InlineData("numberByte", 10)]
        [InlineData("numberByte", 100)]
        [InlineData("numberSByte", 1)]
        [InlineData("numberSByte", 10)]
        [InlineData("numberSByte", 100)]
        [InlineData("numberShort", 1)]
        [InlineData("numberShort", 10)]
        [InlineData("numberShort", 100)]
        [InlineData("numberUShort", 1)]
        [InlineData("numberUShort", 10)]
        [InlineData("numberUShort", 100)]
        [InlineData("numberInt", 1)]
        [InlineData("numberInt", 10)]
        [InlineData("numberInt", 100)]
        [InlineData("numberUInt", 1)]
        [InlineData("numberUInt", 10)]
        [InlineData("numberUInt", 100)]
        [InlineData("numberLong", 1)]
        [InlineData("numberLong", 10)]
        [InlineData("numberLong", 100)]
        [InlineData("numberULong", 1)]
        [InlineData("numberULong", 10)]
        [InlineData("numberULong", 100)]
        [InlineData("numberDouble", 1)]
        [InlineData("numberDouble", 10)]
        [InlineData("numberDouble", 100)]
        [InlineData("numberFloat", 1)]
        [InlineData("numberFloat", 10)]
        [InlineData("numberFloat", 100)]
        [InlineData("numberDecimal", 1)]
        [InlineData("numberDecimal", 10)]
        [InlineData("numberDecimal", 100)]  
        [InlineData("nullableByte", 1)]  
        [InlineData("nullableByte", 10)]  
        [InlineData("nullableByte", 100)]
        [InlineData("nullableSByte", 1)]
        [InlineData("nullableSByte", 10)]
        [InlineData("nullableSByte", 100)]
        [InlineData("nullableShort", 1)]
        [InlineData("nullableShort", 10)]
        [InlineData("nullableShort", 100)]
        [InlineData("nullableUShort", 1)]
        [InlineData("nullableUShort", 10)]
        [InlineData("nullableUShort", 100)]
        [InlineData("nullableInt", 1)]
        [InlineData("nullableInt", 10)]
        [InlineData("nullableInt", 100)]
        [InlineData("nullableUInt", 1)]
        [InlineData("nullableUInt", 10)]
        [InlineData("nullableUInt", 100)]
        [InlineData("nullableLong", 1)]
        [InlineData("nullableLong", 10)]
        [InlineData("nullableLong", 100)]
        [InlineData("nullableULong", 1)]
        [InlineData("nullableULong", 10)]
        [InlineData("nullableULong", 100)]
        [InlineData("nullableDouble", 1)]
        [InlineData("nullableDouble", 10)]
        [InlineData("nullableDouble", 100)]
        [InlineData("nullableFloat", 1)]
        [InlineData("nullableFloat", 10)]
        [InlineData("nullableFloat", 100)]
        [InlineData("nullableDecimal", 1)]
        [InlineData("nullableDecimal", 10)]
        [InlineData("nullableDecimal", 100)]
        [InlineData("text", "abc")]
        [InlineData("email", "text@test.com")]
        public async Task SetProperties(string property, object value)
        {
            if (value is string || value == null)
            {
                value = value != null ? $"\"{value}\"" : "null";
            }
            
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
        ""{property}"": {value}
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
        ""{property}"": {value}
    }}
}}"));
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/web-socket-property-validation-type/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value}  }}"));
        }
        
        [Theory]
        [InlineData("numberByte", 0)]
        [InlineData("numberByte", 101)]
        [InlineData("numberSByte", 0)]
        [InlineData("numberSByte", 101)]
        [InlineData("numberShort", 0)]
        [InlineData("numberShort", 101)]
        [InlineData("numberUShort", 0)]
        [InlineData("numberUShort", 101)]
        [InlineData("numberInt", 0)]
        [InlineData("numberInt", 101)]
        [InlineData("numberUInt", 0)]
        [InlineData("numberUInt", 101)]
        [InlineData("numberLong", 0)]
        [InlineData("numberLong", 101)]
        [InlineData("numberULong", 0)]
        [InlineData("numberULong", 101)]
        [InlineData("numberDouble", 0)]
        [InlineData("numberDouble", 101)]
        [InlineData("numberFloat", 0)]
        [InlineData("numberFloat", 101)]
        [InlineData("numberDecimal", 0)]
        [InlineData("numberDecimal", 101)]  
        [InlineData("nullableByte", 0)]  
        [InlineData("nullableByte", 101)]
        [InlineData("nullableSByte", 0)]
        [InlineData("nullableSByte", 101)]
        [InlineData("nullableShort", 0)]
        [InlineData("nullableShort", 101)]
        [InlineData("nullableUShort", 0)]
        [InlineData("nullableUShort", 101)]
        [InlineData("nullableInt", 0)]
        [InlineData("nullableInt", 101)]
        [InlineData("nullableUInt", 0)]
        [InlineData("nullableUInt", 101)]
        [InlineData("nullableLong", 0)]
        [InlineData("nullableLong", 101)]
        [InlineData("nullableULong", 0)]
        [InlineData("nullableULong", 101)]
        [InlineData("nullableDouble", 0)]
        [InlineData("nullableDouble", 101)]
        [InlineData("nullableFloat", 0)]
        [InlineData("nullableFloat", 101)]
        [InlineData("nullableDecimal", 0)]
        [InlineData("nullableDecimal", 101)]
        [InlineData("text", "")]
        [InlineData("text", null)]
        [InlineData("email", "text")]
        [InlineData("email", null)]
        public async Task SetPropertiesInvalidNumber(string property, object value)
        {
            if (value is string || value == null)
            {
                value = value != null ? $"\"{value}\"" : "null";
            }
            
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
        ""{property}"": {value}
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
        ""message"": ""Invalid property value"",
        ""status"": ""400 Bad Request""
    }}
}}"));
        }
    }
}
