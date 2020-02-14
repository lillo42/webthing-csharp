using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.WebScokets
{
    public class Action
    {
        [Theory]
        [InlineData(50, 2_000)]
        public async Task Create(int level, int duration)
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
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""fade"": {{
            ""input"": {{
                 ""level"": {level},
                 ""duration"": {duration}
            }}
        }}
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

            var json = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(segment.Slice(0, result.Count)),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.Fade.Input.Should().NotBeNull();
            json.Data.Fade.Input.Level.Should().Be(level);
            json.Data.Fade.Input.Duration.Should().Be(duration);
            json.Data.Fade.Href.Should().StartWith("/things/lamp/actions/fade/");
            json.Data.Fade.Status.Should().Be("pending");
            json.Data.Fade.TimeRequested.Should().BeBefore(DateTime.UtcNow);
            json.Data.Fade.TimeCompleted.Should().BeNull();
            
            segment = new ArraySegment<byte>(new byte[4096]);
            result = await socket.ReceiveAsync(segment, CancellationToken.None)
                .ConfigureAwait(false);

            result.MessageType.Should().Be(WebSocketMessageType.Text);
            result.EndOfMessage.Should().BeTrue();
            result.CloseStatus.Should().BeNull();

            json = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(segment.Slice(0, result.Count)),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.Fade.Input.Should().NotBeNull();
            json.Data.Fade.Input.Level.Should().Be(level);
            json.Data.Fade.Input.Duration.Should().Be(duration);
            json.Data.Fade.Href.Should().StartWith("/things/lamp/actions/fade/");
            json.Data.Fade.Status.Should().Be("executing");
            json.Data.Fade.TimeRequested.Should().BeBefore(DateTime.UtcNow);
            json.Data.Fade.TimeCompleted.Should().BeNull();
            
            segment = new ArraySegment<byte>(new byte[4096]);
            result = await socket.ReceiveAsync(segment, CancellationToken.None)
                .ConfigureAwait(false);

            result.MessageType.Should().Be(WebSocketMessageType.Text);
            result.EndOfMessage.Should().BeTrue();
            result.CloseStatus.Should().BeNull();

            json = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(segment.Slice(0, result.Count)),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.Fade.Input.Should().NotBeNull();
            json.Data.Fade.Input.Level.Should().Be(level);
            json.Data.Fade.Input.Duration.Should().Be(duration);
            json.Data.Fade.Href.Should().StartWith("/things/lamp/actions/fade/");
            json.Data.Fade.Status.Should().Be("completed");
            json.Data.Fade.TimeRequested.Should().BeBefore(DateTime.UtcNow);
            json.Data.Fade.TimeCompleted.Should().NotBeNull();
            
            var response = await client.GetAsync($"/things/lamp/actions/fade");
            var message = await response.Content.ReadAsStringAsync();
            var json2 = JsonConvert.DeserializeObject<List<Http.Action.Fade>>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json2[0].Href.Should().StartWith("/things/lamp/actions/fade/");
            json2[0].Status.Should().NotBeNullOrEmpty();
            json2[0].Status.Should().Be("completed");
            json2[0].TimeRequested.Should().BeBefore(DateTime.UtcNow);
            json2[0].TimeCompleted.Should().NotBeNull();
            json2[0].TimeCompleted.Should().BeBefore(DateTime.UtcNow);
        }
        
        public class Message
        {
            public string MessageType { get; set; }
            public ActionSocket Data { get; set; }
        }

        public class ActionSocket
        {
            public Http.Action.Fade Fade { get; set; }
        }
            
    }
}
