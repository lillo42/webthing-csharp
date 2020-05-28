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
    public class Properties : IClassFixture<TestHost>
    {
        private const int s_retries = 3;
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly Uri _baseUrl;

        private readonly WebSocketClient _socketClient;
        private readonly Fixture _fixture;

        public Properties(TestHost testHost)
        {
            _fixture = new Fixture();
            var host = testHost.Host;
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
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            while(true)
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["text"].Type.Should().Be(JTokenType.String);
                    json["data"]["text"].Value<string>().Should().Be(value);
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
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            while(true)
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["level"].Type.Should().Be(JTokenType.Integer);
                    json["data"]["level"].Value<int>().Should().Be(value);
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
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counter = 0;
            while(true)
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("propertyStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["extraInformation"].Type.Should().Be(JTokenType.Array);
                    json["data"]["extraInformation"].Values<string>().Should().BeEquivalentTo(value);
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
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
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
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
        #endregion

        #region Not Found

        [Fact]
        public async Task SetThing_Should_ReturnNotFound_When_PropertyNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<string>();
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    Abc = value
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

        #region Bad Request

        [Fact]
        public async Task SetThingSetModifyViaMethod_Should_ReturnBadRequest()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<bool>();
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    modifyViaMethod = value
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
                    json["data"]["status"].Value<string>().Should().StartWith("400");
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
        public async Task SetThingId_Should_ReturnBadRequest()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<Guid>();
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    Id2 = value
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
                    json["data"]["status"].Value<string>().Should().StartWith("400");
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
        
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public async Task SetThingLevel_Should_ReturnBadRequest_When_ValueIsOutOfRange(int value)
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
                    json["data"]["status"].Value<string>().Should().StartWith("400");
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
        public async Task SetThingSomeText_Should_ReturnBadRequest_When_TextIsNull()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    Text = (string)null
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
                    json["data"]["status"].Value<string>().Should().StartWith("400");
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
        public async Task SetThingExtraInformation_Should_ReturnBadRequest_When_ValueIsNotInEnum()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var message = new WebSocketRequest
            {
                MessageType = "setProperty",
                Data = new
                {
                    ExtraInformation = new[] { _fixture.Create<string>() }
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
                    json["data"]["status"].Value<string>().Should().StartWith("400");
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
    }
}
