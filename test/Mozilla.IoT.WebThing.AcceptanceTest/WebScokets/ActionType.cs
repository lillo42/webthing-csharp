using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.WebScokets
{
    public class ActionType
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly Fixture _fixture;

        public ActionType()
        {
            _fixture = new Fixture();
        }
        
        [Fact]
        public async Task RunAction()
        {
            var @bool = _fixture.Create<bool>();
            var @byte = _fixture.Create<byte>();
            var @sbyte = _fixture.Create<sbyte>();
            var @short = _fixture.Create<short>();
            var @ushort = _fixture.Create<ushort>();
            var @int = _fixture.Create<int>();
            var @uint = _fixture.Create<uint>();
            var @long = _fixture.Create<long>();
            var @ulong = _fixture.Create<ulong>();
            var @double = _fixture.Create<double>();
            var @float = _fixture.Create<float>();
            var @decimal = _fixture.Create<decimal>();
            var @string = _fixture.Create<string>();
            var @dateTime = _fixture.Create<DateTime>();
            var @dateTimeOffset = _fixture.Create<DateTimeOffset>();
            
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""run"": {{
            ""input"": {{
                ""bool"": {@bool.ToString().ToLower()},
                ""byte"": {@byte},
                ""sbyte"": {@sbyte},
                ""short"": {@short},
                ""ushort"": {@ushort},
                ""int"": {@int},
                ""uint"": {@uint},
                ""long"": {@long},
                ""ulong"": {@ulong},
                ""double"": {@double},
                ""float"": {@float},
                ""decimal"": {@decimal},
                ""string"": ""{@string}"",
                ""dateTime"": ""{@dateTime:O}"",
                ""dateTimeOffset"": ""{@dateTimeOffset:O}""
            }}
        }}
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

            var json = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(segment.Slice(0, result.Count)),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.Run.Input.Bool.Should().Be(@bool);
            json.Data.Run.Input.Byte.Should().Be(@byte);
            json.Data.Run.Input.Sbyte.Should().Be(@sbyte);
            json.Data.Run.Input.Short.Should().Be(@short);
            json.Data.Run.Input.UShort.Should().Be(@ushort); 
            json.Data.Run.Input.Int.Should().Be(@int);
            json.Data.Run.Input.Uint.Should().Be(@uint);
            json.Data.Run.Input.Long.Should().Be(@long);
            json.Data.Run.Input.ULong.Should().Be(@ulong);
            json.Data.Run.Input.Double.Should().Be(@double);
            json.Data.Run.Input.Float.Should().Be(@float);
            json.Data.Run.Input.Decimal.Should().Be(@decimal);
            json.Data.Run.Input.String.Should().Be(@string);
            json.Data.Run.Input.DateTime.Should().Be(dateTime);
            json.Data.Run.Input.DateTimeOffset.Should().Be(dateTimeOffset);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunNullAction(bool isNull)
        {
            var @bool = isNull ? null : new bool?(_fixture.Create<bool>());
            var @byte = isNull ? null : new byte?(_fixture.Create<byte>());
            var @sbyte = isNull ? null : new sbyte?(_fixture.Create<sbyte>());
            var @short = isNull ? null : new short?(_fixture.Create<short>());
            var @ushort = isNull ? null : new ushort?(_fixture.Create<ushort>());
            var @int = isNull ? null : new int?(_fixture.Create<int>());
            var @uint = isNull ? null : new uint?(_fixture.Create<uint>());
            var @long = isNull ? null : new long?(_fixture.Create<long>());
            var @ulong = isNull ? null : new ulong?(_fixture.Create<ulong>());
            var @double = isNull ? null : new double?(_fixture.Create<double>());
            var @float = isNull ? null : new float?(_fixture.Create<float>());
            var @decimal = isNull ? null : new decimal?(_fixture.Create<decimal>());
            var @string = isNull ? null : _fixture.Create<string>();
            var @dateTime = isNull ? null : new  DateTime?(_fixture.Create<DateTime>());
            var @dateTimeOffset = isNull ? null : new DateTimeOffset?(_fixture.Create<DateTimeOffset>());
            
            var @boolS = isNull ? "null" : @bool.ToString().ToLower();
            var @byteS = isNull ? "null" : @byte.ToString();
            var @sbyteS = isNull ? "null" : @sbyte.ToString();
            var @shortS = isNull ? "null" : @short.ToString();
            var @ushortS = isNull ? "null" : @ushort.ToString();
            var @intS = isNull ? "null" : @int.ToString();
            var @uintS = isNull ? "null" : @uint.ToString();
            var @longS = isNull ? "null" : @long.ToString();
            var @ulongS = isNull ? "null" : @ulong.ToString();
            var @doubleS = isNull ? "null" : @double.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
            var @floatS = isNull ? "null" : @float.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
            var @decimalS = isNull ? "null" : @decimal.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
            var @stringS = isNull ? "null" : $"\"{@string}\"";
            var @dateTimeS = isNull ? "null" : $"\"{@dateTime:O}\"";
            var @dateTimeOffsetS = isNull ? "null" : $"\"{@dateTimeOffset:O}\"";
            
            
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runNull"": {{
            ""input"": {{
                ""bool"": {@boolS},
                ""byte"": {@byteS},
                ""sbyte"": {@sbyteS},
                ""short"": {@shortS},
                ""ushort"": {@ushortS},
                ""int"": {@intS},
                ""uint"": {@uintS},
                ""long"": {@longS},
                ""ulong"": {@ulongS},
                ""double"": {@doubleS},
                ""float"": {@floatS},
                ""decimal"": {@decimalS},
                ""string"": {@stringS},
                ""dateTime"": {@dateTimeS},
                ""dateTimeOffset"": {@dateTimeOffsetS}
            }}
        }}
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

            var json = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(segment.Slice(0, result.Count)),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.RunNull.Input.Bool.Should().Be(@bool);
            json.Data.RunNull.Input.Byte.Should().Be(@byte);
            json.Data.RunNull.Input.Sbyte.Should().Be(@sbyte);
            json.Data.RunNull.Input.Short.Should().Be(@short);
            json.Data.RunNull.Input.UShort.Should().Be(@ushort); 
            json.Data.RunNull.Input.Int.Should().Be(@int);
            json.Data.RunNull.Input.Uint.Should().Be(@uint);
            json.Data.RunNull.Input.Long.Should().Be(@long);
            json.Data.RunNull.Input.ULong.Should().Be(@ulong);
            json.Data.RunNull.Input.Double.Should().Be(@double);
            json.Data.RunNull.Input.Float.Should().Be(@float);
            json.Data.RunNull.Input.Decimal.Should().Be(@decimal);
            json.Data.RunNull.Input.String.Should().Be(@string);
            json.Data.RunNull.Input.DateTime.Should().Be(dateTime);
            json.Data.RunNull.Input.DateTimeOffset.Should().Be(dateTimeOffset);
        }
        
        [Fact]
        public async Task RunWithValidationAction()
        {
            var @byte = (byte)10;
            var @sbyte = (sbyte)10;
            var @short = (short)10;
            var @ushort = (ushort)10;
            var @int = 10;
            var @uint = (uint)10;
            var @long = (long)10;
            var @ulong = (ulong)10;
            var @double = (double)10;
            var @float = (float)10;
            var @decimal = (decimal)10;
            
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidation"": {{
            ""input"": {{
                ""byte"": {@byte},
                ""sbyte"": {@sbyte},
                ""short"": {@short},
                ""ushort"": {@ushort},
                ""int"": {@int},
                ""uint"": {@uint},
                ""long"": {@long},
                ""ulong"": {@ulong},
                ""double"": {@double},
                ""float"": {@float},
                ""decimal"": {@decimal}
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.RunWithValidation.Input.Byte.Should().Be(@byte);
            json.Data.RunWithValidation.Input.Sbyte.Should().Be(@sbyte);
            json.Data.RunWithValidation.Input.Short.Should().Be(@short);
            json.Data.RunWithValidation.Input.UShort.Should().Be(@ushort); 
            json.Data.RunWithValidation.Input.Int.Should().Be(@int);
            json.Data.RunWithValidation.Input.Uint.Should().Be(@uint);
            json.Data.RunWithValidation.Input.Long.Should().Be(@long);
            json.Data.RunWithValidation.Input.ULong.Should().Be(@ulong);
            json.Data.RunWithValidation.Input.Double.Should().Be(@double);
            json.Data.RunWithValidation.Input.Float.Should().Be(@float);
           // json.Data.RunNullVWithValidation.Input.Decimal.Should().Be(@decimal);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithValidationWithMinAndMax(bool isMin)
        {
            var @byte = isMin ? (byte)1 : (byte)100;
            var @sbyte = isMin ? (sbyte)1 : (sbyte)100;
            var @short = isMin ? (short)1 : (short)100;
            var @ushort = isMin ? (ushort)1 : (ushort)100;
            var @int = isMin ? (int)1 : 100;
            var @uint = isMin ? 1 : (uint)100;
            var @long = isMin ? 1 : (long)100;
            var @ulong = isMin ? 1 : (ulong)100;
            var @double = isMin ? 1 : (double)100;
            var @float = isMin ? 1 : (float)100;
            var @decimal = isMin ? 1 : (decimal)100;
            
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidation"": {{
            ""input"": {{
                ""byte"": {@byte},
                ""sbyte"": {@sbyte},
                ""short"": {@short},
                ""ushort"": {@ushort},
                ""int"": {@int},
                ""uint"": {@uint},
                ""long"": {@long},
                ""ulong"": {@ulong},
                ""double"": {@double},
                ""float"": {@float},
                ""decimal"": {@decimal}
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.RunWithValidation.Input.Byte.Should().Be(@byte);
            json.Data.RunWithValidation.Input.Sbyte.Should().Be(@sbyte);
            json.Data.RunWithValidation.Input.Short.Should().Be(@short);
            json.Data.RunWithValidation.Input.UShort.Should().Be(@ushort); 
            json.Data.RunWithValidation.Input.Int.Should().Be(@int);
            json.Data.RunWithValidation.Input.Uint.Should().Be(@uint);
            json.Data.RunWithValidation.Input.Long.Should().Be(@long);
            json.Data.RunWithValidation.Input.ULong.Should().Be(@ulong);
            json.Data.RunWithValidation.Input.Double.Should().Be(@double);
            json.Data.RunWithValidation.Input.Float.Should().Be(@float);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithInvalidationWithMinAndMax(bool isMin)
        {
            var @byte = isMin ? (byte)0 : (byte)101;
            var @sbyte = isMin ? (sbyte)0 : (sbyte)101;
            var @short = isMin ? (short)0 : (short)101;
            var @ushort = isMin ? (ushort)0 : (ushort)101;
            var @int = isMin ? 0 : 101;
            var @uint = isMin ? 0 : (uint)101;
            var @long = isMin ? 0 : (long)101;
            var @ulong = isMin ? 0 : (ulong)101;
            var @double = isMin ? 0 : (double)101;
            var @float = isMin ? 0 : (float)101;
            var @decimal = isMin ? 0 : (decimal)101;
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidation"": {{
            ""input"": {{
                ""byte"": {@byte},
                ""sbyte"": {@sbyte},
                ""short"": {@short},
                ""ushort"": {@ushort},
                ""int"": {@int},
                ""uint"": {@uint},
                ""long"": {@long},
                ""ulong"": {@ulong},
                ""double"": {@double},
                ""float"": {@float},
                ""decimal"": {@decimal}
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("error");
            json.Data.Status.Should().Be("400 Bad Request");
            json.Data.Message.Should().Be("Invalid action request");
        }
        
        
        [Fact]
        public async Task RunWithValidationExclusive()
        {
            var @byte = (byte)10;
            var @sbyte = (sbyte)10;
            var @short = (short)10;
            var @ushort = (ushort)10;
            var @int = 10;
            var @uint = (uint)10;
            var @long = (long)10;
            var @ulong = (ulong)10;
            var @double = (double)10;
            var @float = (float)10;
            var @decimal = (decimal)10;
            
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidationExclusive"": {{
            ""input"": {{
                ""byte"": {@byte},
                ""sbyte"": {@sbyte},
                ""short"": {@short},
                ""ushort"": {@ushort},
                ""int"": {@int},
                ""uint"": {@uint},
                ""long"": {@long},
                ""ulong"": {@ulong},
                ""double"": {@double},
                ""float"": {@float},
                ""decimal"": {@decimal}
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.RunWithValidationExclusive.Input.Byte.Should().Be(@byte);
            json.Data.RunWithValidationExclusive.Input.Sbyte.Should().Be(@sbyte);
            json.Data.RunWithValidationExclusive.Input.Short.Should().Be(@short);
            json.Data.RunWithValidationExclusive.Input.UShort.Should().Be(@ushort); 
            json.Data.RunWithValidationExclusive.Input.Int.Should().Be(@int);
            json.Data.RunWithValidationExclusive.Input.Uint.Should().Be(@uint);
            json.Data.RunWithValidationExclusive.Input.Long.Should().Be(@long);
            json.Data.RunWithValidationExclusive.Input.ULong.Should().Be(@ulong);
            json.Data.RunWithValidationExclusive.Input.Double.Should().Be(@double);
            json.Data.RunWithValidationExclusive.Input.Float.Should().Be(@float);
           // json.Data.RunNullVWithValidation.Input.Decimal.Should().Be(@decimal);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithValidationWithExclusiveMinAndMax(bool isMin)
        {
            var @byte = isMin ? (byte)2 : (byte)99;
            var @sbyte = isMin ? (sbyte)2 : (sbyte)99;
            var @short = isMin ? (short)2 : (short)99;
            var @ushort = isMin ? (ushort)2 : (ushort)99;
            var @int = isMin ? (int)2 : 99;
            var @uint = isMin ? 2 : (uint)99;
            var @long = isMin ? 2 : (long)99;
            var @ulong = isMin ? 2 : (ulong)99;
            var @double = isMin ? 2 : (double)99;
            var @float = isMin ? 2 : (float)99;
            var @decimal = isMin ? 2 : (decimal)99;
            
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidationExclusive"": {{
            ""input"": {{
                ""byte"": {@byte},
                ""sbyte"": {@sbyte},
                ""short"": {@short},
                ""ushort"": {@ushort},
                ""int"": {@int},
                ""uint"": {@uint},
                ""long"": {@long},
                ""ulong"": {@ulong},
                ""double"": {@double},
                ""float"": {@float},
                ""decimal"": {@decimal}
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.RunWithValidationExclusive.Input.Byte.Should().Be(@byte);
            json.Data.RunWithValidationExclusive.Input.Sbyte.Should().Be(@sbyte);
            json.Data.RunWithValidationExclusive.Input.Short.Should().Be(@short);
            json.Data.RunWithValidationExclusive.Input.UShort.Should().Be(@ushort); 
            json.Data.RunWithValidationExclusive.Input.Int.Should().Be(@int);
            json.Data.RunWithValidationExclusive.Input.Uint.Should().Be(@uint);
            json.Data.RunWithValidationExclusive.Input.Long.Should().Be(@long);
            json.Data.RunWithValidationExclusive.Input.ULong.Should().Be(@ulong);
            json.Data.RunWithValidationExclusive.Input.Double.Should().Be(@double);
            json.Data.RunWithValidationExclusive.Input.Float.Should().Be(@float);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithInvalidationWithExclusiveMinAndMax(bool isMin)
        {
            var @byte = isMin ? (byte)1 : (byte)100;
            var @sbyte = isMin ? (sbyte)1 : (sbyte)100;
            var @short = isMin ? (short)1 : (short)100;
            var @ushort = isMin ? (ushort)1 : (ushort)100;
            var @int = isMin ? 1 : 100;
            var @uint = isMin ? 1 : (uint)100;
            var @long = isMin ? 1 : (long)100;
            var @ulong = isMin ? 1 : (ulong)100;
            var @double = isMin ? 1 : (double)100;
            var @float = isMin ? 1 : (float)100;
            var @decimal = isMin ? 1 : (decimal)100;
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidationExclusive"": {{
            ""input"": {{
                ""byte"": {@byte},
                ""sbyte"": {@sbyte},
                ""short"": {@short},
                ""ushort"": {@ushort},
                ""int"": {@int},
                ""uint"": {@uint},
                ""long"": {@long},
                ""ulong"": {@ulong},
                ""double"": {@double},
                ""float"": {@float},
                ""decimal"": {@decimal}
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("error");
            json.Data.Status.Should().Be("400 Bad Request");
            json.Data.Message.Should().Be("Invalid action request");
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithStringValidationValid(bool isMin)
        {
            var min = _fixture.Create<string>();
            var email = "test@gmail.com";
            
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidationExclusive"": {{
            ""input"": {{
                ""minAnMax"": ""{min}"",
                ""mail"": ""{email}""
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("actionStatus");
            json.Data.RunWithStringValidation.Input.MinAnMax.Should().Be(min);
            json.Data.RunWithStringValidation.Input.Mail.Should().Be(email);
        }
        
        
        [Theory]
        [InlineData(null, "test@tese.com")]
        [InlineData("", "test@tese.com")]
        [InlineData("abc", null)]
        [InlineData("abc", "test")]
        public async Task RunWithStringValidationInvalid(string min, string email)
        {
            
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/action-type"
            }.Uri;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var socket = await webSocketClient.ConnectAsync(uri, source.Token)
                .ConfigureAwait(false);
            
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""requestAction"",
    ""data"": {{
        ""runWithValidationExclusive"": {{
            ""input"": {{
                ""minAnMax"": ""{min}"",
                ""mail"": ""{email}""
            }}
        }}
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

            var message = Encoding.UTF8.GetString(segment.Slice(0, result.Count));
            var json = JsonConvert.DeserializeObject<Message>(message, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
           
            json.MessageType.Should().Be("error");
            json.Data.Status.Should().Be("400 Bad Request");
            json.Data.Message.Should().Be("Invalid action request");
        }
        
        
        public class Message
        {
            public string MessageType { get; set; }
            public ActionSocket Data { get; set; }
        }

        public class ActionSocket
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public Http.ActionType.Run Run { get; set; }
            public Http.ActionType.RunNull RunNull { get; set; }
            
            public Http.ActionType.Run RunWithValidation { get; set; }
            public Http.ActionType.Run RunWithValidationExclusive { get; set; }
            
            
            public Http.ActionType.RunString RunWithStringValidation { get; set; }
        }
            
    }
}
