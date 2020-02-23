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
    public class PropertyEnumType
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly WebSocketClient _webSocketClient;
        private readonly HttpClient _client;
        private readonly Uri _uri;
        
        public PropertyEnumType()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
            _webSocketClient = host.GetTestServer().CreateWebSocketClient();

            _uri =  new UriBuilder(_client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/web-socket-property-enum-type"
            }.Uri;
        }
        
        [Theory]
        [InlineData("numberByte", 0)]
        [InlineData("numberByte", byte.MaxValue)]
        [InlineData("numberByte", byte.MinValue)]
        [InlineData("numberSByte", 0)]
        [InlineData("numberSByte", sbyte.MaxValue)]
        [InlineData("numberSByte", sbyte.MinValue)]
        [InlineData("numberShort", 0)]
        [InlineData("numberShort", short.MaxValue)]
        [InlineData("numberShort", short.MinValue)]
        [InlineData("numberUShort", 0)]
        [InlineData("numberUShort", ushort.MaxValue)]
        [InlineData("numberUShort", ushort.MinValue)]
        [InlineData("numberInt", 0)]
        [InlineData("numberInt", int.MaxValue)]
        [InlineData("numberInt", int.MinValue)]
        [InlineData("numberUInt", 0)]
        [InlineData("numberUInt", uint.MaxValue)]
        [InlineData("numberUInt", uint.MinValue)]
        [InlineData("numberLong", 0)]
        [InlineData("numberLong", long.MaxValue)]
        [InlineData("numberLong", long.MinValue)]
        [InlineData("numberULong", 0)]
        [InlineData("numberULong", ulong.MaxValue)]
        [InlineData("numberULong", ulong.MinValue)]
        [InlineData("numberDouble", 0)]
        [InlineData("numberDouble", double.MaxValue)]
        [InlineData("numberDouble", double.MinValue)]
        [InlineData("numberFloat", 0)]
        [InlineData("numberFloat", float.MaxValue)]
        [InlineData("numberFloat", float.MinValue)]
        [InlineData("bool", true)]
        [InlineData("bool", false)]
        [InlineData("nullableBool", null)]
        [InlineData("nullableBool", true)]
        [InlineData("nullableBool", false)] 
        [InlineData("nullableByte", null)]
        [InlineData("nullableByte", byte.MaxValue)]
        [InlineData("nullableByte", byte.MinValue)]
        [InlineData("nullableSByte", null)]
        [InlineData("nullableSByte", sbyte.MinValue)]
        [InlineData("nullableSByte", sbyte.MaxValue)]
        [InlineData("nullableShort", null)]
        [InlineData("nullableShort", short.MinValue)]
        [InlineData("nullableShort", short.MaxValue)]
        [InlineData("nullableUShort", null)]
        [InlineData("nullableUShort", ushort.MinValue)]
        [InlineData("nullableUShort", ushort.MaxValue)]
        [InlineData("nullableInt", null)]
        [InlineData("nullableInt", int.MinValue)]
        [InlineData("nullableInt", int.MaxValue)]
        [InlineData("nullableUInt", null)]
        [InlineData("nullableUInt", uint.MinValue)]
        [InlineData("nullableUInt", uint.MaxValue)]
        [InlineData("nullableLong", null)]
        [InlineData("nullableLong", long.MinValue)]
        [InlineData("nullableLong", long.MaxValue)]
        [InlineData("nullableULong", null)]
        [InlineData("nullableULong", ulong.MinValue)]
        [InlineData("nullableULong", ulong.MaxValue)]
        [InlineData("nullableDouble", null)]
        [InlineData("nullableDouble", double.MinValue)]
        [InlineData("nullableDouble", double.MaxValue)]
        [InlineData("nullableFloat", null)]
        [InlineData("nullableFloat", float.MinValue)]
        [InlineData("nullableFloat", float.MaxValue)]
        [InlineData("nullableDecimal", null)]
        public async Task SetProperties(string property, object value)
        {
            value = value != null ? value.ToString().ToLower() : "null";
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
            
            var response = await _client.GetAsync($"/things/web-socket-property-enum-type/properties/{property}", source.Token)
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
        [InlineData("text", "ola")]
        [InlineData("text", "ass")]
        [InlineData("text", "aaa")]
        [InlineData("text", null)]
        public async Task SetStringValue(string property, string value)
        {
            value = value != null ? $"\"{value}\"" : "null";

            value = value.ToString().ToLower();

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
            
            var response = await _client.GetAsync($"/things/web-socket-property-enum-type/properties/{property}", source.Token)
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
    }
}
