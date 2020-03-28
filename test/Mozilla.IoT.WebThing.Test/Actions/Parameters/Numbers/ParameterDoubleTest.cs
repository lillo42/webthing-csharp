using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions.Parameters.Number;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Actions.Parameters.Numbers
{
    public class ParameterDoubleTest
    {
        private readonly Fixture _fixture;
        
        public ParameterDoubleTest()
        {
            _fixture = new Fixture();
        }
        
        #region No Nullable 
        private static ParameterDouble CreateNoNullable(double[]? enums = null, double? min = null, double? max = null, double? multipleOf = null)
            => new ParameterDouble(false, min, max, multipleOf, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<double>();
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<double[]>();
            var property = CreateNoNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
                property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
                jsonValue.Should().Be(value);
            }
        }
        
        [Theory]
        [InlineData(11)]
        [InlineData(10)]
        public void SetNoNullableWithMinValue(double value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNoNullableWithMaxValue(double value)
        {
            var property = CreateNoNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithMultipleOfValue()
        {
            var property = CreateNoNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 10 }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(10);
        }
        
        [Fact]
        public void TrySetNoNullableWithNullValue()
        {
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(string))]
        public void TrySetNoNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(bool) ? _fixture.Create<bool>().ToString().ToLower() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Fact]
        public void TrySetNoNullableWithEnumValue()
        {
            var values = _fixture.Create<double[]>();
            var property = CreateNoNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<double>()} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNoNullableWithMinValue(double value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNoNullableWithMaxValue(double value)
        {
            var property = CreateNoNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Fact]
        public void TrySetNoNullableWithMultipleOfValue()
        {
            var property = CreateNoNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 9 }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        #endregion
        
        #region Nullable

        private static ParameterDouble CreateNullable(double[]? enums = null, double? min = null, double? max = null, double? multipleOf = null)
            => new ParameterDouble(true, min, max, multipleOf, enums);

        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<double>();
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithNullValue()
        {
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().BeNull();
        }
        
        [Fact]
        public void SetNullableWithValueEnums()
        {
            var values = _fixture.Create<double[]>();
            var property = CreateNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
                property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
                jsonValue.Should().Be(value);
            }
        }
        
        [Theory]
        [InlineData(11)]
        [InlineData(10)]
        public void SetNullableWithMinValue(double value)
        {
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNullableWithMaxValue(double value)
        {
            var property = CreateNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithMultipleOfValue()
        {
            var property = CreateNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 10 }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(10);
        }
        
        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(string))]
        public void TrySetNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(bool) ? _fixture.Create<bool>().ToString().ToLower() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Fact]
        public void TrySetNullableWitInvalidValueAndNotHaveValueInEnum()
        {
            var values = _fixture.Create<double[]>();
            var property = CreateNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<double>()} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNullableWithMinValue(double value)
        {
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNullableWithMaxValue(double value)
        {
            var property = CreateNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Fact]
        public void TrySetNullableWithMultipleOfValue()
        {
            var property = CreateNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 9 }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        #endregion
    }
}
