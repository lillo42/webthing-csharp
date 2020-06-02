using System;
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
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Web.Http
{
    public class Action : IClassFixture<TestHost>
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private const string s_baseUrl = "/things/action-thing/actions";
        
        private readonly HttpClient _client;
        private readonly Fixture _fixture;

        public Action(TestHost testHost)
        {
            _fixture = new Fixture();

            var host = testHost.Host;
            _client = host.GetTestClient();
        }

        #region Not Found

        [Fact]
        public async Task GetActions_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{_fixture.Create<string>()}/actions", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetAction_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{_fixture.Create<string>()}/actions/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetAction_Should_ReturnNotFound_When_ActionNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"{s_baseUrl}/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetActionById_Should_ReturnNotFound_When_ActionNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.GetAsync(
                $"{s_baseUrl}/{_fixture.Create<string>()}/{_fixture.Create<string>()}",
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetActionById_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.GetAsync(
                $"/things/{_fixture.Create<string>()}/actions/longRunning/{_fixture.Create<string>()}",
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetActionById_Should_ReturnNotFound_When_ActionIdNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.GetAsync(
                $"{s_baseUrl}/longRunning/{_fixture.Create<string>()}",
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task PostAction_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"/things/{_fixture.Create<string>()}/actions/longRunning",
                new StringContent("{ input: { } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task PostAction_Should_ReturnNotFound_When_ActionNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/{_fixture.Create<string>()}",
                new StringContent("{ input: { } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task PostActions_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"/things/{_fixture.Create<string>()}/actions",
                new StringContent("{ input: { } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task DeleteAction_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.DeleteAsync($"/things/{_fixture.Create<string>()}/actions/{_fixture.Create<string>()}/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task DeleteAction_Should_ReturnNotFound_When_ActionNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.DeleteAsync($"{s_baseUrl}/{_fixture.Create<string>()}/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task DeleteAction_Should_ReturnNotFound_When_ActionIdNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.DeleteAsync($"{s_baseUrl}/longRunning/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        #endregion

        #region Bad Request

        [Fact]
        public async Task PostAction_Should_ReturnBadRequest_When_HaveMoreThan1Input()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/longRunning",
                new StringContent(@"{ ""longRunning"": { ""input"": { } }, ""longRunning2"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task PostActions_Should_ReturnBadRequest_When_HaveMoreThan1Input()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}",
                new StringContent(@"{ ""longRunning"": { ""input"": { } }, ""longRunning2"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task PostAction_Should_ReturnBadRequest_When_ActionNotMatch()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/longRunning",
                new StringContent(@"{ ""longRunning2"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Theory]
        [InlineData(null, 2, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("", 1, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("Foo", 100, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("Bar", 50, "c01bff3e-aed4-480a-89cd-20caa40c8468")]
        public async Task PostAction_Should_ReturnBadRequest_When_OneOfValueIsInValid(string value, int level, string id)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/withRestriction",
                new StringContent($@"
{{
    ""withRestriction"": {{ 
        ""input"": {{
            ""value"": {(value == null ? "null" : $@"""{value}""")},
            ""level"": {level},
            ""active"": {_fixture.Create<bool>().ToString().ToLower()},
            ""id"": ""{Guid.Parse(id)}""
        }} 
    }} 
}}", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Theory]
        [InlineData(null, 2, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("", 1, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("Foo", 100, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData("Bar", 50, "c01bff3e-aed4-480a-89cd-20caa40c8468")]
        public async Task PostActions_Should_ReturnBadRequest_When_OneOfValueIsInValid(string value, int level, string id)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}",
                new StringContent($@"
{{
    ""withRestriction"": {{ 
        ""input"": {{
            ""value"": {(value == null ? "null" : $@"""{value}""")},
            ""level"": {level},
            ""active"": {_fixture.Create<bool>().ToString().ToLower()},
            ""id"": ""{Guid.Parse(id)}""
        }} 
    }} 
}}", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        #endregion

        #region Ok

        [Fact]
        public async Task PostLongRunningAction_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/longRunning",
                new StringContent(@"{ ""longRunning"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["longRunning"].Type.Should().Be(JTokenType.Object);
            json["longRunning"]["status"].Type.Should().Be(JTokenType.String);
            json["longRunning"]["status"].Value<string>().Should().Be("created");
            json["longRunning"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync(json["longRunning"]["href"].Value<string>(), source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["longRunning"].Type.Should().Be(JTokenType.Object);
            json["longRunning"]["status"].Type.Should().Be(JTokenType.String);
            json["longRunning"]["status"].Value<string>().Should().BeOneOf("pending");
            json["longRunning"]["href"].Type.Should().Be(JTokenType.String);
        }
        
        [Fact]
        public async Task PostLongRunningActions_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}",
                new StringContent(@"{ ""longRunning"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["longRunning"].Type.Should().Be(JTokenType.Object);
            json["longRunning"]["status"].Type.Should().Be(JTokenType.String);
            json["longRunning"]["status"].Value<string>().Should().BeOneOf("created");
            json["longRunning"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync(json["longRunning"]["href"].Value<string>(), source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["longRunning"].Type.Should().Be(JTokenType.Object);
            json["longRunning"]["status"].Type.Should().Be(JTokenType.String);
            json["longRunning"]["status"].Value<string>().Should().BeOneOf("pending");
            json["longRunning"]["href"].Type.Should().Be(JTokenType.String);
        }
        
        [Fact]
        public async Task PostNoRestrictionAction_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var level = _fixture.Create<int>();
            var active = _fixture.Create<bool>();
            var id = _fixture.Create<Guid>();
            
            var response = await _client.PostAsync(
                $"{s_baseUrl}/noRestriction",
                new StringContent($@"
{{
    ""noRestriction"": {{ 
        ""input"": {{
            ""value"": ""{value}"",
            ""level"": {level},
            ""active"": {active.ToString().ToLower()},
            ""id"": ""{id}""
        }} 
    }} 
}}
", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["noRestriction"].Type.Should().Be(JTokenType.Object);
            json["noRestriction"]["input"].Type.Should().Be(JTokenType.Object);
            
            json["noRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["noRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["noRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["noRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["noRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["noRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToString().ToUpper());
            
            json["noRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["noRestriction"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync(json["noRestriction"]["href"].Value<string>(), source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["noRestriction"].Type.Should().Be(JTokenType.Object);
            
            json["noRestriction"]["input"].Type.Should().Be(JTokenType.Object);
            
            json["noRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["noRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["noRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["noRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["noRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["noRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToString().ToUpper());
            
            json["noRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["noRestriction"]["href"].Type.Should().Be(JTokenType.String);
        }
        
        [Fact]
        public async Task PostNoRestrictionActions_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var level = _fixture.Create<int>();
            var active = _fixture.Create<bool>();
            var id = _fixture.Create<Guid>();
            
            var response = await _client.PostAsync(
                $"{s_baseUrl}",
                new StringContent($@"
{{
    ""noRestriction"": {{ 
        ""input"": {{
            ""value"": ""{value}"",
            ""level"": {level},
            ""active"": {active.ToString().ToLower()},
            ""id"": ""{id}""
        }} 
    }} 
}}
", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["noRestriction"].Type.Should().Be(JTokenType.Object);
            json["noRestriction"]["input"].Type.Should().Be(JTokenType.Object);
            
            json["noRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["noRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["noRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["noRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["noRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["noRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToString().ToUpper());
            
            json["noRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["noRestriction"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync(json["noRestriction"]["href"].Value<string>(), source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["noRestriction"].Type.Should().Be(JTokenType.Object);
            
            json["noRestriction"]["input"].Type.Should().Be(JTokenType.Object);
            
            json["noRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["noRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["noRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["noRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["noRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["noRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToString().ToUpper());
            
            json["noRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["noRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["noRestriction"]["href"].Type.Should().Be(JTokenType.String);
        }
        
        [Theory]
        [InlineData(2, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData(99, "a8e3202d-7eaa-4889-a5cf-ec44275414eb")]
        public async Task PostWithRestrictionAction_Should_ReturnOk(int level, string id)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<string>();
            var active = _fixture.Create<bool>();
            
            var response = await _client.PostAsync(
                $"{s_baseUrl}/withRestriction",
                new StringContent($@"
{{
    ""withRestriction"": {{ 
        ""input"": {{
            ""value"": ""{value}"",
            ""level"": {level},
            ""active"": {active.ToString().ToLower()},
            ""id"": ""{Guid.Parse(id)}""
        }} 
    }}
}}
", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["withRestriction"].Type.Should().Be(JTokenType.Object);
            
            json["withRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["withRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["withRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["withRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["withRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["withRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToUpper());
            
            json["withRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["withRestriction"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync(json["withRestriction"]["href"].Value<string>(), source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["withRestriction"].Type.Should().Be(JTokenType.Object);
            
            json["withRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["withRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["withRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["withRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["withRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["withRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToUpper());
            
            json["withRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["withRestriction"]["href"].Type.Should().Be(JTokenType.String);
        }

        [Theory]
        [InlineData(2, "4f12dbd1-ea24-40e6-ac26-620a1b787a25")]
        [InlineData(99, "a8e3202d-7eaa-4889-a5cf-ec44275414eb")]
        public async Task PostWithRestrictionActions_Should_ReturnOk(int level, string id)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            
            var value = _fixture.Create<string>();
            var active = _fixture.Create<bool>();
            
            var response = await _client.PostAsync(
                $"{s_baseUrl}",
                new StringContent($@"
{{
    ""withRestriction"": {{ 
        ""input"": {{
            ""value"": ""{value}"",
            ""level"": {level},
            ""active"": {active.ToString().ToLower()},
            ""id"": ""{Guid.Parse(id)}""
        }} 
    }}
}}
", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["withRestriction"].Type.Should().Be(JTokenType.Object);
            
            json["withRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["withRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["withRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["withRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["withRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["withRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToUpper());
            
            json["withRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["withRestriction"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync(json["withRestriction"]["href"].Value<string>(), source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["withRestriction"].Type.Should().Be(JTokenType.Object);
            
            json["withRestriction"]["input"]["value"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["value"].Value<string>().Should().Be(value);
            
            json["withRestriction"]["input"]["level"].Type.Should().Be(JTokenType.Integer);
            json["withRestriction"]["input"]["level"].Value<int>().Should().Be(level);
            
            json["withRestriction"]["input"]["active"].Type.Should().Be(JTokenType.Boolean);
            json["withRestriction"]["input"]["active"].Value<bool>().Should().Be(active);
            
            json["withRestriction"]["input"]["id"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["input"]["id"].Value<string>().ToUpper().Should().Be(id.ToUpper());
            
            json["withRestriction"]["status"].Type.Should().Be(JTokenType.String);
            json["withRestriction"]["status"].Value<string>().Should().BeOneOf("created", "pending", "completed");
            json["withRestriction"]["href"].Type.Should().Be(JTokenType.String);
        }
        
        [Fact]
        public async Task GetLongRunningAction_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/longRunning",
                new StringContent(@"{ ""longRunning"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["longRunning"].Type.Should().Be(JTokenType.Object);
            json["longRunning"]["status"].Type.Should().Be(JTokenType.String);
            json["longRunning"]["status"].Value<string>().Should().BeOneOf("created", "pending");
            json["longRunning"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync($"{s_baseUrl}/longRunning", source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Array);
            json.Count().Should().BeGreaterOrEqualTo(1);
        }
        
        [Fact]
        public async Task GetActions_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/longRunning",
                new StringContent(@"{ ""longRunning"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["longRunning"].Type.Should().Be(JTokenType.Object);
            json["longRunning"]["status"].Type.Should().Be(JTokenType.String);
            json["longRunning"]["status"].Value<string>().Should().BeOneOf("created", "pending");
            json["longRunning"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.GetAsync(s_baseUrl, source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Array);
            json.Count().Should().BeGreaterOrEqualTo(1);
        }
        
        [Fact]
        public async Task DeleteActions_Should_ReturnOk()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client.PostAsync(
                $"{s_baseUrl}/longRunning",
                new StringContent(@"{ ""longRunning"": { ""input"": { } } }", Encoding.UTF8),
                source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());

            json.Type.Should().Be(JTokenType.Object);
            json["longRunning"].Type.Should().Be(JTokenType.Object);
            json["longRunning"]["status"].Type.Should().Be(JTokenType.String);
            json["longRunning"]["status"].Value<string>().Should().BeOneOf("created", "pending");
            json["longRunning"]["href"].Type.Should().Be(JTokenType.String);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            response = await _client.DeleteAsync(json["longRunning"]["href"].Value<string>(), source.Token);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            response = await _client.GetAsync(json["longRunning"]["href"].Value<string>(), source.Token);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        #endregion
    }
}
