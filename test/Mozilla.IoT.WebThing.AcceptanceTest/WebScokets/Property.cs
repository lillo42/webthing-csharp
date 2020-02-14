using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.WebScokets
{
    public class Property
    {
        [Theory]
        [InlineData("on", true)]
        [InlineData("brightness", 10)]
        public async Task SetProperties(string property, object value)
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/lamp"
            }.Uri;
            var socket = await webSocketClient.ConnectAsync(uri, CancellationToken.None);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""setProperty"",
    ""data"": {{
        ""{property}"": {value.ToString().ToLower()}
    }}
}}"), WebSocketMessageType.Text, true,
                    CancellationToken.None)
                .ConfigureAwait(false);
            
            var segment = new ArraySegment<byte>(new byte[4096]);
            var result = await socket.ReceiveAsync(segment, CancellationToken.None)
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
        ""{property}"": {value.ToString().ToLower()}
    }}
}}"));
            var response = await client.GetAsync($"/things/Lamp/properties/{property}");
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));
        }
        
        [Theory]
        [InlineData("brightness", -1, 0,  "Invalid property value")]
        [InlineData("brightness", 101, 0, "Invalid property value")]
        [InlineData("reader", 50, 0, "Read-only property")]
        public async Task SetPropertiesInvalidValue(string property, object value, object defaultValue, string errorMessage)
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/lamp"
            }.Uri;
            var socket = await webSocketClient.ConnectAsync(uri, CancellationToken.None);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""setProperty"",
    ""data"": {{
        ""{property}"": {value.ToString().ToLower()}
    }}
}}"), WebSocketMessageType.Text, true,
                    CancellationToken.None)
                .ConfigureAwait(false);
            
            
            var segment = new ArraySegment<byte>(new byte[4096]);
            var result = await socket.ReceiveAsync(segment, CancellationToken.None)
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
            
            var response = await client.GetAsync($"/things/Lamp/properties/{property}");
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {defaultValue.ToString().ToLower()} }}"));
        }
    }
}
