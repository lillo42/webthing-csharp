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
    public class Properties
    {
        private readonly Fixture _fixture;
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly HttpClient _client;
        public Properties()
        {
            _fixture = new Fixture();
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }
        
        #region GET
        [Fact]
        public async Task GetAll()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync("/things/lamp/properties", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse(@"
{
    ""on"": false,
    ""brightness"": 0
}
"));
        }
        
        [Theory]
        [InlineData("on", false)]
        [InlineData("brightness", 0)]
        public async Task Get(string property, object value)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/lamp/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));
        }
        
        [Fact]
        public async Task GetInvalid()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.GetAsync($"/things/lamp/properties/{_fixture.Create<string>()}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region PUT

        [Theory]
        [InlineData("on", true)]
        [InlineData("brightness", 10)]
        public async Task Put(string property, object value)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PutAsync($"/things/lamp/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"), source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));

            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            response = await _client.GetAsync($"/things/lamp/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));
        }

        [Theory]
        [InlineData("brightness", -1, 0)]
        [InlineData("brightness", 101, 0)]
        public async Task PutInvalidValue(string property, object value, object defaultValue)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PutAsync($"/things/lamp/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"), source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync($"/things/lamp/properties/{property}", source.Token)
                .ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {defaultValue.ToString().ToLower()}  }}"));
        }
        
        [Fact]
        public async Task PutInvalidProperty()
        {
            var property = _fixture.Create<string>();
            
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var response = await _client.PutAsync($"/things/lamp/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {_fixture.Create<int>()}  }}"), source.Token);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
