using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Mozilla.IoT.WebThing.Builder;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class WsUrlBuilderTest
    {
        private readonly Fixture _fixture;
        private readonly HttpRequest _request;
        private readonly IWsUrlBuilder _builder;

        public WsUrlBuilderTest()
        {
            _fixture = new Fixture();
            _builder = new WsUrlBuilder();
            _request = Substitute.For<HttpRequest>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Build_Should_ReturnWsWithoutThingPath_When_ThingIsNotPass(string thing)
        {
            string host = _fixture.Create<string>();
            int port = _fixture.Create<int>();
            
            _request.Scheme.Returns("http");
            _request.Host.Returns(new HostString(host, port));
            _request.Path.Returns(new PathString("/"));

            string url = _builder.Build(_request, thing);

            url.Should().Be($"ws://{host}:{port}/");
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Build_Should_ReturnWssWithoutThingPath_When_ThingIsNotPass(string thing)
        {
            string host = _fixture.Create<string>();
            int port = _fixture.Create<int>();
            
            _request.Scheme.Returns("https");
            _request.Host.Returns(new HostString(host, port));
            _request.Path.Returns(new PathString("/"));

            string url = _builder.Build(_request, thing);

            url.Should().Be($"wss://{host}:{port}/");
        }
        
        
        [Fact]
        public void Build_Should_ReturnWsWithThingPath_When_ThingIsPass()
        {
            string thing = _fixture.Create<string>();
            string host = _fixture.Create<string>();
            int port = _fixture.Create<int>();
            
            _request.Scheme.Returns("http");
            _request.Host.Returns(new HostString(host, port));
            _request.Path.Returns(new PathString("/"));

            string url = _builder.Build(_request, thing);

            url.Should().Be($"ws://{host}:{port}/things/{thing}");
        }
        
        [Fact]
        public void Build_Should_ReturnWssWithThingPath_When_ThingIsPass()
        {
            string thing = _fixture.Create<string>();
            string host = _fixture.Create<string>();
            int port = _fixture.Create<int>();
            
            _request.Scheme.Returns("https");
            _request.Host.Returns(new HostString(host, port));
            _request.Path.Returns(new PathString());

            string url = _builder.Build(_request, thing);

            url.Should().Be($"wss://{host}:{port}/things/{thing}");
        }
    }
}
