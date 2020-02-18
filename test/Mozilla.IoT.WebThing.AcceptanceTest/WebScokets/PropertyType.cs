using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.WebScokets
{
    public class PropertyType
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly Fixture _fixture;
        private readonly WebSocketClient _webSocketClient;
        private readonly HttpClient _client;
        private readonly Uri _uri;
        
        public PropertyType()
        {
            _fixture = new Fixture();
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
            _webSocketClient = host.GetTestServer().CreateWebSocketClient();

            _uri =  new UriBuilder(_client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/web-socket-property-type"
            }.Uri;
        }
        
        [Theory]
        [InlineData("numberByte", typeof(byte))]
        [InlineData("numberSByte", typeof(sbyte))]
        [InlineData("numberShort", typeof(short))]
        [InlineData("numberUShort", typeof(ushort))]
        [InlineData("numberInt", typeof(int))]
        [InlineData("numberUInt", typeof(uint))]
        [InlineData("numberLong", typeof(long))]
        [InlineData("numberULong", typeof(ulong))]
        [InlineData("numberDouble", typeof(double))]
        [InlineData("numberFloat", typeof(float))]
        [InlineData("numberDecimal", typeof(decimal))]
        [InlineData("bool", typeof(bool))] 
        [InlineData("nullableBool", typeof(bool?))] 
        [InlineData("nullableByte", typeof(byte?))]
        [InlineData("nullableSByte", typeof(sbyte?))]
        [InlineData("nullableShort", typeof(short?))]
        [InlineData("nullableUShort", typeof(ushort?))]
        [InlineData("nullableInt", typeof(int?))]
        [InlineData("nullableUInt", typeof(uint?))]
        [InlineData("nullableLong", typeof(long?))]
        [InlineData("nullableULong", typeof(ulong?))]
        [InlineData("nullableDouble", typeof(double?))]
        [InlineData("nullableFloat", typeof(float?))]
        [InlineData("nullableDecimal", typeof(decimal?))]
        public async Task SetProperties(string property, Type type)
        {
            var value = CreateValue(type)?.ToString().ToLower();
            
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
            
            var response = await _client.GetAsync($"/things/web-socket-property-type/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value?.ToString().ToLower()}  }}"));
        }
        
        
        [Theory]
        [InlineData("data", typeof(DateTime))]
        [InlineData("dataOffset", typeof(DateTimeOffset))]
        [InlineData("nullableData", typeof(DateTime?))]
        [InlineData("nullableDataOffset", typeof(DateTimeOffset?))]
        [InlineData("text", typeof(string))]
        public async Task SetPropertiesStringValue(string property, Type type)
        {
            var value = CreateValue(type);
            
            if (value != null && (type == typeof(DateTime)
                                  || type == typeof(DateTime?)))
            {
                value = ((DateTime)value).ToString("O");
            }
            
            if (value != null && (type == typeof(DateTimeOffset)
                                  || type == typeof(DateTimeOffset?)))
            {
                value = ((DateTimeOffset)value).ToString("O");
            }
            
            value = value != null ? $"\"{value}\"" : "null";

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
            
            var response = await _client.GetAsync($"/things/web-socket-property-type/properties/{property}", source.Token)
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
        
        
        private object CreateValue(Type type)
        {
            if (type == typeof(bool))
            {
                return _fixture.Create<bool>();
            }
            
            if (type == typeof(bool?))
            {
                return _fixture.Create<bool?>();
            }
            
            if (type == typeof(byte))
            {
                return _fixture.Create<byte>();
            }
            
            if (type == typeof(byte?))
            {
                return _fixture.Create<byte?>();
            }
            
            if (type == typeof(sbyte))
            {
                return _fixture.Create<sbyte>();
            }
            
            if (type == typeof(sbyte?))
            {
                return _fixture.Create<sbyte?>();
            }
            
            if (type == typeof(short))
            {
                return _fixture.Create<short>();
            }
            
            if (type == typeof(short?))
            {
                return _fixture.Create<short?>();
            }
            
            if (type == typeof(ushort))
            {
                return _fixture.Create<ushort>();
            }
            
            if (type == typeof(ushort?))
            {
                return _fixture.Create<ushort?>();
            }
            
            if (type == typeof(int))
            {
                return _fixture.Create<int>();
            }
            
            if (type == typeof(int?))
            {
                return _fixture.Create<int?>();
            }
            
            if (type == typeof(uint))
            {
                return _fixture.Create<uint>();
            }
            
            if (type == typeof(uint?))
            {
                return _fixture.Create<uint?>();
            }
            
            if (type == typeof(long))
            {
                return _fixture.Create<long>();
            }
            
            if (type == typeof(long?))
            {
                return _fixture.Create<long?>();
            }
            
            if (type == typeof(ulong))
            {
                return _fixture.Create<ulong>();
            }

            if (type == typeof(ulong?))
            {
                return _fixture.Create<ulong?>();
            }
            
            if (type == typeof(double))
            {
                return _fixture.Create<double>();
            }
            
            if (type == typeof(double?))
            {
                return _fixture.Create<double?>();
            }
            
            if (type == typeof(float))
            {
                return _fixture.Create<float>();
            }
            
            if (type == typeof(float?))
            {
                return _fixture.Create<float?>();
            }
            
            if (type == typeof(decimal))
            {
                return _fixture.Create<decimal>();
            }
            
            if (type == typeof(decimal?))
            {
                return _fixture.Create<decimal?>();
            }
            
            if (type == typeof(DateTime))
            {
                return _fixture.Create<DateTime>();
            }
            
            if (type == typeof(DateTime?))
            {
                return _fixture.Create<DateTime?>();
            }

            if (type == typeof(DateTimeOffset))
            {
                return  _fixture.Create<DateTimeOffset>();
            }
            
            if (type == typeof(DateTimeOffset?))
            {
                return  _fixture.Create<DateTimeOffset?>();
            }

            return  _fixture.Create<string>();
        }
    }
}
