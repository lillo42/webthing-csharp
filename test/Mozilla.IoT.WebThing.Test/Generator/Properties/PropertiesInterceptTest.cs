using System;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Properties;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Boolean;
using Mozilla.IoT.WebThing.Properties.Number;
using Mozilla.IoT.WebThing.Properties.String;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator.Properties
{
    public class PropertiesInterceptTest
    {
        private readonly Fixture _fixture;
        private readonly PropertiesIntercept _intercept;

        public PropertiesInterceptTest()
        {
            _fixture = new Fixture();
            _intercept = new PropertiesIntercept(new ThingOption());
        }

        [Fact]
        public void IgnoreCase()
        {
            var intercept = new PropertiesIntercept(new ThingOption
            {
                IgnoreCase = true
            });
            
            var key = _fixture.Create<string>();
            var property = Substitute.For<IProperty>();
            intercept.Properties.Add(key, property);
            intercept.Properties.Should().ContainKey(key);
            intercept.Properties.Should().ContainKey(key.ToUpper());
        }
        
        [Fact]
        public void NotIgnoreCase()
        {
            var intercept = new PropertiesIntercept(new ThingOption
            {
                IgnoreCase = false
            });
            
            var key = _fixture.Create<string>();
            var property = Substitute.For<IProperty>();
            intercept.Properties.Add(key, property);
            intercept.Properties.Should().ContainKey(key);
            intercept.Properties.Should().NotContainKey(key.ToUpper());
        }
        
        [Theory]
        [InlineData(nameof(PropertyThing.ReadOnlyGetter))]
        [InlineData(nameof(PropertyThing.ReadOnlyPrivateSetter))]
        [InlineData(nameof(PropertyThing.ReadOnlyByAttribute))]
        public void VisitReadOnlyWhenCanWriteIsFalse(string propertyName)
        {
            var thing = new PropertyThing();
            var property = typeof(PropertyThing).GetProperty(propertyName)!;
            _intercept.Visit(thing, property!, property.GetCustomAttribute<ThingPropertyAttribute>());

            _intercept.Properties.Should().ContainKey(propertyName);
            _intercept.Properties[propertyName].Should().NotBeNull();
            _intercept.Properties[propertyName].Should().BeAssignableTo<PropertyReadOnly>();
        }

        [Theory]
        [InlineData(nameof(PropertyThing.Bool), typeof(PropertyBoolean))]
        [InlineData(nameof(PropertyThing.Char), typeof(PropertyChar))]
        [InlineData(nameof(PropertyThing.DateTime), typeof(PropertyDateTime))]
        [InlineData(nameof(PropertyThing.DateTimeOffset), typeof(PropertyDateTimeOffset))]
        [InlineData(nameof(PropertyThing.Guid), typeof(PropertyGuid))]
        [InlineData(nameof(PropertyThing.Enum), typeof(PropertyEnum))]
        [InlineData(nameof(PropertyThing.String), typeof(PropertyString))]
        [InlineData(nameof(PropertyThing.TimeSpan), typeof(PropertyTimeSpan))]
        [InlineData(nameof(PropertyThing.Byte), typeof(PropertyByte))]
        [InlineData(nameof(PropertyThing.Sbyte), typeof(PropertySByte))]
        [InlineData(nameof(PropertyThing.Short), typeof(PropertyShort))]
        [InlineData(nameof(PropertyThing.Ushort), typeof(PropertyUShort))]
        [InlineData(nameof(PropertyThing.Int), typeof(PropertyInt))]
        [InlineData(nameof(PropertyThing.Uint), typeof(PropertyUInt))]
        [InlineData(nameof(PropertyThing.Long), typeof(PropertyLong))]
        [InlineData(nameof(PropertyThing.Ulong), typeof(PropertyULong))]
        [InlineData(nameof(PropertyThing.Float), typeof(PropertyFloat))]
        [InlineData(nameof(PropertyThing.Double), typeof(PropertyDouble))]
        [InlineData(nameof(PropertyThing.Decimal), typeof(PropertyDecimal))]
        public void Execute(string propertyName, Type instanceOf )
        {
            var property = typeof(PropertyThing).GetProperty(propertyName);
            var thing = new PropertyThing();
            
            _intercept.Visit(thing, property, property.GetCustomAttribute<ThingPropertyAttribute>());
            _intercept.Properties.Should().ContainKey(property.Name);
            _intercept.Properties[property.Name].Should().BeAssignableTo(instanceOf);
        }
        
        #region Thing

        public class PropertyThing : Thing
        {
            public override string Name => "property-thing";
            
            public object ReadOnlyGetter { get; } = new object();
            public object ReadOnlyPrivateSetter { get; private set; } = new object();
            
            [ThingProperty(IsReadOnly = true)]
            public object ReadOnlyByAttribute { get; set; } = new object();

            public bool Bool { get; set; }
            
            public char Char { get; set; }
            public DateTime DateTime { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
            public Guid Guid { get; set; }
            public Foo Enum { get; set; }
            public string String { get; set; }
            public TimeSpan TimeSpan { get; set; }

            public byte Byte { get; set; }
            public sbyte Sbyte { get; set; }
            public short Short { get; set; }
            public ushort Ushort { get; set; }
            public int Int { get; set; }
            public uint Uint { get; set; }
            public long Long { get; set; }
            public ulong Ulong { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            
        }
        
        public enum Foo
        {
            A,
            B,
            C
        }

        #endregion
    }
}
