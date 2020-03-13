using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties.String;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties.Strings
{
    public class PropertyGuidTest
    {
        private readonly GuidThing _thing;
        private readonly Fixture _fixture;

        public PropertyGuidTest()
        {
            _fixture = new Fixture();
            _thing = new GuidThing();
        }
        
        #region No Nullable
        private PropertyGuid CreateNoNullable(Guid[]? enums = null)
            => new PropertyGuid(_thing,
                thing => ((GuidThing)thing).Guid,
                (thing, value) => ((GuidThing)thing).Guid = (Guid)value,
                false, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<Guid>();
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.Guid.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<Guid[]>();
            var property = CreateNoNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
                property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
                _thing.Guid.Should().Be(value);
            }
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
        [InlineData(typeof(DateTime))]
        public void TrySetNoNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"""{_fixture.Create<DateTime>()}""";
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNoNullableWithEnumValue()
        {
            var values = _fixture.Create<Guid[]>();
            var property = CreateNoNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<Guid>()}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        #endregion
        
        #region Nullable

        private PropertyGuid CreateNullable(Guid[] enums = null)
            => new PropertyGuid(_thing,
                thing => ((GuidThing)thing).NullableGuid,
                (thing, value) => ((GuidThing)thing).NullableGuid = (Guid?)value,
                true, enums);

        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<Guid>();
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableGuid.Should().NotBeNull();
            _thing.NullableGuid.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithNullValue()
        {
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableGuid.Should().BeNull();
        }
        
        [Fact]
        public void SetNullableWithValueEnums()
        {
            var values = _fixture.Create<Guid[]>();
            var property = CreateNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
                property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
                _thing.NullableGuid.Should().Be(value);
            }
        }
        
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(DateTime))]
        public void TrySetNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"""{_fixture.Create<DateTime>()}""";
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNullableWitInvalidValueAndNotHaveValueInEnum()
        {
            var values = _fixture.Create<Guid[]>();
            var property = CreateNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<Guid>()}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        #endregion
        
        public class GuidThing : Thing
        {
            public override string Name => "guid-thing";
            
            public Guid Guid { get; set; }
            public Guid? NullableGuid { get; set; }
        }
    }
}
