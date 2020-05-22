using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Web.WebSockets
{
    public class Event : IClassFixture<TestHost>
    {
        private const int s_retries = 3;
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly Uri _baseUrl;

        private readonly WebSocketClient _socketClient;

        public Event(TestHost testHost)
        {
            var host = testHost.Host;
            _baseUrl = new UriBuilder(host.GetTestServer().BaseAddress) {Scheme = "ws", Path = "/things/web-socket-event-thing/"}.Uri;
            _socketClient = host.GetTestServer().CreateWebSocketClient();
        }

        #region Not Found

        [Fact]
        public async Task AddEventSubscription_Should_ReturnNotFound_When_ActionNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var message = new WebSocketRequest
            {
                MessageType = "addEventSubscription",
                Data = new
                {
                    Abc = new object()
                }
            };
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            while (true)
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("error");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["status"].Type.Should().Be(JTokenType.String);
                    json["data"]["status"].Value<string>().Should().StartWith("404");
                    break;
                }
                catch (Exception)
                {
                    counter++;

                    if (counter >= s_retries)
                    {
                        throw;
                    }
                }
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        #endregion

        #region Ok

        [Fact]
        public async Task AddEventSubscriptionForLevel_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var message = new WebSocketRequest
            {
                MessageType = "addEventSubscription",
                Data = new
                {
                    Level = new object()
                }
            };
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            while (true)
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("event");
                    json["data"]["level"].Type.Should().Be(JTokenType.Object);
                    json["data"]["level"]["data"].Type.Should().Be(JTokenType.Integer);
                    json["data"]["level"]["timestamp"].Type.Should().Be(JTokenType.Date);
                    break;
                }
                catch (Exception)
                {
                    counter++;

                    if (counter >= s_retries)
                    {
                        throw;
                    }
                }
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
        
        [Fact]
        public async Task AddEventSubscriptionForInfoShould_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var message = new WebSocketRequest
            {
                MessageType = "addEventSubscription",
                Data = new
                {
                    Info = new object()
                }
            };
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            while (true)
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("event");
                    json["data"]["info"].Type.Should().Be(JTokenType.Object);
                    json["data"]["info"]["data"].Type.Should().Be(JTokenType.String);
                    json["data"]["info"]["timestamp"].Type.Should().Be(JTokenType.Date);
                    break;
                }
                catch (Exception)
                {
                    counter++;

                    if (counter >= s_retries)
                    {
                        throw;
                    }
                }
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
        
        [Fact]
        public async Task AddEventSubscriptionForAllEvent_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var message = new WebSocketRequest
            {
                MessageType = "addEventSubscription",
                Data = new
                {
                    Level = new object(),
                    Info = new object()
                }
            };
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            var state = 0;
            var level = false;
            var info = false;
            while (state < 1)
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("event");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    
                    
                    if (json["data"]["level"] != null)
                    {
                        json["data"]["level"].Type.Should().Be(JTokenType.Object);
                        json["data"]["level"]["data"].Type.Should().Be(JTokenType.Integer);
                        json["data"]["level"]["timestamp"].Type.Should().Be(JTokenType.Date);
                        
                        if (!level)
                        {
                            state++;
                            level = true;
                        }
                    }
                    else
                    {
                        json["data"]["info"].Type.Should().Be(JTokenType.Object);
                        json["data"]["info"]["data"].Type.Should().Be(JTokenType.String);
                        json["data"]["info"]["timestamp"].Type.Should().Be(JTokenType.Date);
                        
                        if (!info)
                        {
                            state++;
                            info = true;
                        }
                    }
                }
                catch (Exception)
                {
                    counter++;

                    if (counter >= s_retries)
                    {
                        throw;
                    }
                }
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        #endregion
    }
}
