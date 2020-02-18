using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class PropertiesType
    {
        private readonly Fixture _fixture;
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30_000);
        private readonly HttpClient _client;
        public PropertiesType()
        {
            _fixture = new Fixture();
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }
        
        #region PUT

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
        public async Task PutNumber(string property, Type type)
        {
            var value = CreateValue(type)?.ToString().ToLower();
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PutAsync($"/things/property-type/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {value ?? "null"}  }}"), source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value}  }}"));

            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            response = await _client.GetAsync($"/things/property-type/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value}  }}"));
        }
        
        [Theory]
        [InlineData("data", typeof(DateTime))]
        [InlineData("dataOffset", typeof(DateTimeOffset))]
        [InlineData("nullableData", typeof(DateTime?))]
        [InlineData("nullableDataOffset", typeof(DateTimeOffset?))]
        [InlineData("text", typeof(string))]
        public async Task PutStringValue(string property, Type type)
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
            

            var response = await _client.PutAsync($"/things/property-type/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {value}  }}"), source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value}  }}"));

            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            response = await _client.GetAsync($"/things/property-type/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value}  }}"));
        }
        

        #endregion

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
