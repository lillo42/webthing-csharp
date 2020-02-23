using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class EventType
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly HttpClient _client;
        private readonly Fixture _fixture;
        
        public EventType()
        {
            _fixture = new Fixture();
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }

        [Fact]
        public async Task GetNoNullable()
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
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/event-type/actions/run", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            response = await _client.GetAsync("/things/event-type/events", source.Token)
                .ConfigureAwait(false);

            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be("application/json");

            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<List<Events>>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            var validBool = json.Where(x => x.Bool != null).Select(x => x.Bool).ToArray();
            validBool.Should().HaveCountGreaterOrEqualTo(1);
            validBool.Should().Contain(x => x.Data == @bool);
            
            var validByte = json.Where(x => x.Byte != null).Select(x=> x.Byte).ToArray();
            validByte.Should().HaveCountGreaterOrEqualTo(1);
            validByte.Should().Contain(x => x.Data == @byte);

            var validSByte = json.Where(x => x.Sbyte != null).Select(x=> x.Sbyte).ToArray();
            validSByte.Should().HaveCountGreaterOrEqualTo(1);
            validSByte.Should().Contain(x => x.Data == @sbyte);
            
            var validShort = json.Where(x => x.Short != null).Select(x=> x.Short).ToArray();
            validShort.Should().HaveCountGreaterOrEqualTo(1);
            validShort.Should().Contain(x => x.Data == @short);
            
            var validUShort = json.Where(x => x.Ushort != null).Select(x=> x.Ushort).ToArray();
            validUShort.Should().HaveCountGreaterOrEqualTo(1);
            validUShort.Should().Contain(x => x.Data == @ushort);
            
            var validInt = json.Where(x => x.Int != null).Select(x=> x.Int).ToArray();
            validInt.Should().HaveCountGreaterOrEqualTo(1);
            validInt.Should().Contain(x => x.Data == @int);
            
            var validUInt = json.Where(x => x.Uint != null).Select(x=> x.Uint).ToArray();
            validUInt.Should().HaveCountGreaterOrEqualTo(1);
            validUInt.Should().Contain(x => x.Data == @uint);
            
            var validLong = json.Where(x => x.Long != null).Select(x=> x.Long).ToArray();
            validLong.Should().HaveCountGreaterOrEqualTo(1);
            validLong.Should().Contain(x => x.Data == @long);
            
            var validULong = json.Where(x => x.Ulong != null).Select(x=> x.Ulong).ToArray();
            validULong.Should().HaveCountGreaterOrEqualTo(1);
            validULong.Should().Contain(x => x.Data == @ulong);

            var validFloat = json.Where(x => x.Float != null).Select(x=> x.Float).ToArray();
            validFloat.Should().HaveCountGreaterOrEqualTo(1);
            validFloat.Should().Contain(x => x.Data == @float);
            
            var validDouble = json.Where(x => x.Double != null).Select(x=> x.Double).ToArray();
            validDouble.Should().HaveCountGreaterOrEqualTo(1);
            validDouble.Should().Contain(x => x.Data == @double);
            
            var validDecimal = json.Where(x => x.Decimal != null).Select(x=> x.Decimal).ToArray();
            validDecimal.Should().HaveCountGreaterOrEqualTo(1);
            validDecimal.Should().Contain(x => x.Data == @decimal);
            
            var validString = json.Where(x => x.String != null).Select(x=> x.String).ToArray();
            validString.Should().HaveCountGreaterOrEqualTo(1);
            validString.Should().Contain(x => x.Data == @string);
            
            var validDateTime = json.Where(x => x.DateTime != null).Select(x=> x.DateTime).ToArray();
            validDateTime.Should().HaveCountGreaterOrEqualTo(1);
            validDateTime.Should().Contain(x => x.Data == dateTime);
            
            var validDateTimeOffset = json.Where(x => x.DateTimeOffset != null).Select(x=> x.DateTimeOffset).ToArray();
            validDateTimeOffset.Should().HaveCountGreaterOrEqualTo(1);
            validDateTimeOffset.Should().Contain(x => x.Data == dateTimeOffset);
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
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/event-type/actions/runNull", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            response = await _client.GetAsync("/things/event-type/events", source.Token)
                .ConfigureAwait(false);

            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be("application/json");
            
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<List<Events>>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            var validBool = json.Where(x => x.NullableBool != null).Select(x => x.NullableBool).ToArray();
            validBool.Should().HaveCountGreaterOrEqualTo(1);
            validBool.Should().Contain(x => x.Data == @bool);
            
            var validByte = json.Where(x => x.NullableByte != null).Select(x=> x.NullableByte).ToArray();
            validByte.Should().HaveCountGreaterOrEqualTo(1);
            validByte.Should().Contain(x => x.Data == @byte);

            var validSByte = json.Where(x => x.NullableSbyte != null).Select(x=> x.NullableSbyte).ToArray();
            validSByte.Should().HaveCountGreaterOrEqualTo(1);
            validSByte.Should().Contain(x => x.Data == @sbyte);
            
            var validShort = json.Where(x => x.NullableShort != null).Select(x=> x.NullableShort).ToArray();
            validShort.Should().HaveCountGreaterOrEqualTo(1);
            validShort.Should().Contain(x => x.Data == @short);
            
            var validUShort = json.Where(x => x.NullableUshort != null).Select(x=> x.NullableUshort).ToArray();
            validUShort.Should().HaveCountGreaterOrEqualTo(1);
            validUShort.Should().Contain(x => x.Data == @ushort);
            
            var validInt = json.Where(x => x.NullableInt != null).Select(x=> x.NullableInt).ToArray();
            validInt.Should().HaveCountGreaterOrEqualTo(1);
            validInt.Should().Contain(x => x.Data == @int);
            
            var validUInt = json.Where(x => x.NullableUint != null).Select(x=> x.NullableUint).ToArray();
            validUInt.Should().HaveCountGreaterOrEqualTo(1);
            validUInt.Should().Contain(x => x.Data == @uint);
            
            var validLong = json.Where(x => x.NullableLong != null).Select(x=> x.NullableLong).ToArray();
            validLong.Should().HaveCountGreaterOrEqualTo(1);
            validLong.Should().Contain(x => x.Data == @long);
            
            var validULong = json.Where(x => x.NullableUlong != null).Select(x=> x.NullableUlong).ToArray();
            validULong.Should().HaveCountGreaterOrEqualTo(1);
            validULong.Should().Contain(x => x.Data == @ulong);

            var validFloat = json.Where(x => x.NullableFloat != null).Select(x=> x.NullableFloat).ToArray();
            validFloat.Should().HaveCountGreaterOrEqualTo(1);
            validFloat.Should().Contain(x => x.Data == @float);
            
            var validDouble = json.Where(x => x.NullableDouble != null).Select(x=> x.NullableDouble).ToArray();
            validDouble.Should().HaveCountGreaterOrEqualTo(1);
            validDouble.Should().Contain(x => x.Data == @double);
            
            var validDecimal = json.Where(x => x.NullableDecimal != null).Select(x=> x.NullableDecimal).ToArray();
            validDecimal.Should().HaveCountGreaterOrEqualTo(1);
            validDecimal.Should().Contain(x => x.Data == @decimal);
            
            var validString = json.Where(x => x.NullableString != null).Select(x=> x.NullableString).ToArray();
            validString.Should().HaveCountGreaterOrEqualTo(1);
            validString.Should().Contain(x => x.Data == @string);
            
            var validDateTime = json.Where(x => x.NullableDateTime != null).Select(x=> x.NullableDateTime).ToArray();
            validDateTime.Should().HaveCountGreaterOrEqualTo(1);
            validDateTime.Should().Contain(x => x.Data == dateTime);
            
            var validDateTimeOffset = json.Where(x => x.NullableDateTimeOffset != null).Select(x=> x.NullableDateTimeOffset).ToArray();
            validDateTimeOffset.Should().HaveCountGreaterOrEqualTo(1);
            validDateTimeOffset.Should().Contain(x => x.Data == dateTimeOffset);
        }
        
        public class Event<T>
        {
            public T Data { get; set; }
            public DateTime Timestamp { get; set; }
        }
        
        public class Events
        {
            public Event<bool> Bool { get; set; }
            public Event<byte> Byte { get; set; }
            public Event<sbyte> Sbyte { get; set; }
            public Event<short> Short { get; set; }
            public Event<ushort> Ushort { get; set; }
            public Event<int> Int { get; set; }
            public Event<uint> Uint { get; set; }
            public Event<long> Long { get; set; }
            public Event<ulong> Ulong { get; set; }
            public Event<double> Double { get; set; }
            public Event<float> Float { get; set; }
            public Event<decimal> Decimal { get; set; }
            public Event<string> String { get; set; }
            public Event<DateTime> DateTime { get; set; }
            public Event<DateTimeOffset> DateTimeOffset { get; set; }
            
            public Event<bool?> NullableBool { get; set; }
            public Event<byte?> NullableByte { get; set; }
            public Event<sbyte?> NullableSbyte { get; set; }
            public Event<short?> NullableShort { get; set; }
            public Event<ushort?> NullableUshort { get; set; }
            public Event<int?> NullableInt { get; set; }
            public Event<uint?> NullableUint { get; set; }
            public Event<long?> NullableLong { get; set; }
            public Event<ulong?> NullableUlong { get; set; }
            public Event<double?> NullableDouble { get; set; }
            public Event<float?> NullableFloat { get; set; }
            public Event<decimal?> NullableDecimal { get; set; }
            public Event<string?> NullableString { get; set; }
            public Event<DateTime?> NullableDateTime { get; set; }
            public Event<DateTimeOffset?> NullableDateTimeOffset { get; set; }

        }
        
    }
}
