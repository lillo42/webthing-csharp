using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Properties;
using Mozilla.IoT.WebThing.Test.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator
{
    public class PropertyInterceptFactoryTest
    {
        private readonly Fixture _fixture;
        private readonly PropertiesInterceptFactory _factory;

        public PropertyInterceptFactoryTest()
        {
            _fixture = new Fixture();
            _factory = new PropertiesInterceptFactory(new ThingOption());
        }

        [Fact]
        public void Ignore()
        {
            var thing = new PropertyThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
 
            var properties = _factory.Properties;
            properties.Should().NotContainKey(nameof(PropertyThing.Ignore));
        }
        
        [Fact]
        public void SetName()
        {
            var thing = new PropertyThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
        
            var properties = _factory.Properties;
            properties.Should().ContainKey("test");
            properties.Should().NotContainKey(nameof(PropertyThing.Value));
        }
        
        [Theory]
        [InlineData(nameof(PropertyThing.ReadOnly))]
        [InlineData(nameof(PropertyThing.NoPublicSet))]
        [InlineData(nameof(PropertyThing.OtherReadOnly))]
        public void ReadOnlyProperty(string propertyName)
        {
            var thing = new PropertyThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
            
            var properties = _factory.Properties;
            
            properties.Should().ContainKey(propertyName);
            
            var jsonProperty = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{propertyName}"": {_fixture.Create<int>()} }}")
                .GetProperty(propertyName);
            
            properties[propertyName].SetValue(jsonProperty).Should().Be(SetPropertyResult.ReadOnly);
        }

        #region NoNullable

        [Theory]
        [InlineData(nameof(NoNullablePropertyThing.Bool), typeof(bool))]
        [InlineData(nameof(NoNullablePropertyThing.Byte), typeof(byte))]
        [InlineData(nameof(NoNullablePropertyThing.Sbyte), typeof(sbyte))]
        [InlineData(nameof(NoNullablePropertyThing.Short), typeof(short))]
        [InlineData(nameof(NoNullablePropertyThing.UShort), typeof(ushort))]
        [InlineData(nameof(NoNullablePropertyThing.Int), typeof(int))]
        [InlineData(nameof(NoNullablePropertyThing.UInt), typeof(uint))]
        [InlineData(nameof(NoNullablePropertyThing.Long), typeof(long))]
        [InlineData(nameof(NoNullablePropertyThing.ULong), typeof(ulong))]
        [InlineData(nameof(NoNullablePropertyThing.Float), typeof(float))]
        [InlineData(nameof(NoNullablePropertyThing.Double), typeof(double))]
        [InlineData(nameof(NoNullablePropertyThing.Decimal), typeof(decimal))]
        [InlineData(nameof(NoNullablePropertyThing.String), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.DateTime), typeof(DateTime))]
        [InlineData(nameof(NoNullablePropertyThing.DateTimeOffset), typeof(DateTimeOffset))]
        public void SetValidValueNoNullable(string propertyName, Type type)
        { 
            var thing = new NoNullablePropertyThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
            
            var properties = _factory.Properties;
            properties.Should().ContainKey(propertyName);

            var value = GetValue(type);
            var jsonValue = value;
            if (value is bool)
            {
                jsonValue = value.ToString().ToLower();
            }
            else if (value is string)
            {
                jsonValue = $@"""{value}""";
            }
            else if (value is DateTime d)
            {
                jsonValue = $@"""{d:O}""";
            }
            else if (value is DateTimeOffset df)
            {
                jsonValue = $@"""{df:O}""";
            }

            var jsonProperty = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{propertyName}"": {jsonValue} }}")
                .GetProperty(propertyName);
            
            properties[propertyName].SetValue(jsonProperty).Should().Be(SetPropertyResult.Ok);
            properties[propertyName].GetValue().Should().Be(value);
        }
        
        [Theory]
        [InlineData(nameof(NoNullablePropertyThing.Bool), typeof(int))]
        [InlineData(nameof(NoNullablePropertyThing.Byte), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.Sbyte), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.Short), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.UShort), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.Int), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.UInt), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.Long), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.ULong), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.Float), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.Double), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.Decimal), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.String), typeof(bool))]
        [InlineData(nameof(NoNullablePropertyThing.DateTime), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.DateTimeOffset), typeof(string))]
        [InlineData(nameof(NoNullablePropertyThing.DateTime), typeof(int))]
        [InlineData(nameof(NoNullablePropertyThing.DateTimeOffset), typeof(int))]
        public void TrySetInvalidValueNoNullable(string propertyName, Type type)
        { 
            var thing = new NoNullablePropertyThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
            
            var properties = _factory.Properties;
            properties.Should().ContainKey(propertyName);

            var jsonValue = GetValue(type);
            if (jsonValue is bool)
            {
                jsonValue = jsonValue.ToString().ToLower();
            }
            else if (jsonValue is string)
            {
                jsonValue = $@"""{jsonValue}""";
            }
            else if (jsonValue is DateTime d)
            {
                jsonValue = $@"""{d:O}""";
            }
            else if (jsonValue is DateTimeOffset df)
            {
                jsonValue = $@"""{df:O}""";
            }
            
            var value = properties[propertyName].GetValue();
            var jsonProperty = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{propertyName}"": {jsonValue} }}")
                .GetProperty(propertyName);
            
            properties[propertyName].SetValue(jsonProperty).Should().Be(SetPropertyResult.InvalidValue);
            properties[propertyName].GetValue().Should().Be(value);
        }
        
        #endregion

        #region NoNullablePropertyWithValidationThing
        
        [Theory]
        [ClassData(typeof(NoNullablePropertyWithValidationSuccessThingDataGenerator))]
        public void SetValidValueNoNullableWithValidation(string propertyName, object value)
        { 
            var thing = new NoNullablePropertyWithValidationThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });

            var jsonValue = value;
            if (value is string)
            {
                jsonValue = $@"""{value}""";
            }
            var properties = _factory.Properties;
            properties.Should().ContainKey(propertyName);
            
            var jsonProperty = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{propertyName}"": {jsonValue} }}")
                .GetProperty(propertyName);
            
            properties[propertyName].SetValue(jsonProperty).Should().Be(SetPropertyResult.Ok);
            properties[propertyName].GetValue().Should().Be(value);
        }
        
        [Theory]
        [ClassData(typeof(NoNullablePropertyWithInvalidationSuccessThingDataGenerator))]
        public void TrySetInvalidValueNoNullableWithValidation(string propertyName, object value)
        { 
            var thing = new NoNullablePropertyWithValidationThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });

            if (value is string)
            {
                value = $@"""{value}""";
            }
            
            var properties = _factory.Properties;
            properties.Should().ContainKey(propertyName);
            
            var jsonProperty = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{propertyName}"": {value} }}")
                .GetProperty(propertyName);
            
            properties[propertyName].SetValue(jsonProperty).Should().Be(SetPropertyResult.InvalidValue);
        }
        #endregion
        
        #region NullableProperty

        [Theory]
        [ClassData(typeof(NullablePropertyValidGenerator))]
        public void SetValidValueNullableWithValidation(string propertyName, object value)
        {
            var thing = new NullablePropertyThing();
            CodeGeneratorFactory.Generate(thing, new[] {_factory});

            var jsonValue = value;
            switch (value)
            {
                case string _:
                    jsonValue = $@"""{value}""";
                    break;
                case DateTime d:
                    jsonValue = $@"""{d:O}""";
                    break;
                case DateTimeOffset dt:
                    jsonValue = $@"""{dt:O}""";
                    break;
                case bool _:
                    jsonValue = value.ToString().ToLower();
                    break;
                case null:
                    jsonValue = "null";
                    break;
            }

            var properties = _factory.Properties;
            properties.Should().ContainKey(propertyName);
            
            var jsonProperty = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{propertyName}"": {jsonValue} }}")
                .GetProperty(propertyName);
            
            properties[propertyName].SetValue(jsonProperty).Should().Be(SetPropertyResult.Ok);
            properties[propertyName].GetValue().Should().Be(value);
        }
        
        #endregion

        
        
        
        private object GetValue(Type type)
        {
            if (type == typeof(bool))
            {
                return _fixture.Create<bool>();
            }

            if (type == typeof(byte))
            {
                return _fixture.Create<byte>();
            }
            
            if (type == typeof(sbyte))
            {
                return _fixture.Create<sbyte>();
            }
            
            if (type == typeof(short))
            {
                return _fixture.Create<short>();
            }
            
            if (type == typeof(ushort))
            {
                return _fixture.Create<ushort>();
            }
            
            if (type == typeof(int))
            {
                return _fixture.Create<int>();
            }
            
            if (type == typeof(uint))
            {
                return _fixture.Create<uint>();
            }
            
            if (type == typeof(long))
            {
                return _fixture.Create<long>();
            }
            
            if (type == typeof(ulong))
            {
                return _fixture.Create<ulong>();
            }
            
            if (type == typeof(float))
            {
                return _fixture.Create<float>();
            }
            
            if (type == typeof(double))
            {
                return _fixture.Create<double>();
            }
            
            if (type == typeof(decimal))
            {
                return _fixture.Create<decimal>();
            }

            if (type == typeof(DateTime))
            {
                return _fixture.Create<DateTime>();
            }
            
            if (type == typeof(DateTimeOffset))
            {
                return _fixture.Create<DateTimeOffset>();
            }

            return _fixture.Create<string>();
        }

        #region Thing
        public class PropertyThing : Thing
        {
            public override string Name => nameof(PropertyThing);

            [ThingProperty(Name = "test")]
            public string Value { get; set; }

            [ThingProperty(Ignore = true)]
            public bool Ignore { get; set; }

            public int ReadOnly => 1;
            
            public int NoPublicSet { get; private set; }
            
            [ThingProperty(IsReadOnly =  true)]
            public int OtherReadOnly { get; set; }
        }

        public class NoNullablePropertyThing : Thing
        {
            public override string Name => nameof(NoNullablePropertyThing);

            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte Sbyte { get; set; }
            public short Short { get; set; }
            public ushort UShort { get; set; }
            public int Int { get; set; }
            public uint UInt { get; set; }
            public long Long { get; set; }
            public ulong ULong { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public string String { get; set; }
            public DateTime DateTime { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
        }
        
        public class NullablePropertyThing : Thing
        {
            public override string Name => nameof(NullablePropertyThing);

            public bool? Bool { get; set; }
            public byte? Byte { get; set; }
            public sbyte? SByte { get; set; }
            public short? Short { get; set; }
            public ushort? UShort { get; set; }
            public int? Int { get; set; }
            public uint? UInt { get; set; }
            public long? Long { get; set; }
            public ulong? ULong { get; set; }
            public float? Float { get; set; }
            public double? Double { get; set; }
            public decimal? Decimal { get; set; }
            public string? String { get; set; }
            public DateTime? DateTime { get; set; }
            public DateTimeOffset? DateTimeOffset { get; set; }
        }
        
        public class NoNullablePropertyWithValidationThing : Thing
        {
            public override string Name => nameof(NoNullablePropertyWithValidationThing);

            #region Byte

            [ThingProperty(Minimum = 1, Maximum = 100)]
            public byte Byte { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public byte MultipleOfByte { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public byte ExclusiveByte { get; set; }

            #endregion

            #region SByte

            [ThingProperty(Minimum = 1, Maximum = 100)]
            public sbyte SByte { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public sbyte MultipleOfSByte { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public sbyte ExclusiveSByte { get; set; }

            #endregion

            #region Short

            [ThingProperty(Minimum = 1, Maximum = 100)]
            public short Short { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public short MultipleOfShort { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public short ExclusiveShort { get; set; }

            #endregion
            
            #region UShort
            
            [ThingProperty(Minimum = 1, Maximum = 100)]
            public ushort UShort { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public ushort MultipleOfUShort { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public ushort ExclusiveUShort { get; set; }

            #endregion

            #region Int

            [ThingProperty(Minimum = 1, Maximum = 100)]
            public int Int { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public int MultipleOfInt { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public int ExclusiveInt { get; set; }

            #endregion

            #region UInt

            [ThingProperty(Minimum = 1, Maximum = 100)]
            public uint UInt { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public uint MultipleOfUInt { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public uint ExclusiveUInt { get; set; }
            
            #endregion

            #region Long
            
            [ThingProperty(Minimum = 1, Maximum = 100)]
            public long Long { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public long MultipleOfLong { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public long ExclusiveLong { get; set; }

            #endregion

            #region ULong
            
            [ThingProperty(Minimum = 1, Maximum = 100)]
            public ulong ULong { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public ulong MultipleOfULong { get; set; }

            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public ulong ExclusiveULong { get; set; }
            #endregion

            #region Float

            [ThingProperty(Minimum = 1, Maximum = 100)]
            public float Float { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public float MultipleOfFloat { get; set; }

            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public float ExclusiveFloat { get; set; }

            #endregion

            #region Double
            
            [ThingProperty(Minimum = 1, Maximum = 100)]
            public double Double { get; set; }

            [ThingProperty(MultipleOf = 2)]
            public double MultipleOfDouble { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public double ExclusiveDouble { get; set; }
            
            #endregion

            #region Decimal

            [ThingProperty(Minimum = 1, Maximum = 100)]
            public decimal Decimal { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public decimal MultipleOfDecimal { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]
            public double ExclusiveDecimal { get; set; }
            
            #endregion
            
            #region String
            
            [ThingProperty(MinimumLength = 1, MaximumLength = 100)]
            public string String { get; set; }
            
            [ThingProperty(Pattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]
            public string Mail { get; set; }

            #endregion
        }
        #endregion
        
        #region Data generator

        public class NoNullablePropertyWithValidationSuccessThingDataGenerator : IEnumerable<object[]>
        {
            private readonly Fixture _fixture = new Fixture();
            private readonly List<string> _propertyName = new List<string>
            {
                nameof(NoNullablePropertyWithValidationThing.Byte),
                nameof(NoNullablePropertyWithValidationThing.SByte), 
                nameof(NoNullablePropertyWithValidationThing.Short), 
                nameof(NoNullablePropertyWithValidationThing.UShort),
                nameof(NoNullablePropertyWithValidationThing.Int),
                nameof(NoNullablePropertyWithValidationThing.UInt),
                nameof(NoNullablePropertyWithValidationThing.Long),
                nameof(NoNullablePropertyWithValidationThing.ULong), 
                nameof(NoNullablePropertyWithValidationThing.Float), 
                nameof(NoNullablePropertyWithValidationThing.Double),
                nameof(NoNullablePropertyWithValidationThing.Decimal)
            };
            private readonly List<string> _propertyNameMultipleOf = new List<string>
            {
                nameof(NoNullablePropertyWithValidationThing.MultipleOfByte),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfSByte), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfShort), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfUShort),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfInt),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfUInt),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfLong),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfULong), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfFloat), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfDouble),
            };
            
            private readonly List<string> _propertyNameExclusive = new List<string>
            {
                nameof(NoNullablePropertyWithValidationThing.ExclusiveByte),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveSByte), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveShort), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveUShort),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveInt),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveUInt),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveLong),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveULong), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveFloat), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveDouble),
            };
            
            private readonly int[] _values = { 1, 10, 100};
            private readonly int[] _valuesExclusive = { 2, 10, 99};
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var property in _propertyName)
                {
                    foreach (var value in _values)
                    {
                        yield return new object[] { property,  value };
                    }
                }
                
                foreach (var property in _propertyNameMultipleOf)
                {
                    yield return new object[] { property,  10 };
                }
                
                foreach (var property in _propertyNameExclusive)
                {
                    foreach (var value in _valuesExclusive)
                    {
                        yield return new object[] { property,  value };
                    }
                }

                yield return  new object[] { nameof(NoNullablePropertyWithValidationThing.String), _fixture.Create<string>() };
                yield return  new object[] { nameof(NoNullablePropertyWithValidationThing.Mail), "test@teste.com" };
            }

            IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();
        }
        
        public class NoNullablePropertyWithInvalidationSuccessThingDataGenerator : IEnumerable<object[]>
        {
            private readonly Fixture _fixture = new Fixture();
            private readonly List<string> _propertyName = new List<string>
            {
                nameof(NoNullablePropertyWithValidationThing.Byte),
                nameof(NoNullablePropertyWithValidationThing.SByte), 
                nameof(NoNullablePropertyWithValidationThing.Short), 
                nameof(NoNullablePropertyWithValidationThing.UShort),
                nameof(NoNullablePropertyWithValidationThing.Int),
                nameof(NoNullablePropertyWithValidationThing.UInt),
                nameof(NoNullablePropertyWithValidationThing.Long),
                nameof(NoNullablePropertyWithValidationThing.ULong), 
                nameof(NoNullablePropertyWithValidationThing.Float), 
                nameof(NoNullablePropertyWithValidationThing.Double),
                nameof(NoNullablePropertyWithValidationThing.Decimal)
            };
            private readonly List<string> _propertyNameMultipleOf = new List<string>
            {
                nameof(NoNullablePropertyWithValidationThing.MultipleOfByte),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfSByte), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfShort), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfUShort),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfInt),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfUInt),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfLong),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfULong), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfFloat), 
                nameof(NoNullablePropertyWithValidationThing.MultipleOfDouble),
                nameof(NoNullablePropertyWithValidationThing.MultipleOfDecimal),
            };
            
            private readonly List<string> _propertyNameExclusive = new List<string>
            {
                nameof(NoNullablePropertyWithValidationThing.ExclusiveByte),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveSByte), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveShort), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveUShort),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveInt),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveUInt),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveLong),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveULong), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveFloat), 
                nameof(NoNullablePropertyWithValidationThing.ExclusiveDouble),
                nameof(NoNullablePropertyWithValidationThing.ExclusiveDecimal),
            };
            
            private readonly int[] _values = { 0, 101};
            private readonly int[] _valuesExclusive = { 1, 100};
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var property in _propertyName)
                {
                    foreach (var value in _values)
                    {
                        yield return new object[] { property,  value };
                    }
                }
                
                foreach (var property in _propertyNameMultipleOf)
                {
                    yield return new object[] { property,  9 };
                }
                
                foreach (var property in _propertyNameExclusive)
                {
                    foreach (var value in _valuesExclusive)
                    {
                        yield return new object[] { property,  value };
                    }
                }

                var invalid = _fixture.Create<string>() + _fixture.Create<string>() + _fixture.Create<string>() +
                              _fixture.Create<string>();
                yield return  new object[] { nameof(NoNullablePropertyWithValidationThing.String), string.Empty };
                yield return  new object[] { nameof(NoNullablePropertyWithValidationThing.String),  invalid};
                yield return  new object[] { nameof(NoNullablePropertyWithValidationThing.Mail), _fixture.Create<string>() };
            }

            IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();
        }
        
        public class NullablePropertyValidGenerator : IEnumerable<object[]>
        {
            private readonly Fixture _fixture = new Fixture();
            private readonly List<(string, Type)> _propertyName = new List<(string, Type)>
            {
                (nameof(NullablePropertyThing.Bool), typeof(bool)),
                (nameof(NullablePropertyThing.Byte), typeof(byte)),
                (nameof(NullablePropertyThing.SByte), typeof(sbyte)),
                (nameof(NullablePropertyThing.Short), typeof(short)),
                (nameof(NullablePropertyThing.UShort), typeof(ushort)),
                (nameof(NullablePropertyThing.Int), typeof(int)),
                (nameof(NullablePropertyThing.UInt), typeof(uint)),
                (nameof(NullablePropertyThing.Long), typeof(long)),
                (nameof(NullablePropertyThing.ULong),  typeof(ulong)),
                (nameof(NullablePropertyThing.Float),  typeof(float)),
                (nameof(NullablePropertyThing.Double), typeof(double)),
                (nameof(NullablePropertyThing.Decimal), typeof(decimal)),
                (nameof(NullablePropertyThing.String), typeof(string)),
                (nameof(NullablePropertyThing.DateTime), typeof(DateTime)),
                (nameof(NullablePropertyThing.DateTimeOffset), typeof(DateTimeOffset))
            };
            
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (property, type) in _propertyName)
                {
                    yield return new object[]{property, null};
                    yield return new []{property, _fixture.GetValue(type)};
                }
            }

            IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();
            
            
        }
        
        #endregion
    }
}
