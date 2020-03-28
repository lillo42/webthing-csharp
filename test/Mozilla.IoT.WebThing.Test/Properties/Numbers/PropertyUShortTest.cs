using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Number;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties.Numbers
{
    public class PropertyUShortTest
    {
        private readonly NumberThing _thing;
        private readonly Fixture _fixture;
        
        public PropertyUShortTest()
        {
            _fixture = new Fixture();
            _thing = new NumberThing();
        }
        
        #region No Nullable 
        private PropertyUShort CreateNoNullable(ushort[]? enums = null, ushort? min = null, ushort? max = null, ushort? multipleOf = null)
            => new PropertyUShort(_thing,
                thing => ((NumberThing)thing).Number,
                (thing, value) => ((NumberThing)thing).Number = (ushort)value,
                false, min, max, multipleOf, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<ushort>();
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<ushort[]>();
            var property = CreateNoNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
                property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
                _thing.Number.Should().Be(value);
            }
        }
        
        [Theory]
        [InlineData(11)]
        [InlineData(10)]
        public void SetNoNullableWithMinValue(ushort value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNoNullableWithMaxValue(ushort value)
        {
            var property = CreateNoNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithMultipleOfValue()
        {
            var property = CreateNoNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 10 }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(10);
        }
        
        [Fact]
        public void TrySetNoNullableWithNullValue()
        {
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(string))]
        public void TrySetNoNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(bool) ? _fixture.Create<bool>().ToString().ToLower() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNoNullableWithEnumValue()
        {
            var values = _fixture.Create<ushort[]>();
            var property = CreateNoNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<ushort>()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNoNullableWithMinValue(ushort value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNoNullableWithMaxValue(ushort value)
        {
            var property = CreateNoNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNoNullableWithMultipleOfValue()
        {
            var property = CreateNoNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 9 }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        #endregion
        
        #region Nullable

        private PropertyUShort CreateNullable(ushort[]? enums = null, ushort? min = null, ushort? max = null, ushort? multipleOf = null)
            => new PropertyUShort(_thing,
                thing => ((NumberThing)thing).NullableNumber,
                (thing, value) => ((NumberThing)thing).NullableNumber = (ushort?)value,
                true, min, max, multipleOf, enums);

        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<ushort>();
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableNumber.Should().NotBeNull();
            _thing.NullableNumber.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithNullValue()
        {
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableNumber.Should().BeNull();
        }
        
        [Fact]
        public void SetNullableWithValueEnums()
        {
            var values = _fixture.Create<ushort[]>();
            var property = CreateNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
                property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
                _thing.NullableNumber.Should().Be(value);
            }
        }
        
        [Theory]
        [InlineData(11)]
        [InlineData(10)]
        public void SetNullableWithMinValue(ushort value)
        {
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableNumber.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNullableWithMaxValue(ushort value)
        {
            var property = CreateNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableNumber.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithMultipleOfValue()
        {
            var property = CreateNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 10 }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableNumber.Should().Be(10);
        }
        
        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(string))]
        public void TrySetNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(bool) ? _fixture.Create<bool>().ToString().ToLower() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNullableWitInvalidValueAndNotHaveValueInEnum()
        {
            var values = _fixture.Create<ushort[]>();
            var property = CreateNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<ushort>()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNullableWithMinValue(ushort value)
        {
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNullableWithMaxValue(ushort value)
        {
            var property = CreateNullable(max: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNullableWithMultipleOfValue()
        {
            var property = CreateNullable(multipleOf: 2);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": 9 }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        #endregion
        
        public class NumberThing : Thing
        {
            public override string Name => "number-thing";
            
            public ushort Number { get; set; }
            public ushort? NullableNumber { get; set; }
        }
    }
}
