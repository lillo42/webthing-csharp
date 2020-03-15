using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Properties.String;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Properties.Strings
{
    public class PropertyTimeSpanTest
    {
        private readonly TimeSpanThing _thing;
        private readonly Fixture _fixture;

        public PropertyTimeSpanTest()
        {
            _fixture = new Fixture();
            _thing = new TimeSpanThing();
        }
        
        #region No Nullable
        private PropertyTimeSpan CreateNoNullable(TimeSpan[]? enums = null)
            => new PropertyTimeSpan(_thing,
                thing => ((TimeSpanThing)thing).TimeSpan,
                (thing, value) => ((TimeSpanThing)thing).TimeSpan = (TimeSpan)value,
                false, enums);

        [Fact]
        public void SetNoNullableWithValue()
        {
            var value = _fixture.Create<TimeSpan>();
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.TimeSpan.Should().Be(value);
        }
        
        [Fact]
        public void SetNoNullableWithValueEnums()
        {
            var values = _fixture.Create<TimeSpan[]>();
            var property = CreateNoNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
                property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
                _thing.TimeSpan.Should().Be(value);
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
        [InlineData(typeof(string))]
        public void TrySetNoNullableWitInvalidValue(Type type)
        {
            var value = type == typeof(int) ? _fixture.Create<int>().ToString() : $@"""{_fixture.Create<string>()}""";
            var property = CreateNoNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNoNullableWithEnumValue()
        {
            var values = _fixture.Create<TimeSpan[]>();
            var property = CreateNoNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<TimeSpan>()}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        #endregion
        
        #region Nullable

        private PropertyTimeSpan CreateNullable(TimeSpan[] enums = null)
            => new PropertyTimeSpan(_thing,
                thing => ((TimeSpanThing)thing).NullableTimeSpan,
                (thing, value) => ((TimeSpanThing)thing).NullableTimeSpan = (TimeSpan?)value,
                true, enums);

        [Fact]
        public void SetNullableWithValue()
        {
            var value = _fixture.Create<TimeSpan>();
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableTimeSpan.Should().NotBeNull();
            _thing.NullableTimeSpan.Should().Be(value);
        }
        
        [Fact]
        public void SetNullableWithNullValue()
        {
            var property = CreateNullable();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
            _thing.NullableTimeSpan.Should().BeNull();
        }
        
        [Fact]
        public void SetNullableWithValueEnums()
        {
            var values = _fixture.Create<TimeSpan[]>();
            var property = CreateNullable(values);
            foreach (var value in values)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}");
                property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.Ok);
                _thing.NullableTimeSpan.Should().Be(value);
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
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        [Fact]
        public void TrySetNullableWitInvalidValueAndNotHaveValueInEnum()
        {
            var values = _fixture.Create<TimeSpan[]>();
            var property = CreateNullable(values);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{_fixture.Create<TimeSpan>()}"" }}");
            property.SetValue(jsonElement.GetProperty("input")).Should().Be(SetPropertyResult.InvalidValue);
        }
        
        #endregion
        
        public class TimeSpanThing : Thing
        {
            public override string Name => "time-span-thing";
            
            public TimeSpan TimeSpan { get; set; }
            public TimeSpan? NullableTimeSpan { get; set; }
        }
    }
}
