using System;
using System.Net.Http;
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
    public class Properties : IDisposable
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly Uri _baseUrl;

        private WebSocket _socket;
        private readonly WebSocketClient _socketClient;
        private readonly Fixture _fixture;

        public Properties()
        {
            _fixture = new Fixture();
            var host = HostFactory.CreateHost().GetAwaiter().GetResult();
            _baseUrl = new UriBuilder(host.GetTestServer().BaseAddress) {Scheme = "ws", Path = "/things/web-socket-property-thing/"}.Uri;
            _socketClient = host.GetTestServer().CreateWebSocketClient();
        }

        #region Ok

        [Fact]
        public async Task SetThing_Should_ReturnOk_When_SetText()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<string>();
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    Text = value
                }
            };
            
            _socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            _socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await _socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            do
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await _socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["text"].Type.Should().Be(JTokenType.String);
                    json["data"]["text"].Value<string>().Should().Be(value);
                    break;
                }
                catch (Exception e)
                {
                    counter++;
                }
            } while (counter <= 3);

            counter.Should().BeLessOrEqualTo(3);
        }

        [Theory] 
        [InlineData(0)] 
        [InlineData(20)] 
        [InlineData(10)]
        public async Task SetThing_Should_ReturnOk_When_SetLevel(int value)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    Level = value
                }
            };
            
            _socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            _socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await _socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            do
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await _socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["level"].Type.Should().Be(JTokenType.Integer);
                    json["data"]["level"].Value<int>().Should().Be(value);
                    break;
                }
                catch (Exception e)
                {
                    counter++;
                }
            } while (counter <= 3);

            counter.Should().BeLessOrEqualTo(3);
        }
        
        [Fact]
        public async Task SetThing_Should_ReturnOk_When_SetExtraInformation()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = new[]{"DEF", "GHI"};
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    ExtraInformation = value
                }
            };
            
            _socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            _socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await _socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            do
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await _socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["extraInformation"].Type.Should().Be(JTokenType.Array);
                    json["data"]["extraInformation"].Value<string[]>().Should().BeEquivalentTo(value);
                    break;
                }
                catch (Exception e)
                {
                    counter++;
                }
            } while (counter <= 3);

            counter.Should().BeLessOrEqualTo(3);
        }

        [Fact]
        public async Task SetThing_Should_ReturnOk_When_SetWrite()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<bool>();
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    Write = value
                }
            };
            
            _socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            _socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await _socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            do
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await _socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["write"].Type.Should().Be(JTokenType.Boolean);
                    json["data"]["write"].Value<bool>().Should().Be(value);
                    break;
                }
                catch (Exception)
                {
                    counter++;
                }
            } while (counter <= 3);

            counter.Should().BeLessOrEqualTo(3);
        }
        
        [Fact]
        public async Task SetThing_Should_ReturnOk_When_SetWrite2()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<bool>();
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    Write2 = value
                }
            };
            
            _socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            _socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await _socket.SendAsync(Serializer.Serialize(value), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            do
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await _socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["write2"].Type.Should().Be(JTokenType.Boolean);
                    json["data"]["write2"].Value<bool>().Should().Be(value);
                    break;
                }
                catch (Exception e)
                {
                    counter++;
                }
            } while (counter > 3);

            counter.Should().BeLessOrEqualTo(3);
        }
        #endregion

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}
