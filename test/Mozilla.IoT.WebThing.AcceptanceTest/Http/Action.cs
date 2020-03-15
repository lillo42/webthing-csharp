using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Http
{
    public class Action
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private readonly HttpClient _client;
        public Action()
        {
            var host = Program.GetHost().GetAwaiter().GetResult();
            _client = host.GetTestServer().CreateClient();
        }
        
        [Theory]
        [InlineData(50, 2_000)]
        public async Task Create(int level, int duration)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync("/things/lamp/actions", 
                new StringContent($@"
{{ 
    ""fade"": {{
        ""input"": {{
            ""level"": {level},
            ""duration"": {duration}
        }}    
    }} 
}}"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.PostAsync("/things/lamp/actions/fade", 
                new StringContent($@"
{{ 
    ""fade"": {{
        ""input"": {{
            ""level"": {level},
            ""duration"": {duration}
        }}    
    }} 
}}"), source.Token).ConfigureAwait(false);
            
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.PostAsync("/things/lamp/actions/aaaa", 
                new StringContent(@"
{ 
    ""aaaa"": {
        ""input"": {
            ""level"": 10,
            ""duration"": 100
        }    
    } 
}"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task TryCreateActionWithOtherName()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.PostAsync("/things/lamp/actions/fade", 
                new StringContent(@"
{ 
    ""aaaa"": {
        ""input"": {
            ""level"": 10,
            ""duration"": 100
        }    
    } 
}"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Theory]
        [InlineData(-1, 2_000)]
        [InlineData(101, 2_000)]
        public async Task TryCreateWithInvalidParameter(int level, int duration)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.PostAsync("/things/lamp/actions", 
                new StringContent($@"
{{ 
    ""fade"": {{
        ""input"": {{
            ""level"": {level},
            ""duration"": {duration}
        }}    
    }} 
}}"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            response = await _client.GetAsync("/things/lamp/actions", source.Token).ConfigureAwait(false);
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JToken.Parse(message);
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCount(0);
        }
        
        [Fact]
        public async Task LongRunner()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.PostAsync("/things/lamp/actions", 
                new StringContent($@"
{{ 
    ""longRun"": {{

    }} 
}}"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<LongRun>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Href.Should().StartWith("/things/lamp/actions/longRun/");
            json.Status.Should().NotBeNullOrEmpty();
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);

            await Task.Delay(3_000).ConfigureAwait(false);

            response = await _client.GetAsync($"/things/lamp/actions/longRun/{json.Href.Substring(json.Href.LastIndexOf('/') + 1)}", source.Token)
                .ConfigureAwait(false);
            
            message = await response.Content.ReadAsStringAsync();
            json = JsonConvert.DeserializeObject<LongRun>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Href.Should().StartWith("/things/lamp/actions/longRun/");
            json.Status.Should().NotBeNullOrEmpty();
            json.Status.Should().Be(Status.Completed.ToString().ToLower());
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);
            json.TimeCompleted.Should().NotBeNull();
            json.TimeCompleted.Should().BeBefore(DateTime.UtcNow);
        }
        
        [Fact]
        public async Task CancelAction()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.PostAsync("/things/lamp/actions", 
                new StringContent($@"
{{ 
    ""longRun"": {{
    }} 
}}"), source.Token).ConfigureAwait(false);
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<LongRun>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            json.Href.Should().StartWith("/things/lamp/actions/longRun/");
            json.Status.Should().NotBeNullOrEmpty();
            json.TimeRequested.Should().BeBefore(DateTime.UtcNow);

            response = await _client.DeleteAsync($"/things/lamp/actions/longRun/{json.Href.Substring(json.Href.LastIndexOf('/') + 1)}", source.Token)
                .ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            response = await _client.GetAsync($"/things/lamp/actions/longRun/{json.Href.Substring(json.Href.LastIndexOf('/') + 1)}", source.Token)
                .ConfigureAwait(false);
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
