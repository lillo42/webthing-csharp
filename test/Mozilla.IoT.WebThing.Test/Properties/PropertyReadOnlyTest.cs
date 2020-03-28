using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties
{
    public class PropertyReadOnlyTest
    {
        private readonly Fixture _fixture;
        private readonly ReadOnlyThing _thing;

        public PropertyReadOnlyTest()
        {
            _fixture = new Fixture();
            _thing = new ReadOnlyThing();
        }

        [Fact]
        public void GetValue()
        {
            var property = new PropertyReadOnly(_thing, thing => ((ReadOnlyThing)thing).Reader);

            _thing.Reader = _fixture.Create<string>();

            property.GetValue().Should().NotBeNull();
            property.GetValue().Should().Be(_thing.Reader);
        }

        [Fact]
        public void TrySet()
        {
            var property = new PropertyReadOnly(_thing, thing => ((ReadOnlyThing)thing).Reader);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<string>()}"" }}");
            property.SetValue(jsonElement).Should().Be(SetPropertyResult.ReadOnly);
            property.GetValue().Should().BeNull();
        }
        
        public class ReadOnlyThing : Thing
        {
            public override string Name => "read-only-thing";
            
            
            public string Reader { get; set; }
        }
    }
}
