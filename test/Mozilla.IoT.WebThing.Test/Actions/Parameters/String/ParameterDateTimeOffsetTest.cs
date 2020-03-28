using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions.Parameters.String;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Actions.Parameters.String
{
    public class ParameterDateTimeOffsetTest
    {
        private readonly Fixture _fixture;

        public ParameterDateTimeOffsetTest()
        {
            _fixture = new Fixture();
        }
        
        #region No Nullable
        private static ParameterDateTimeOffset CreateNoNullable(DateTimeOffset[]? enums = null)
            => new ParameterDateTimeOffset(false, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<DateTimeOffset>();
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value:O}"" }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
            jsonValue.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<DateTimeOffset[]>();
            var property = CreateNoNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value:O}"" }}");
                property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
                jsonValue.Should().Be(value);
            }
        }
        
        [Fact]
        public void TrySetNoNullableWithNullValue()
        {
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void TrySetNoNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Fact]
        public void TrySetNoNullableWithEnumValue()
        {
            var values = _fixture.Create<DateTimeOffset[]>();
            var property = CreateNoNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<DateTimeOffset>():O}"" }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        #endregion
        
        #region Nullable
        
        private static ParameterDateTimeOffset CreateNullable(DateTimeOffset[] enums = null)
            => new ParameterDateTimeOffset(true, enums);

        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<DateTimeOffset>();
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value:O}"" }}");
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
            var values = _fixture.Create<DateTimeOffset[]>();
            var property = CreateNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value:O}"" }}");
                property.TryGetValue(jsonElement.GetProperty("input"), out var jsonValue).Should().BeTrue();
                jsonValue.Should().Be(value);
            }
        }
        
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void TrySetNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        [Fact]
        public void TrySetNullableWitInvalidValueAndNotHaveValueInEnum()
        {
            var values = _fixture.Create<DateTimeOffset[]>();
            var property = CreateNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<DateTimeOffset>():O}"" }}");
            property.TryGetValue(jsonElement.GetProperty("input"), out _).Should().BeFalse();
        }
        
        #endregion
    }
}
