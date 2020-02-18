using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class PropertiesEnumType
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30_000);
        private readonly HttpClient _client;
        public PropertiesEnumType()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }
        
        #region PUT

        [Theory]
        [InlineData("numberByte", 0)]
        [InlineData("numberByte", byte.MaxValue)]
        [InlineData("numberByte", byte.MinValue)]
        [InlineData("numberSByte", 0)]
        [InlineData("numberSByte", sbyte.MaxValue)]
        [InlineData("numberSByte", sbyte.MinValue)]
        [InlineData("numberShort", 0)]
        [InlineData("numberShort", short.MaxValue)]
        [InlineData("numberShort", short.MinValue)]
        [InlineData("numberUShort", 0)]
        [InlineData("numberUShort", ushort.MaxValue)]
        [InlineData("numberUShort", ushort.MinValue)]
        [InlineData("numberInt", 0)]
        [InlineData("numberInt", int.MaxValue)]
        [InlineData("numberInt", int.MinValue)]
        [InlineData("numberUInt", 0)]
        [InlineData("numberUInt", uint.MaxValue)]
        [InlineData("numberUInt", uint.MinValue)]
        [InlineData("numberLong", 0)]
        [InlineData("numberLong", long.MaxValue)]
        [InlineData("numberLong", long.MinValue)]
        [InlineData("numberULong", 0)]
        [InlineData("numberULong", ulong.MaxValue)]
        [InlineData("numberULong", ulong.MinValue)]
        [InlineData("numberDouble", 0)]
        [InlineData("numberDouble", double.MaxValue)]
        [InlineData("numberDouble", double.MinValue)]
        [InlineData("numberFloat", 0)]
        [InlineData("numberFloat", float.MaxValue)]
        [InlineData("numberFloat", float.MinValue)]
        [InlineData("bool", true)]
        [InlineData("bool", false)]
        public async Task PutNumber(string property, object value)
        {
            value = value.ToString().ToLower();
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PutAsync($"/things/property-enum-type/properties/{property}", 
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

            
            response = await _client.GetAsync($"/things/property-enum-type/properties/{property}", source.Token)
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
        [InlineData("text", "ola")]
        [InlineData("text", "ass")]
        [InlineData("text", "aaa")]
        public async Task PutStringValue(string property, string value)
        {

            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PutAsync($"/things/property-enum-type/properties/{property}", 
                new StringContent($@"{{ ""{property}"": ""{value}""  }}"), source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": ""{value}""  }}"));

            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            response = await _client.GetAsync($"/things/property-enum-type/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": ""{value}""  }}"));
        }
        

        #endregion
    }
}
