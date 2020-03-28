using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Number;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties.Numbers
{
    public class PropertyByteTest
    {
        private readonly NumberThing _thing;
        private readonly Fixture _fixture;
        
        public PropertyByteTest()
        {
            _fixture = new Fixture();
            _thing = new NumberThing();
        }
        
        #region No Nullable 
        private PropertyByte CreateNoNullable(byte[]? enums = null, byte? min = null, byte? max = null, byte? multipleOf = null)
            => new PropertyByte(_thing,
                thing => ((NumberThing)thing).Number,
                (thing, value) => ((NumberThing)thing).Number = (byte)value,
                false, min, max, multipleOf, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<byte>();
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<byte[]>();
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
        public void SetNoNullableWithMinValue(byte value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNoNullableWithMaxValue(byte value)
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
            var values = _fixture.Create<byte[]>();
            var property = CreateNoNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<byte>()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNoNullableWithMinValue(byte value)
        {
            var property = CreateNoNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNoNullableWithMaxValue(byte value)
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

        private PropertyByte CreateNullable(byte[]? enums = null, byte? min = null, byte? max = null, byte? multipleOf = null)
            => new PropertyByte(_thing,
                thing => ((NumberThing)thing).NullableNumber,
                (thing, value) => ((NumberThing)thing).NullableNumber = (byte?)value,
                true, min, max, multipleOf, enums);

        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<byte>();
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
            var values = _fixture.Create<byte[]>();
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
        public void SetNullableWithMinValue(byte value)
        {
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableNumber.Should().Be(value);
        }
        
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        public void SetNullableWithMaxValue(byte value)
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
            var values = _fixture.Create<byte[]>();
            var property = CreateNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {_fixture.Create<byte>()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public void TrySetNullableWithMinValue(int param)
        {
            var value = Convert.ToByte(param);
            var property = CreateNullable(min: 10);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(11)]
        public void TrySetNullableWithMaxValue(int param)
        {
            var value = Convert.ToByte(param);
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
            
            public byte Number { get; set; }
            public byte? NullableNumber { get; set; }
        }
    }
}
