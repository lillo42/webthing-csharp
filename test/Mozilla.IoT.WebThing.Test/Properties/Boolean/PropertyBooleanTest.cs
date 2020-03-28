using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Boolean;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties.Boolean
{
    public class PropertyBooleanTest
    {
        private readonly BoolThing _thing;
        private readonly Fixture _fixture;

        public PropertyBooleanTest()
        {
            _thing = new BoolThing();
            _fixture = new Fixture();
        }
        
        #region No Nullable

        private PropertyBoolean CreateNoNullable()
            => new PropertyBoolean(_thing,
                thing => ((BoolThing)thing).Bool,
                (thing, value) => ((BoolThing)thing).Bool = (bool)value,
                false);

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetNoNullableWithValue(bool value)
        {
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value.ToString().ToLower()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Bool.Should().Be(value);
        }
        
        [Fact]
        public void TrySetNoNullableWithNullValue()
        {
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void TrySetNoNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        #endregion
        
        #region Nullable

        private PropertyBoolean CreateNullable()
            => new PropertyBoolean(_thing,
                thing => ((BoolThing)thing).NullableBool,
                (thing, value) => ((BoolThing)thing).NullableBool = (bool?)value,
                true);

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetNullableWithValue(bool value)
        {
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value.ToString().ToLower()} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableBool.Should().NotBeNull();
            _thing.NullableBool.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithNullValue()
        {
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableBool.Should().BeNull();
        }
        
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void TrySetNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"""{_fixture.Create<int>().ToString()}""";
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        #endregion
        
        public class BoolThing : Thing
        {
            public override string Name => "bool-thing";
            
            public bool Bool { get; set; }
            public bool? NullableBool { get; set; }
        }
    }
}
