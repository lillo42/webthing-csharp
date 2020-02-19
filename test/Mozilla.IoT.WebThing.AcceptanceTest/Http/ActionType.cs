using System;
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
        public async Task Create()
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
            var json = JsonConvert.DeserializeObject<Fade>(message, new JsonSerializerSettings
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
        
        public class Fade
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
    }
}
