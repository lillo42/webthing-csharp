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
    public class Actions : IClassFixture<TestHost>
    {
        private const int s_retries = 3;
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly Uri _baseUrl;

        private readonly WebSocketClient _socketClient;
        private readonly Fixture _fixture;

        public Actions(TestHost testHost)
        {
            _fixture = new Fixture();
            var host = testHost.Host;
            _baseUrl = new UriBuilder(host.GetTestServer().BaseAddress) {Scheme = "ws", Path = "/things/web-socket-action-thing/"}.Uri;
            _socketClient = host.GetTestServer().CreateWebSocketClient();
        }
        
        #region Not Found
        
        [Fact]
        public async Task RequestAction_Should_ReturnNotFound_When_ActionNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var message = new WebSocketRequest
            {
                MessageType = "requestAction",
                Data = new
                {
                    Abc = new
                    {
                        Input = new object()
                    }
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

        [Theory]
        [InlineData(null, 2, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("", 1, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("Foo", 100, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("Bar", 50, "c01bff3e-aed4-480a-89cd-20caa40c8468")]
        public async Task RequestAction_Should_ReturnBadRequest_When_InputIsInvalid(string value, int level, string id)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var message = new WebSocketRequest
            {
                MessageType = "requestAction",
                Data = new
                {
                    WithRestriction = new
                    {
                        Input = new
                        {
                            Value = value,
                            Level = level,
                            Active = _fixture.Create<bool>(),
                            Id = Guid.Parse(id)
                        }
                    }
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

        #region Ok

        [Fact]
        public async Task RequestNoRestrictionAction_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var level = _fixture.Create<int>();
            var active = _fixture.Create<bool>();
            var id = _fixture.Create<Guid>();
            
            var message = new WebSocketRequest
            {
                MessageType = "requestAction",
                Data = new
                {
                    NoRestriction = new
                    {
                        Input = new
                        {
                            Value = value,
                            Level = level,
                            Active = active,
                            Id = id
                        }
                    }
                }
            };
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counterError = 0;
            var status = 0;
            while (status < 3) 
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("actionStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["noRestriction"].Type.Should().Be(JTokenType.Object);
                    json["data"]["noRestriction"]["input"].Type.Should().Be(JTokenType.Object);
                    
                    json["data"]["noRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
                    json["data"]["noRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
                    json["data"]["noRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
                    json["data"]["noRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
                    json["data"]["noRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
                    json["data"]["noRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToString().ToUpper());
            
                    json["data"]["noRestriction"]["status"].Type.Should().Be(JTokenType.String);
                    json["data"]["noRestriction"]["href"].Type.Should().Be(JTokenType.String);

                    if (status == 0)
                    {
                        json["data"]["noRestriction"]["status"].Value<string>().Should().BeOneOf("created");
                        status++;
                        counterError = 0;
                    }
                    
                    if (status == 1)
                    {
                        json["data"]["noRestriction"]["status"].Value<string>().Should().BeOneOf("pending");
                        status++;
                        counterError = 0;
                    }
                    
                    if (status == 2)
                    {
                        json["data"]["noRestriction"]["status"].Value<string>().Should().BeOneOf("completed");
                        status++;
                        counterError = 0;
                    }
                }
                catch (Exception)
                {
                    counterError++;

                    if (counterError >= s_retries)
                    {
                        throw;
                    }
                }
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
        
        [Theory]
        [InlineData(2, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData(99, "a8e3202d-7eaa-4889-a5cf-ec44275414eb")]
        public async Task RequestWithRestrictionAction_Should_ReturnOk(int level, string id)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var active = _fixture.Create<bool>();
            
            var message = new WebSocketRequest
            {
                MessageType = "requestAction",
                Data = new
                {
                    WithRestriction = new
                    {
                        Input = new
                        {
                            Value = value,
                            Level = level,
                            Active = active,
                            Id = Guid.Parse(id)
                        }
                    }
                }
            };
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counterError = 0;
            var status = 0;
            while (status < 3) 
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("actionStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["withRestriction"].Type.Should().Be(JTokenType.Object);
                    json["data"]["withRestriction"]["input"].Type.Should().Be(JTokenType.Object);
                    
                    json["data"]["withRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
                    json["data"]["withRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
                    json["data"]["withRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
                    json["data"]["withRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
                    json["data"]["withRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
                    json["data"]["withRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToString().ToUpper());
            
                    json["data"]["withRestriction"]["status"].Type.Should().Be(JTokenType.String);
                    json["data"]["withRestriction"]["href"].Type.Should().Be(JTokenType.String);

                    if (status == 0)
                    {
                        json["data"]["withRestriction"]["status"].Value<string>().Should().BeOneOf("created");
                        status++;
                        counterError = 0;
                    }
                    
                    if (status == 1)
                    {
                        json["data"]["withRestriction"]["status"].Value<string>().Should().BeOneOf("pending");
                        status++;
                        counterError = 0;
                    }
                    
                    if (status == 2)
                    {
                        json["data"]["withRestriction"]["status"].Value<string>().Should().BeOneOf("completed");
                        status++;
                        counterError = 0;
                    }
                }
                catch (Exception)
                {
                    counterError++;

                    if (counterError >= s_retries)
                    {
                        throw;
                    }
                }
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        [Fact]
        public async Task RequestLongRunningAction_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            

            var message = new WebSocketRequest
            {
                MessageType = "requestAction",
                Data = new
                {
                    LongRunning = new
                    {
                        Input = new object()
                    }
                }
            };
            
            using var socket = await _socketClient.ConnectAsync(_baseUrl, source.Token);
            socket.State.Should().Be(WebSocketState.Open);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            await socket.SendAsync(Serializer.Serialize(message), WebSocketMessageType.Text, true, source.Token);
            
            var counterError = 0;
            var status = 0;
            while (status < 3) 
            {
                source = new CancellationTokenSource();
                source.CancelAfter(s_timeout);

                try
                {
                    var array = new ArraySegment<byte>(new byte[4096]);
                    var result =  await socket.ReceiveAsync(array, source.Token);
                    var json = Serializer.Deserialize(array, result);
                    json["messageType"].Value<string>().Should().Be("actionStatus");
                    json["data"].Type.Should().Be(JTokenType.Object);
                    json["data"]["longRunning"].Type.Should().Be(JTokenType.Object);
                    
                    json["data"]["longRunning"]["status"].Type.Should().Be(JTokenType.String);
                    json["data"]["longRunning"]["href"].Type.Should().Be(JTokenType.String);

                    if (status == 0)
                    {
                        json["data"]["longRunning"]["status"].Value<string>().Should().BeOneOf("created");
                        status++;
                        counterError = 0;
                    }
                    
                    if (status == 1)
                    {
                        json["data"]["longRunning"]["status"].Value<string>().Should().BeOneOf("pending");
                        status++;
                        counterError = 0;
                    }
                    
                    if (status == 2)
                    {
                        json["data"]["longRunning"]["status"].Value<string>().Should().BeOneOf("completed");
                        status++;
                        counterError = 0;
                    }
                }
                catch (Exception)
                {
                    counterError++;

                    if (counterError >= s_retries)
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
