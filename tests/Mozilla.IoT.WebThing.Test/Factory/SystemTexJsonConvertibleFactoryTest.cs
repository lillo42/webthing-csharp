using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.Convertibles.Array;
using Mozilla.IoT.WebThing.Json.Convertibles.String;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Factory
{
    public class SystemTexJsonConvertibleFactoryTest
    {
        private readonly SystemTexJsonConvertibleFactory _factory;

        public SystemTexJsonConvertibleFactoryTest()
        {
            _factory = new SystemTexJsonConvertibleFactory();
        }

        
        [Theory]
        [InlineData(typeof(bool), typeof(SystemTexJsonBooleanConvertible))]
        [InlineData(typeof(string), typeof(SystemTexJsonStringConvertible))]
        [InlineData(typeof(char), typeof(SystemTexJsonCharConvertible))]
        [InlineData(typeof(DateTime), typeof(SystemTexJsonDateTimeConvertible))]
        [InlineData(typeof(DateTimeOffset), typeof(SystemTexJsonDateTimeOffsetConvertible))]
        [InlineData(typeof(Guid), typeof(SystemTexJsonGuidConvertible))]
        [InlineData(typeof(TimeSpan), typeof(SystemTexJsonTimeSpanConvertible))]
        [InlineData(typeof(Foo), typeof(SystemTexJsonEnumConvertible))]
        [InlineData(typeof(byte), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(sbyte), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(short), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(ushort), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(int), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(uint), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(long), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(ulong), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(float), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(double), typeof(SystemTexJsonNumberConvertible))]
        [InlineData(typeof(decimal), typeof(SystemTexJsonNumberConvertible))]
        //[InlineData(typeof(object[]), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(int[]), typeof(SystemTexJsonArrayConvertible))]
        //[InlineData(typeof(IEnumerable), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(IEnumerable<int>), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(IList<int>), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(List<int>), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(LinkedList<int>), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(HashSet<int>), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(ISet<int>), typeof(SystemTexJsonArrayConvertible))]
        [InlineData(typeof(ICollection<int>), typeof(SystemTexJsonArrayConvertible))]
        public void Create(Type type, Type convertible)
        {
            var code = type.ToTypeCode();

            var result = _factory.Create(code, type);
            result.Should().NotBeNull();
            result.Should().BeAssignableTo(convertible);
        }
        
        public enum Foo
        {
            A,
            B,
            C
        }
    }
}
