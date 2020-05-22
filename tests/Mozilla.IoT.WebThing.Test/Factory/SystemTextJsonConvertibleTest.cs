using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles;
using Mozilla.IoT.WebThing.Convertibles.Collection;
using Mozilla.IoT.WebThing.Convertibles.Number;
using Mozilla.IoT.WebThing.Convertibles.Strings;
using Mozilla.IoT.WebThing.Factories;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Factory
{
    public class SystemTextJsonConvertibleTest
    {
        private readonly ConvertibleFactory _factory;

        public SystemTextJsonConvertibleTest()
        {
            _factory = new ConvertibleFactory();
        }
        
        
        [Theory]
        [InlineData(typeof(bool), typeof(BooleanConvertible))]
        [InlineData(typeof(string), typeof(StringConvertible))]
        [InlineData(typeof(char), typeof(CharConvertible))]
        [InlineData(typeof(DateTime), typeof(DateTimeConvertible))]
        [InlineData(typeof(DateTimeOffset), typeof(DateTimeOffsetConvertible))]
        [InlineData(typeof(Guid), typeof(GuidConvertible))]
        [InlineData(typeof(TimeSpan), typeof(TimeSpanConvertible))]
        [InlineData(typeof(Foo), typeof(EnumConvertible))]
        [InlineData(typeof(byte), typeof(NumberConvertible))]
        [InlineData(typeof(sbyte), typeof(NumberConvertible))]
        [InlineData(typeof(short), typeof(NumberConvertible))]
        [InlineData(typeof(ushort), typeof(NumberConvertible))]
        [InlineData(typeof(int), typeof(NumberConvertible))]
        [InlineData(typeof(uint), typeof(NumberConvertible))]
        [InlineData(typeof(long), typeof(NumberConvertible))]
        [InlineData(typeof(ulong), typeof(NumberConvertible))]
        [InlineData(typeof(float), typeof(NumberConvertible))]
        [InlineData(typeof(double), typeof(NumberConvertible))]
        [InlineData(typeof(decimal), typeof(NumberConvertible))]
        public void CreateNotArray(Type type, Type convertible)
        {
            var code = type.ToTypeCode();

            var result = _factory.Create(code, type);
            result.Should().NotBeNull();
            result.Should().BeAssignableTo(convertible);
        }
        
        
        [Theory]
        [InlineData(typeof(object[]), typeof(ArrayConvertible<object>))]
        [InlineData(typeof(string[]), typeof(ArrayConvertible<string>))]
        [InlineData(typeof(int[]), typeof(ArrayConvertible<int>))]
        [InlineData(typeof(IEnumerable), typeof(ArrayConvertible<object>))]
        [InlineData(typeof(IEnumerable<object>), typeof(LinkedListConvertible<object>))]
        [InlineData(typeof(IEnumerable<char>), typeof(LinkedListConvertible<char>))]
        [InlineData(typeof(IEnumerable<byte>), typeof(LinkedListConvertible<byte>))]
        [InlineData(typeof(ICollection<object>), typeof(LinkedListConvertible<object>))]
        [InlineData(typeof(ICollection<Foo>), typeof(LinkedListConvertible<Foo>))]
        [InlineData(typeof(ICollection<uint>), typeof(LinkedListConvertible<uint>))]
        [InlineData(typeof(ISet<object>), typeof(HashSetConvertible<object>))]
        [InlineData(typeof(ISet<Guid>), typeof(HashSetConvertible<Guid>))]
        [InlineData(typeof(ISet<long>), typeof(HashSetConvertible<long>))]
        [InlineData(typeof(HashSet<object>), typeof(HashSetConvertible<object>))]
        [InlineData(typeof(HashSet<TimeSpan>), typeof(HashSetConvertible<TimeSpan>))]
        [InlineData(typeof(HashSet<double>), typeof(HashSetConvertible<double>))]
        [InlineData(typeof(IList<object>), typeof(ListConvertible<object>))]
        [InlineData(typeof(IList<DateTime>), typeof(ListConvertible<DateTime>))]
        [InlineData(typeof(IList<float>), typeof(ListConvertible<float>))]
        [InlineData(typeof(List<object>), typeof(ListConvertible<object>))]
        [InlineData(typeof(List<DateTimeOffset>), typeof(ListConvertible<DateTimeOffset>))]
        [InlineData(typeof(List<short>), typeof(ListConvertible<short>))]
        public void CreateArray(Type type, Type convertible)
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
