using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using NSubstitute;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;
using TypeCode = Mozilla.IoT.WebThing.Factories.TypeCode;

namespace Mozilla.IoT.WebThing.Test.Factory
{
    public class PropertyFactoryTest
    {
        private readonly FakeThing _thing;
        private readonly IJsonSchemaValidationFactory _jsonSchemaValidationFactory;
        private readonly IJsonConvertibleFactory _jsonConvertibleFactory;
        private readonly IConvertibleFactory _convertibleFactory;
        
        private readonly Action<object, object> _setter;
        private readonly Func<object, object> _getter;
        private readonly JsonSchema _jsonSchema;

        private readonly PropertyFactory _factory;
        private readonly Fixture _fixture;

        public PropertyFactoryTest()
        {
            _jsonSchemaValidationFactory = Substitute.For<IJsonSchemaValidationFactory>();
            _jsonConvertibleFactory = Substitute.For<IJsonConvertibleFactory>();
            _convertibleFactory = Substitute.For<IConvertibleFactory>();

            _fixture = new Fixture();
            
            _jsonSchema = new JsonSchema(Substitute.For<IJsonSchema>(), _fixture.Create<object[]>(), 
                _fixture.Create<JsonType>() , _fixture.Create<string>(), _fixture.Create<bool>());
            
            _thing = new FakeThing();
            
            _setter = (thing, value) => ((FakeThing)thing).Value = value;
            _getter = thing => ((FakeThing)thing).Value;
            
            _factory = new PropertyFactory(_jsonSchemaValidationFactory, _jsonConvertibleFactory, _convertibleFactory);
        }

        [Fact]
        public void Ctor_Should_Throw_When_JsonSchemaValidationFactoryIsNull()
            => Assert.Throws<ArgumentNullException>(() =>
                new PropertyFactory(null, _jsonConvertibleFactory, _convertibleFactory));
        
        [Fact]
        public void Ctor_Should_Throw_When_JsonConvertibleFactoryIsNull()
            => Assert.Throws<ArgumentNullException>(() =>
                new PropertyFactory(_jsonSchemaValidationFactory, null, _convertibleFactory));
        
        
        [Fact]
        public void Ctor_Should_Throw_When_ConvertibleFactoryIsNull()
            => Assert.Throws<ArgumentNullException>(() =>
                new PropertyFactory(_jsonSchemaValidationFactory, _jsonConvertibleFactory, null));


        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(string))]
        [InlineData(typeof(char))]
        [InlineData(typeof(char?))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(DateTimeOffset?))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(Guid?))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(TimeSpan?))]
        [InlineData(typeof(Foo))]
        [InlineData(typeof(Foo?))]
        public void Create_Should_ReturnNewInstance_When_TypeDoesNotNeedConvertible(Type type)
        {
            var jsonConvertible = Substitute.For<IJsonConvertible>();
            var validation = Substitute.For<IJsonSchemaValidation>();
            
            var notNullableType = type.GetUnderlyingType();
            var code = notNullableType.ToTypeCode();
            
            _jsonConvertibleFactory.Create(code, notNullableType)
                .Returns(jsonConvertible);
            
            _jsonSchemaValidationFactory.Create(code, _jsonSchema, Arg.Any<Type>())
                .Returns(validation);


            var result = _factory.Create(type, _jsonSchema, _thing, _setter, _getter, _fixture.Create<string>());
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ThingProperty>();
            
            _jsonConvertibleFactory
                .Received(1)
                .Create(code, notNullableType);
            
            _jsonSchemaValidationFactory
                .Received(1)
                .Create(code, _jsonSchema, Arg.Any<Type>());

            _convertibleFactory
                .DidNotReceive()
                .Create(Arg.Any<TypeCode>(), Arg.Any<Type>());
        }
        
        [Theory]
        [InlineData(typeof(byte))]
        [InlineData(typeof(byte?))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(sbyte?))]
        [InlineData(typeof(short))]
        [InlineData(typeof(short?))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(ushort?))]
        [InlineData(typeof(int))]
        [InlineData(typeof(int?))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(uint?))]
        [InlineData(typeof(long))]
        [InlineData(typeof(long?))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(ulong?))]
        [InlineData(typeof(float))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(decimal?))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(object[]))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(IEnumerable<object>))]
        [InlineData(typeof(ISet<string>))]
        [InlineData(typeof(ISet<int>))]
        [InlineData(typeof(ISet<object>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(List<object>))]
        public void Create_Should_ReturnNewInstance_When_TypeNeedConvertible(Type type)
        {
            var jsonConvertible = Substitute.For<IJsonConvertible>();
            var validation = Substitute.For<IJsonSchemaValidation>();
            var convertible = Substitute.For<IConvertible>();
            
            var notNullableType = type.GetUnderlyingType();
            var code = notNullableType.ToTypeCode();
            
            _jsonConvertibleFactory.Create(code, notNullableType)
                .Returns(jsonConvertible);
            
            _jsonSchemaValidationFactory.Create(code, _jsonSchema, Arg.Any<Type>())
                .Returns(validation);
            
            _convertibleFactory
                .Create(code, notNullableType)
                .Returns(convertible);


            var result =_factory.Create(type, _jsonSchema, _thing, _setter, _getter, _fixture.Create<string>());
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ThingProperty>();
            
            _jsonConvertibleFactory
                .Received(1)
                .Create(code, notNullableType);
            
            _jsonSchemaValidationFactory
                .Received(1)
                .Create(code, _jsonSchema, Arg.Any<Type>());

            _convertibleFactory
                .Received(1)
                .Create(code, notNullableType);
        }
        
        public class FakeThing : Thing
        {
            public override string Name => "fake-thing";
            
            public object Value { get; set; }
        }
        
        public enum Foo
        {
            A,
            B,
            C
        }
    }
}
