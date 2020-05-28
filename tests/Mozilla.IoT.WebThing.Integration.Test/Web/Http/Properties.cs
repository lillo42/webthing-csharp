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

namespace Mozilla.IoT.WebThing.Integration.Test.Web.Http
{
    public class Properties : IClassFixture<TestHost>
    {
        private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(30);
        private const string s_baseUrl = "/things/property-thing/properties";
        
        private readonly HttpClient _client;
        private readonly Fixture _fixture;

        public Properties(TestHost testHost)
        {
            _fixture = new Fixture();

            var host = testHost.Host;
            _client = host.GetTestClient();
        }

        #region Not found
        [Fact]
        public async Task GetProperty_Should_ReturnNotFound_When_NotFoundProperty()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"{s_baseUrl}/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetProperties_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{_fixture.Create<string>()}/properties", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetProperty_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client.GetAsync($"/things/{_fixture.Create<string>()}/properties/{_fixture.Create<string>()}", 
                source.Token);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task PutNotExistProperty_Should_ReturnNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var response = await _client
                .PutAsync($"{s_baseUrl}/AAAa", 
                    new StringContent($@"{{ ""AAA"": ""{value}"" }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Put_Should_ReturnNotFound_When_ThingNotFound()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var response = await _client
                .PutAsync($"/things/{_fixture.Create<string>()}/properties/text", 
                    new StringContent($@"{{ ""text"": ""{value}"" }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Bad Request
        [Theory]
        [InlineData("write")]
        [InlineData("write2")]
        public async Task GetWriteOnlyProperty_Should_ReturnBdRequest(string propertyName)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client
                .GetAsync($"{s_baseUrl}/{propertyName}", 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        

        [Fact]
        public async Task PutId_Should_ReturnBadRequest()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client
                .PutAsync($"{s_baseUrl}/Id2", 
                    new StringContent(@"{ ""Id2"": ""3756710b-623e-4602-8675-d94aa224e680"" }"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task PutModifyViaMethod_Should_ReturnBadRequest()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client
                .PutAsync($"{s_baseUrl}/modifyViaMethod", 
                    new StringContent(@"{ ""modifyViaMethod"": false }"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public async Task PutLevel_Should_ReturnBadRequest_When_ValueIsOutOfRange(int value)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client
                .PutAsync($"{s_baseUrl}/level", 
                    new StringContent($@"{{ ""level"": {value} }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task PutSomeText_Should_ReturnBadRequest_When_TextIsNull()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client
                .PutAsync($"{s_baseUrl}/text", 
                    new StringContent(@"{ ""text"": null }"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task PutExtraInformation_Should_ReturnBadRequest_When_ValueIsNotInEnum()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client
                .PutAsync($"{s_baseUrl}/extraInformation", 
                    new StringContent($@"{{ ""extraInformation"": [""{_fixture.Create<string>()}""] }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task PutText_Should_ReturnBdRequest_When_PropertyInBodyNotMatch()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var response = await _client
                .PutAsync($"{s_baseUrl}/text", 
                    new StringContent($@"{{ ""tex2"": ""{value}"" }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        #endregion

        #region Ok

        [Fact]
        public async Task GetProperties_Should_ReturnAllProperties_When_Requested()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            var response = await _client.GetAsync($"{s_baseUrl}", source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var message = await response.Content.ReadAsStringAsync();

            var json = JToken.Parse(message);

            json.Type.Should().Be(JTokenType.Object);
            json.Should().HaveCount(5);
            
            var id = json["id2"];
            id.Type.Should().Be(JTokenType.String);
            id.Value<string>().Should().Be("77bd476e-469d-4954-83b5-d9eedb2543ff");
            
            var modifyViaMethod = json["modifyViaMethod"];
            modifyViaMethod.Type.Should().Be(JTokenType.Boolean);
            modifyViaMethod.Value<bool>().Should().Be(true);
            
            var text = json["text"];
            text.Type.Should().Be(JTokenType.String);
            
            var level = json["level"];
            level.Type.Should().Be(JTokenType.Integer);
            
            var extraInformation = json["extraInformation"];
            extraInformation.Type.Should().Be(JTokenType.Array);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(10)]
        public async Task PutLevel_Should_ReturnOk_When_IsInRange(int value)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);
            
            var response = await _client
                .PutAsync($"{s_baseUrl}/level", 
                    new StringContent($@"{{ ""level"": {value} }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            
            response = await _client
                .GetAsync($"{s_baseUrl}/level", 
                    source.Token);

            var message = await response.Content.ReadAsStringAsync();
            
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse($@"
{{
    ""level"": {value}
}}
"));
        }
        
        [Fact]
        public async Task PutText_Should_ReturnOk_When_ValueIsNotNull()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var value = _fixture.Create<string>();
            var response = await _client
                .PutAsync($"{s_baseUrl}/text", 
                    new StringContent($@"{{ ""text"": ""{value}"" }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            
            response = await _client
                .GetAsync($"{s_baseUrl}/text", 
                    source.Token);

            var message = await response.Content.ReadAsStringAsync();
            
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse($@"
{{
    ""text"": ""{value}""
}}
"));
        }
        
        
        [Fact]
        public async Task PutExtraInformation_Should_ReturnOk_When_ValueIsNotNull()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(s_timeout);

            var response = await _client
                .PutAsync($"{s_baseUrl}/extraInformation", 
                    new StringContent($@"{{ ""extraInformation"": [""ABC"", ""GHI""] }}"), 
                    source.Token);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            
            response = await _client
                .GetAsync($"{s_baseUrl}/extraInformation", 
                    source.Token);

            var message = await response.Content.ReadAsStringAsync();
            
            FluentAssertions.Json.JsonAssertionExtensions
                .Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse($@"
{{
    ""extraInformation"": [""ABC"", ""GHI""] 
}}
"));
        }
        
        #endregion
    }
}
