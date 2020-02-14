using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class Action
    {
        [Theory]
        [InlineData(50, 2_000)]
        public async Task Create(int level, int duration)
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            
            var response = await client.PostAsync("/things/Lamp/actions", 
                new StringContent($@"
{{ 
    ""fade"": {{
        ""input"": {{
            ""level"": {level},
            ""duration"": {duration}
        }}    
    }} 
}}"));
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<Fade>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Level.Should().Be(level);
            json.Input.Duration.Should().Be(duration);
            json.Href.Should().StartWith("/things/lamp/actions/fade/");
            json.Status.Should().NotBeNullOrEmpty();
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);
        }
        
        [Theory]
        [InlineData(50, 2_000)]
        public async Task CreateInSpecificUrl(int level, int duration)
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            
            var response = await client.PostAsync("/things/lamp/actions/fade", 
                new StringContent($@"
{{ 
    ""fade"": {{
        ""input"": {{
            ""level"": {level},
            ""duration"": {duration}
        }}    
    }} 
}}"));
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<Fade>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Input.Should().NotBeNull();
            json.Input.Level.Should().Be(level);
            json.Input.Duration.Should().Be(duration);
            json.Href.Should().StartWith("/things/lamp/actions/fade/");
            json.Status.Should().NotBeNullOrEmpty();
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);
        }
        
        [Fact]
        public async Task InvalidAction()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            
            var response = await client.PostAsync("/things/lamp/actions/aaaa", 
                new StringContent(@"
{ 
    ""aaaa"": {
        ""input"": {
            ""level"": 10,
            ""duration"": 100
        }    
    } 
}"));
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task TryCreateActionWithOtherName()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            
            var response = await client.PostAsync("/things/lamp/actions/fade", 
                new StringContent(@"
{ 
    ""aaaa"": {
        ""input"": {
            ""level"": 10,
            ""duration"": 100
        }    
    } 
}"));
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Theory]
        [InlineData(-1, 2_000)]
        [InlineData(101, 2_000)]
        public async Task TryCreateWithInvalidParameter(int level, int duration)
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            
            var response = await client.PostAsync("/things/Lamp/actions", 
                new StringContent($@"
{{ 
    ""fade"": {{
        ""input"": {{
            ""level"": {level},
            ""duration"": {duration}
        }}    
    }} 
}}"));
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            response = await client.GetAsync("/things/Lamp/actions");
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(message);
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCount(0);
        }
        
        [Fact]
        public async Task LongRunner()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            
            var response = await client.PostAsync("/things/Lamp/actions", 
                new StringContent($@"
{{ 
    ""longRun"": {{
    }} 
}}"));
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<LongRun>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Href.Should().StartWith("/things/lamp/actions/longRun/");
            json.Status.Should().NotBeNullOrEmpty();
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);

            await Task.Delay(3_000);

            response = await client.GetAsync($"/things/lamp/actions/longRun/{json.Href.Substring(json.Href.LastIndexOf('/') + 1)}");
            message = await response.Content.ReadAsStringAsync();
            json = JsonConvert.DeserializeObject<LongRun>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Href.Should().StartWith("/things/lamp/actions/longRun/");
            json.Status.Should().NotBeNullOrEmpty();
            json.Status.Should().Be("completed");
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);
            json.TimeCompleted.Should().NotBeNull();
            json.TimeCompleted.Should().BeBefore(DateTime.UtcNow);
        }
        
        [Fact]
        public async Task CancelAction()
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync();
            var client = host.GetTestServer().CreateClient();
            
            var response = await client.PostAsync("/things/Lamp/actions", 
                new StringContent($@"
{{ 
    ""LongRun"": {{
    }} 
}}"));
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<LongRun>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Href.Should().StartWith("/things/lamp/actions/longRun/");
            json.Status.Should().NotBeNullOrEmpty();
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);

            response = await client.DeleteAsync($"/things/lamp/actions/longRun/{json.Href.Substring(json.Href.LastIndexOf('/') + 1)}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            response = await client.GetAsync($"/things/lamp/actions/longRun/{json.Href.Substring(json.Href.LastIndexOf('/') + 1)}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        public class LongRun
        {
            public string Href { get; set; }
            public string Status { get; set; }
            public DateTime TimeRequested { get; set; }
            public DateTime? TimeCompleted { get; set; }
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
            public int Level { get; set; }
            public int Duration { get; set; }
        }
    }
}
