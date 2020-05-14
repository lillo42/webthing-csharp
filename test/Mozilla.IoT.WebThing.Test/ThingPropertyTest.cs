using System;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test
{
    public class ThingPropertyTest
    {
        private readonly Fixture _fixture;
        private readonly FakeThing _thing;
        private readonly IJsonConvertible _jsonConvertible;
        private readonly IJsonSchemaValidation _jsonSchemaValidation;
        private readonly Mozilla.IoT.WebThing.Convertibles.IConvertible _convertible;
        
        private readonly Action<Thing, object> _setter;
        private readonly Func<Thing, object> _getter;
        private readonly string _originPropertyName;
        public ThingPropertyTest()
        {
            _fixture = new Fixture();
            _thing = new FakeThing();
            _jsonConvertible = Substitute.For<IJsonConvertible>();
            _jsonSchemaValidation = Substitute.For<IJsonSchemaValidation>();
            _convertible = Substitute.For<Mozilla.IoT.WebThing.Convertibles.IConvertible>();

            _setter = (thing, value) => ((FakeThing)thing).Value = value;
            _getter = thing => ((FakeThing)thing).Value;

            _originPropertyName = _fixture.Create<string>();
        }

        [Fact]
        public void Ctor_Should_ThrowArgumentNullException_When_ThingIsNull()
            => Assert.Throws<ArgumentNullException>(() => new ThingProperty(null, false, false,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName));
        [Fact]
        public void Ctor_Should_ArgumentNullException_When_GetterIsNull()
            => Assert.Throws<ArgumentNullException>(() => new ThingProperty(_thing, false, false,
                null, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName));

        [Fact]
        public void Ctor_Should_ArgumentNullException_When_IsNotReadOnlyAndSetterIsNull()
            => Assert.Throws<ArgumentNullException>(() => new ThingProperty(_thing, false, false,
                _getter, null, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName));
        
        [Fact]
        public void Ctor_Should_ArgumentNullException_When_IsNotReadOnlyAndJsonSchemaValidationIsNull()
            => Assert.Throws<ArgumentNullException>(() => new ThingProperty(_thing, false, false,
                _getter, _setter, null, _jsonConvertible, _convertible, _originPropertyName));
        
        [Fact]
        public void Ctor_Should_ArgumentNullException_When_IsNotReadOnlyAndJsonConvertibleIsNull()
            => Assert.Throws<ArgumentNullException>(() => new ThingProperty(_thing, false, false,
                _getter, _setter, _jsonSchemaValidation, null, _convertible, _originPropertyName));

        [Fact]
        public void TryGetValue_Should_ReturnTrue_When_IsNotWriteOnly()
        {
            var property = new ThingProperty(_thing, false, false,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName);

            _thing.Value = _fixture.Create<string>();

            property.TryGetValue(out var value).Should().BeTrue();
            value.Should().Be(_thing.Value);
        }
        
        [Fact]
        public void TryGetValue_Should_ReturnFalse_When_IsWriteOnly()
        {
            var property = new ThingProperty(_thing, false, true,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName);

            _thing.Value = _fixture.Create<string>();

            property.TryGetValue(out var value).Should().BeFalse();
            value.Should().NotBe(_thing.Value);
            value.Should().BeNull();
        }
        
        [Fact]
        public void TrySetValue_Should_ReturnReadOnly_When_IsReadOnly()
        {
            var property = new ThingProperty(_thing, true, false,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName);

            var value = _fixture.Create<string>();

            property.TrySetValue(value).Should().Be(SetPropertyResult.ReadOnly);
            _thing.Value.Should().NotBe(value);
            
            _jsonConvertible
                .DidNotReceive()
                .TryConvert(Arg.Any<object>(), out Arg.Any<object>());
            
            _jsonSchemaValidation
                .DidNotReceive()
                .IsValid(Arg.Any<object>());
        }
        
        [Fact]
        public void TrySetValue_Should_ReturnInvalid_When_TryConvertIsFalse()
        {
            var property = new ThingProperty(_thing, false, false,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName);
            
            var value = _fixture.Create<string>();

            _jsonConvertible.TryConvert(value, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = null;
                    return false;
                });
            
            property.TrySetValue(value).Should().Be(SetPropertyResult.InvalidValue);
            _thing.Value.Should().NotBe(value);
            
            _jsonConvertible
                .Received(1)
                .TryConvert(value, out Arg.Any<object>());
            
            _jsonSchemaValidation
                .DidNotReceive()
                .IsValid(Arg.Any<object>());
        }
        
        [Fact]
        public void TrySetValue_Should_ReturnInvalid_When_IsValidIsFalse()
        {
            var property = new ThingProperty(_thing, false, false,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName);
            
            var value = _fixture.Create<string>();
            var jsonValue = _fixture.Create<string>();

            _jsonConvertible.TryConvert(value, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = jsonValue;
                    return true;
                });
            
            _jsonSchemaValidation.IsValid(jsonValue)
                .Returns(false);
            
            property.TrySetValue(value).Should().Be(SetPropertyResult.InvalidValue);
            _thing.Value.Should().NotBe(value);
            _thing.Value.Should().NotBe(jsonValue);

            _jsonSchemaValidation
                .Received(1)
                .IsValid(jsonValue);
            
            _jsonConvertible
                .Received(1)
                .TryConvert(value, out Arg.Any<object>());
        }
        
        [Fact]
        public void TrySetValue_Should_ReturnOk_When_ConvertibleIsNull()
        {
            var property = new ThingProperty(_thing, false, false,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, null, _originPropertyName);
            
            var value = _fixture.Create<string>();
            var jsonValue = _fixture.Create<string>();

            _jsonConvertible.TryConvert(value, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = jsonValue;
                    return true;
                });
            
            _jsonSchemaValidation.IsValid(jsonValue)
                .Returns(true);
            
            property.TrySetValue(value).Should().Be(SetPropertyResult.Ok);
            _thing.Value.Should().Be(jsonValue);

            _jsonSchemaValidation
                .Received(1)
                .IsValid(jsonValue);
            
            _jsonConvertible
                .Received(1)
                .TryConvert(value, out Arg.Any<object>());
        }
        
        [Fact]
        public void TrySetValue_Should_ReturnOk_When_ConvertibleIsNotNull()
        {
            var property = new ThingProperty(_thing, false, false,
                _getter, _setter, _jsonSchemaValidation, _jsonConvertible, _convertible, _originPropertyName);
            
            var value = _fixture.Create<string>();
            var jsonValue = _fixture.Create<string>();
            var finalValue = _fixture.Create<string>();

            _jsonConvertible.TryConvert(value, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = jsonValue;
                    return true;
                });
            
            _jsonSchemaValidation.IsValid(jsonValue)
                .Returns(true);

            _convertible.Convert(jsonValue)
                .Returns(finalValue);
            
            property.TrySetValue(value).Should().Be(SetPropertyResult.Ok);
            _thing.Value.Should().Be(finalValue);

            _jsonSchemaValidation
                .Received(1)
                .IsValid(jsonValue);
            
            _jsonConvertible
                .Received(1)
                .TryConvert(value, out Arg.Any<object>());

            _convertible
                .Received(1)
                .Convert(jsonValue);
        }
        
        public class FakeThing : Thing
        {
            public override string Name => "fake-thing";
            
            public object Value { get; set; }
        }
    }
}
