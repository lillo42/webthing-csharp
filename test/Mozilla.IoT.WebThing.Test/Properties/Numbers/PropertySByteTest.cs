using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Number;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties.Numbers
{
    public class PropertySByteTest
    {
        private readonly NumberThing _thing;
        private readonly Fixture _fixture;
        
        public PropertySByteTest()
        {
            _fixture = new Fixture();
            _thing = new NumberThing();
        }
        
        #region No Nullable 
        private PropertySByte CreateNoNullable(sbyte[]? enums = null, sbyte? min = null, sbyte? max = null, sbyte? multipleOf = null)
            => new PropertySByte(_thing,
                thing => ((NumberThing)thing).Number,
                (thing, value) => ((NumberThing)thing).Number = (sbyte)value,
                false, min, max, multipleOf, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<sbyte>();
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<sbyte[]>();
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
        public void SetNoNullableWithMinValue(sbyte value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNoNullableWithMaxValue(sbyte value)
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
            var values = _fixture.Create<sbyte[]>();
            var property = CreateNoNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<sbyte>()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNoNullableWithMinValue(sbyte value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNoNullableWithMaxValue(sbyte value)
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

        private PropertySByte CreateNullable(sbyte[]? enums = null, sbyte? min = null, sbyte? max = null, sbyte? multipleOf = null)
            => new PropertySByte(_thing,
                thing => ((NumberThing)thing).NullableNumber,
                (thing, value) => ((NumberThing)thing).NullableNumber = (sbyte?)value,
                true, min, max, multipleOf, enums);

        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<sbyte>();
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
            var values = _fixture.Create<sbyte[]>();
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
        public void SetNullableWithMinValue(sbyte value)
        {
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableNumber.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNullableWithMaxValue(sbyte value)
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
            var values = _fixture.Create<sbyte[]>();
            var property = CreateNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<sbyte>()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNullableWithMinValue(sbyte value)
        {
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNullableWithMaxValue(sbyte value)
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
            
            public sbyte Number { get; set; }
            public sbyte? NullableNumber { get; set; }
        }
    }
}
