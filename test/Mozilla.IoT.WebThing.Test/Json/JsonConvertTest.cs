using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Json;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Json
{
    public class JsonConvertTest
    {
        private readonly Fixture _fixture;
        private readonly DefaultJsonConvert _convert;

        public JsonConvertTest()
        {
            _fixture = new Fixture();
            _convert = new DefaultJsonConvert(new DefaultJsonSerializerSettings(new JsonSerializerOptions
            {
               IgnoreNullValues = true,
               WriteIndented = false
            }));
        }

        [Fact]
        public void Deserialize_Should_ReturnDictionary_When_HaveASimpleStruct()
        {
            string key = _fixture.Create<string>();
            string value = _fixture.Create<string>();
            var json = Encoding.UTF8.GetBytes($@"{{
            ""{key}"": ""{value}"" 
            }}");

            var result = _convert.Deserialize<Dictionary<string, object>>(new ReadOnlySpan<byte>(json));

            result.Should().HaveCount(1);
            result.Should().ContainKey(key);
            ((JsonElement)result[key]).GetString().Should().Be(value);
        }
    }
}
