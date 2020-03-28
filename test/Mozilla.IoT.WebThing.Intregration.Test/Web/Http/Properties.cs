using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Mozilla.IoT.WebThing.Intregration.Test.Web.Things;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Intregration.Test.Web.Http
{
    public class Properties
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly Fixture _fixture;
        private readonly HttpClient _client;
        
        public Properties()
        {
            _fixture = new Fixture();
            
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }

        [Fact]
        public async Task ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            var response = await _client.GetAsync($"/things/{_fixture.Create<string>()}/properties", 
                source.Token).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetProperties()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            var response = await _client.GetAsync($"/things/{ImmutablePropertyThing.NAME}/properties", 
                source.Token).ConfigureAwait(false);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be("application/json");
            
            var message = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var json = JToken.Parse(message);
            json.Type.Should().Be(JTokenType.Object);
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse(@"
{
    ""isEnable"": false,
    ""level"": 10,
    ""value"": ""test"" 
}
"));
        }
    }
}
