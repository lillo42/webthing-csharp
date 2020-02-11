using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class Properties
    {
        private readonly Fixture _fixture;

        public Properties()
        {
            _fixture = new Fixture();
        }
        
        #region GET
        [Fact]
        public async Task GetAll()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync("/things/Lamp/properties");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
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
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync($"/things/Lamp/properties/{property}");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                    .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));
        }
        
        [Fact]
        public async Task GetInvalid()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync($"/things/Lamp/properties/{_fixture.Create<string>()}");
            
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
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.PutAsync($"/things/Lamp/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));

            response = await client.GetAsync($"/things/Lamp/properties/{property}");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            message = await response.Content.ReadAsStringAsync();
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));
        }

        [Theory]
        [InlineData("brightness", -1, 0)]
        [InlineData("brightness", 101, 0)]
        public async Task PutInvalidValue(string property, object value, object defaulValue)
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.PutAsync($"/things/Lamp/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {value.ToString().ToLower()}  }}"));
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            response = await client.GetAsync($"/things/Lamp/properties/{property}");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Object);
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(json)
                .BeEquivalentTo(JToken.Parse($@"{{ ""{property}"": {defaulValue.ToString().ToLower()}  }}"));
        }
        
        [Fact]
        public async Task PutInvalidProperty()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var property = _fixture.Create<string>();
            var response = await client.PutAsync($"/things/Lamp/properties/{property}", 
                new StringContent($@"{{ ""{property}"": {_fixture.Create<int>()}  }}"));
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
