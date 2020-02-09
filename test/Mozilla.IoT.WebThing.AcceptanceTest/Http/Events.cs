using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class Events
    {

        #region GET
        [Fact]
        public async Task GetAll()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync("/things/Lamp/events");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().BeEmpty();

            await Task.Delay(3_000);
            
            response = await client.GetAsync("/things/Lamp/events");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            message = await response.Content.ReadAsStringAsync();
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCountGreaterOrEqualTo(1);

            var obj = ((JArray)json)[0] as JObject;
            obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Value<int>().Should().Be(0);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);

        }
        
        [Fact]
        public async Task GetEvent()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync("/things/Lamp/events/overheated");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().BeEmpty();

            await Task.Delay(4_500);
            
            response = await client.GetAsync("/things/Lamp/events/overheated");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            message = await response.Content.ReadAsStringAsync();
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCount(1);

            var obj = ((JArray)json)[0] as JObject;
            obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Value<int>().Should().Be(0);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);

        }
        
        [Fact]
        public async Task GetInvalidEvent()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync("/things/Lamp/events/aaaaa");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        }
        #endregion

    }
}
