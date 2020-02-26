using System;
using System.Globalization;
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
    public class ActionType
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly HttpClient _client;
        private readonly Fixture _fixture;
        
        public ActionType()
        {
            _fixture = new Fixture();
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
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
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/action-type/actions/run", 
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
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<Run>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Bool.Should().Be(@bool);
            json.Input.Byte.Should().Be(@byte);
            json.Input.Sbyte.Should().Be(@sbyte);
            json.Input.Short.Should().Be(@short);
            json.Input.UShort.Should().Be(@ushort); 
            json.Input.Int.Should().Be(@int);
            json.Input.Uint.Should().Be(@uint);
            json.Input.Long.Should().Be(@long);
            json.Input.ULong.Should().Be(@ulong);
            json.Input.Double.Should().Be(@double);
            json.Input.Float.Should().Be(@float);
            json.Input.Decimal.Should().Be(@decimal);
            json.Input.String.Should().Be(@string);
            json.Input.DateTime.Should().Be(dateTime);
            json.Input.DateTimeOffset.Should().Be(dateTimeOffset);
            json.Status.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RunWithValidation()
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
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/action-type/actions/runWithValidation", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<Run>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Byte.Should().Be(@byte);
            json.Input.Sbyte.Should().Be(@sbyte);
            json.Input.Short.Should().Be(@short);
            json.Input.UShort.Should().Be(@ushort); 
            json.Input.Int.Should().Be(@int);
            json.Input.Uint.Should().Be(@uint);
            json.Input.Long.Should().Be(@long);
            json.Input.ULong.Should().Be(@ulong);
            json.Input.Double.Should().Be(@double);
            json.Input.Float.Should().Be(@float);
//            json.Input.Decimal.Should().Be(@decimal);
            json.Status.Should().NotBeNullOrEmpty();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithValidationActionMinAndMax(bool isMin)
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
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/action-type/actions/runWithValidation", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<Run>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Byte.Should().Be(@byte);
            json.Input.Sbyte.Should().Be(@sbyte);
            json.Input.Short.Should().Be(@short);
            json.Input.UShort.Should().Be(@ushort); 
            json.Input.Int.Should().Be(@int);
            json.Input.Uint.Should().Be(@uint);
            json.Input.Long.Should().Be(@long);
            json.Input.ULong.Should().Be(@ulong);
            json.Input.Double.Should().Be(@double);
            json.Input.Float.Should().Be(@float);
//            json.Input.Decimal.Should().Be(@decimal);
            json.Status.Should().NotBeNullOrEmpty();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithInvalidation(bool isMin)
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
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/action-type/actions/runWithValidation", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task RunWithValidationExclusiveValid()
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
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/action-type/actions/runWithValidationExclusive", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<Run>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Byte.Should().Be(@byte);
            json.Input.Sbyte.Should().Be(@sbyte);
            json.Input.Short.Should().Be(@short);
            json.Input.UShort.Should().Be(@ushort); 
            json.Input.Int.Should().Be(@int);
            json.Input.Uint.Should().Be(@uint);
            json.Input.Long.Should().Be(@long);
            json.Input.ULong.Should().Be(@ulong);
            json.Input.Double.Should().Be(@double);
            json.Input.Float.Should().Be(@float);
//            json.Input.Decimal.Should().Be(@decimal);
            json.Status.Should().NotBeNullOrEmpty();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithValidationExclusiveMinAndMax(bool isMin)
        {
            var @byte = isMin ? (byte)2 : (byte)99;
            var @sbyte = isMin ? (sbyte)2 : (sbyte)99;
            var @short = isMin ? (short)2 : (short)99;
            var @ushort = isMin ? (ushort)2 : (ushort)99;
            var @int = isMin ? 2 : 99;
            var @uint = isMin ? 2 : (uint)99;
            var @long = isMin ? 2 : (long)99;
            var @ulong = isMin ? 2 : (ulong)99;
            var @double = isMin ? 2 : (double)99;
            var @float = isMin ? 2 : (float)99;
            var @decimal = isMin ? 2 : (decimal)99;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/action-type/actions/runWithValidationExclusive", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<Run>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Byte.Should().Be(@byte);
            json.Input.Sbyte.Should().Be(@sbyte);
            json.Input.Short.Should().Be(@short);
            json.Input.UShort.Should().Be(@ushort); 
            json.Input.Int.Should().Be(@int);
            json.Input.Uint.Should().Be(@uint);
            json.Input.Long.Should().Be(@long);
            json.Input.ULong.Should().Be(@ulong);
            json.Input.Double.Should().Be(@double);
            json.Input.Float.Should().Be(@float);
//            json.Input.Decimal.Should().Be(@decimal);
            json.Status.Should().NotBeNullOrEmpty();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RunWithValidationExclusiveActionInvalid(bool isMin)
        {
            var @byte = isMin ? 1 : 100;
            var @sbyte = isMin ? 1 : 100;
            var @short = isMin ? 1 : 100;
            var @ushort = isMin ? 1 : 100;
            var @int = isMin ? 1 : 100;
            var @uint =isMin ? 1 : 100;
            var @long = isMin ? 1 : 100;
            var @ulong = isMin ? 1 : 100;
            var @double = isMin ? 1 : 100;
            var @float = isMin ? 1 : 100;
            var @decimal = isMin ? 1 : 100;
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/action-type/actions/runWithValidationExclusive", 
                new StringContent($@"
{{ 
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
}}", Encoding.UTF8, "application/json"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

            var response = await _client.PostAsync("/things/action-type/actions/runNull", 
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
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<RunNull>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Bool.Should().Be(@bool);
            json.Input.Byte.Should().Be(@byte);
            json.Input.Sbyte.Should().Be(@sbyte);
            json.Input.Short.Should().Be(@short);
            json.Input.UShort.Should().Be(@ushort); 
            json.Input.Int.Should().Be(@int);
            json.Input.Uint.Should().Be(@uint);
            json.Input.Long.Should().Be(@long);
            json.Input.ULong.Should().Be(@ulong);
            json.Input.Double.Should().Be(@double);
            json.Input.Float.Should().Be(@float);
            json.Input.Decimal.Should().Be(@decimal);
            json.Input.String.Should().Be(@string);
            json.Input.DateTime.Should().Be(dateTime);
            json.Input.DateTimeOffset.Should().Be(dateTimeOffset);
            json.Status.Should().NotBeNullOrEmpty();
        }

        public class Run
        {
            public Input Input { get; set; }
            public string Href { get; set; }
            public string Status { get; set; }
            public DateTime TimeRequested { get; set; }
            public DateTime? TimeCompleted { get; set; }
        }
        
        public class Input
        {
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte Sbyte { get; set; }
            public short Short { get; set; }
            public ushort UShort { get; set; }
            public int Int { get; set; }
            public uint Uint { get; set; }
            public long Long { get; set; }
            public ulong ULong { get; set; }
            public double Double { get; set; }
            public float Float { get; set; }
            public decimal Decimal { get; set; }
            public string String { get; set; }
            public DateTime DateTime { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }

        }
        
        public class RunNull
        {
            public InputNull Input { get; set; }
            public string Href { get; set; }
            public string Status { get; set; }
            public DateTime TimeRequested { get; set; }
            public DateTime? TimeCompleted { get; set; }
        }
        
        public class InputNull
        {
            public bool? Bool { get; set; }
            public byte? Byte { get; set; }
            public sbyte? Sbyte { get; set; }
            public short? Short { get; set; }
            public ushort? UShort { get; set; }
            public int? Int { get; set; }
            public uint? Uint { get; set; }
            public long? Long { get; set; }
            public ulong? ULong { get; set; }
            public double? Double { get; set; }
            public float? Float { get; set; }
            public decimal? Decimal { get; set; }
            public string? String { get; set; }
            public DateTime? DateTime { get; set; }
            public DateTimeOffset? DateTimeOffset { get; set; }
        }
    }
}
