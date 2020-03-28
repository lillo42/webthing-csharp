using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.String;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties.Strings
{
    public class PropertyStringTest
    {
        private readonly StringThing _thing;
        private readonly Fixture _fixture;

        public PropertyStringTest()
        {
            _fixture = new Fixture();
            _thing = new StringThing();
        }
        
        private PropertyString CreateProperty(string[]? enums = null, string pattern = null, int? minimum = null, int? maximum = null, bool isNullable = false)
            => new PropertyString(_thing,
                thing => ((StringThing)thing).String,
                (thing, value) => ((StringThing)thing).String = (string)value,
                isNullable,
                minimum, maximum, pattern, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<string>();
            var property = CreateProperty();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<string>();
            var property = CreateProperty(isNullable: true);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().NotBeNull();
            _thing.String.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithNullValue()
        {
            var property = CreateProperty(isNullable: true);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().BeNull();
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<string[]>();
            var property = CreateProperty(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
                property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
                _thing.String.Should().Be(value);
            }
        }
        
        [Fact]
        public void SetNoNullableWithValuePattern()
        {
            var property = CreateProperty(pattern: @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
            const string value = "test@test.com";
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithMinLength()
        {
            var property = CreateProperty(minimum: 1);
            var value = _fixture.Create<char>().ToString();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().Be(value);
            
            value = _fixture.Create<string>();
            jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithMaxLength()
        {
            var property = CreateProperty(maximum: 37);
            var value = _fixture.Create<string>();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().Be(value);
            
            value = _fixture.Create<string>() + _fixture.Create<char>();
            jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.String.Should().Be(value);
        }
        
        [Fact]
        public void TrySetNoNullableWithNullValue()
        {
            var property = CreateProperty();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(bool))]
        public void TrySetNoNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"{_fixture.Create<bool>().ToString().ToLower()}";
            var property = CreateProperty();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNoNullableWithEnumValue()
        {
            var values = _fixture.Create<string[]>();
            var property = CreateProperty(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<string>()}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNoNullableWithMinLength()
        {
            var property = CreateProperty(minimum: 1);
            var value = string.Empty;
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);

            value = _fixture.Create<string>();
            jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": null }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
                
        [Fact]
        public void TrySetNoNullableWithMaxLength()
        {
            var property = CreateProperty(maximum: 36);
            var value = _fixture.Create<string>() + _fixture.Create<int>().ToString()[0];
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
            
            jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": null }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNoNullableWithValuePattern()
        {
            var property = CreateProperty(pattern: @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<string>()}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }

        public class StringThing : Thing
        {
            public override string Name => "string-thing";
            
            public string String { get; set; }
        }
    }
}
