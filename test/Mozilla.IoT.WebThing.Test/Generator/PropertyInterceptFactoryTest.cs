using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Properties;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator
{
    public class PropertyInterceptFactoryTest
    {
        private readonly Fixture _fixture;
        private readonly LampThing _thing;
        private readonly PropertiesInterceptFactory _factory;

        public PropertyInterceptFactoryTest()
        {
            _fixture = new Fixture();
            _thing = new LampThing();
            _factory = new PropertiesInterceptFactory(_thing);
        }

        [Fact]
        public void Ignore()
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });
 
            var properties = _factory.Create();
            properties.GetProperties().ContainsKey(nameof(LampThing.Ignore)).Should().BeFalse();
        }
        
        
        [Fact]
        public void GetValue()
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });
            var id = _fixture.Create<int>();
            var value = _fixture.Create<string>();
            _thing.Id = id;
            _thing.Value = value;

            var properties = _factory.Create();
            var values = properties.GetProperties();
            
            values.ContainsKey(nameof(LampThing.Id)).Should().BeTrue();
            values.ContainsKey("test").Should().BeTrue();
            
            values[nameof(LampThing.Id)].Should().Be(id);
            values["test"].Should().Be(value);
            
            properties.GetProperties(nameof(LampThing.Id))[nameof(LampThing.Id)].Should().Be(id);
            properties.GetProperties("test")["test"].Should().Be(value);
        }
        
        [Fact]
        public void SetPropertyThatNotExist()
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });
            
            var properties = _factory.Create();
            var result = properties.SetProperty(_fixture.Create<string>(), _fixture.Create<int>());
            result.Should().Be(SetPropertyResult.NotFound);
        }
        
        [Fact]
        public void SetPropertyWithoutValidation()
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });
            var id = _fixture.Create<int>();

            var doc = JsonDocument.Parse($"{{ \"p\": {id} }}");
            var properties = _factory.Create();
            var result = properties.SetProperty(nameof(LampThing.Id), doc.RootElement.GetProperty("p"));
            result.Should().Be(SetPropertyResult.Ok);
            _thing.Id.Should().Be(id);
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        [InlineData(31)]
        public void SetPropertyWithInvalidationValue(int value)
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });

            var initial = _fixture.Create<int>();
            _thing.Valid = initial;
            var doc = JsonDocument.Parse($"{{ \"p\": {value} }}");
            var properties = _factory.Create();
            var result = properties.SetProperty(nameof(LampThing.Valid), doc.RootElement.GetProperty("p"));
            result.Should().Be(SetPropertyResult.InvalidValue);
            _thing.Valid.Should().Be(initial);
        }
        
        [Theory]
        [InlineData(2)]
        [InlineData(100)]
        public void SetPropertyWithValidationValue(int value)
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });

            var initial = _fixture.Create<int>();
            _thing.Valid = initial;
            var doc = JsonDocument.Parse($"{{ \"p\": {value} }}");
            var properties = _factory.Create();
            var result = properties.SetProperty(nameof(LampThing.Valid), doc.RootElement.GetProperty("p"));
            result.Should().Be(SetPropertyResult.Ok);
            _thing.Valid.Should().Be(value);
        }
        
        [Fact]
        public void SetPropertyWithInvalidationRange()
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });

            var value = _fixture.Create<string>();
            var initial = _fixture.Create<string>();
            _thing.Range = initial;
            var doc = JsonDocument.Parse($"{{ \"p\": \"{value}\" }}");
            var properties = _factory.Create();
            var result = properties.SetProperty(nameof(LampThing.Range), doc.RootElement.GetProperty("p"));
            result.Should().Be(SetPropertyResult.InvalidValue);
            _thing.Range.Should().Be(initial);
        }
        
        [Theory]
        [InlineData("AAA")]
        [InlineData("BBB")]
        public void SetPropertyWithValidationRange(string value)
        {
            CodeGeneratorFactory.Generate(_thing, new []{ _factory });

            var initial = _fixture.Create<string>();
            _thing.Range = initial;
            var doc = JsonDocument.Parse($"{{ \"p\": \"{value}\" }}");
            var properties = _factory.Create();
            var result = properties.SetProperty(nameof(LampThing.Range), doc.RootElement.GetProperty("p"));
            result.Should().Be(SetPropertyResult.Ok);
            _thing.Range.Should().Be(value);
        }
        

        public class LampThing : Thing
        {
            public override string Name => nameof(LampThing);
            
            public int Id { get; set; }
            
            [ThingProperty(Name = "test")]
            public string Value { get; set; }

            [ThingProperty(Ignore = true)]
            public bool Ignore { get; set; }
            
            [ThingProperty(Minimum = 0, Maximum = 100, MultipleOf = 2)]
            public int Valid { get; set; }
            
            [ThingProperty(Enum = new object[]{ "AAA", "BBB"})]
            public string Range { get; set; }
        }
    }
}
