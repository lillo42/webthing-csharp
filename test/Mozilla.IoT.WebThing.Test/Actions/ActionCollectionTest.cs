using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Actions
{
    public class ActionCollectionTest
    {
        private readonly IJsonConvertible _jsonConvertible;
        private readonly IJsonSchemaValidation _schemaValidation;
        private readonly Mozilla.IoT.WebThing.Convertibles.IConvertible _convertible;
        private readonly IActionInformationFactory _factory;
        private readonly ActionCollection _collection;
        private readonly Fixture _fixture;

        public ActionCollectionTest()
        {
            _fixture = new Fixture();

            _jsonConvertible = Substitute.For<IJsonConvertible>();
            _schemaValidation = Substitute.For<IJsonSchemaValidation>();
            _convertible = Substitute.For<Mozilla.IoT.WebThing.Convertibles.IConvertible>();
            _factory = Substitute.For<IActionInformationFactory>();
            
            _collection = new ActionCollection(_jsonConvertible, _schemaValidation, _convertible, _factory);
        }

        #region TryAdd

        [Fact]
        public void TryAdd_Should_ReturnFalse_When_TryConvertReturnFalse()
        {
            var source = _fixture.Create<string>();
            _jsonConvertible.TryConvert(source, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = null;
                    return false;
                });


            _collection.TryAdd(source, out _).Should().BeFalse();

            _jsonConvertible
                .Received(1)
                .TryConvert(source, out Arg.Any<object>());
        }
        
        [Fact]
        public void TryAdd_Should_ReturnFalse_When_IsValidReturnFalse()
        {
            var source = _fixture.Create<string>();
            var converted = _fixture.Create<object>();
            
            _jsonConvertible.TryConvert(source, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = converted;
                    return true;
                });
            
            _schemaValidation.IsValid(converted)
                .Returns(false);


            _collection.TryAdd(source, out _).Should().BeFalse();

            _jsonConvertible
                .Received(1)
                .TryConvert(source, out Arg.Any<object>());
            
            _schemaValidation
                .Received(1)
                .IsValid(converted);
        }
        
        [Fact]
        public void TryAdd_Should_ReturnTrue()
        {
            var source = _fixture.Create<string>();
            var converted = _fixture.Create<object>();
            
            _jsonConvertible.TryConvert(source, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = converted;
                    return true;
                });
            
            _schemaValidation.IsValid(converted)
                .Returns(true);

            var convertible = _fixture.Create<Dictionary<string, object>>();
            
            _convertible.Convert(converted)
                .Returns(convertible);

            var info = Substitute.For<ThingActionInformation>();
            
            _factory.Convert(convertible)
                .Returns(info);

            _collection.TryAdd(source, out _).Should().BeTrue();

            _jsonConvertible
                .Received(1)
                .TryConvert(source, out Arg.Any<object>());
            
            _schemaValidation
                .Received(1)
                .IsValid(converted);
            
            _convertible
                .Received(1)
                .Convert(converted);

            _factory
                .Received(1)
                .Convert(convertible);

            _collection.Should().NotBeEmpty();
            _collection.Should().HaveCount(1);
            
            foreach (var item in _collection)
            {
                item.Should().Be(info);
            }
        }

        #endregion

        #region TryGetValue

        [Fact]
        public void TryGetValue_Should_ReturnFalse_When_IdNotExist()
        {
            _collection.TryGetValue(_fixture.Create<Guid>(), out var result).Should().BeFalse();
            result.Should().BeNull();
        }
        
        [Fact]
        public void TryGetValue_Should_ReturnTrue()
        {
            var source = _fixture.Create<string>();
            var converted = _fixture.Create<object>();
            
            _jsonConvertible.TryConvert(source, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = converted;
                    return true;
                });
            
            _schemaValidation.IsValid(converted)
                .Returns(true);

            var convertible = _fixture.Create<Dictionary<string, object>>();
            
            _convertible.Convert(converted)
                .Returns(convertible);

            var info = Substitute.For<ThingActionInformation>();
            
            _factory.Convert(convertible)
                .Returns(info);

            _collection.TryAdd(source, out _).Should().BeTrue();


            _collection.TryGetValue(info.GetId(), out var info2).Should().BeTrue();
            info2.Should().NotBeNull();
            info2.Should().Be(info);

            _jsonConvertible
                .Received(1)
                .TryConvert(source, out Arg.Any<object>());
            
            _schemaValidation
                .Received(1)
                .IsValid(converted);
            
            _convertible
                .Received(1)
                .Convert(converted);

            _factory
                .Received(1)
                .Convert(convertible);

            _collection.Should().NotBeEmpty();
            _collection.Should().HaveCount(1);
        }

        #endregion

        #region TryRemove

        [Fact]
        public void TryRemove_Should_ReturnFalse_When_IdNotExist()
        {
            _collection.TryRemove(_fixture.Create<Guid>(), out var result).Should().BeFalse();
            result.Should().BeNull();
        }
        
        [Fact]
        public void TryRemove_Should_ReturnTrue()
        {
            var source = _fixture.Create<string>();
            var converted = _fixture.Create<object>();
            
            _jsonConvertible.TryConvert(source, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = converted;
                    return true;
                });
            
            _schemaValidation.IsValid(converted)
                .Returns(true);

            var convertible = _fixture.Create<Dictionary<string, object>>();
            
            _convertible.Convert(converted)
                .Returns(convertible);

            var info = Substitute.For<ThingActionInformation>();
            
            _factory.Convert(convertible)
                .Returns(info);

            _collection.TryAdd(source, out _).Should().BeTrue();
            
            _collection.TryRemove(info.GetId(), out var info2).Should().BeTrue();
            info2.Should().NotBeNull();
            info2.Should().Be(info);

            _jsonConvertible
                .Received(1)
                .TryConvert(source, out Arg.Any<object>());
            
            _schemaValidation
                .Received(1)
                .IsValid(converted);
            
            _convertible
                .Received(1)
                .Convert(converted);

            _factory
                .Received(1)
                .Convert(convertible);

            _collection.Should().BeEmpty();
        }

        #endregion

        [Fact]
        public void Change_Should_Raiser_When_EventChanged()
        {
            var source = _fixture.Create<string>();
            var converted = _fixture.Create<object>();
            
            _jsonConvertible.TryConvert(source, out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = converted;
                    return true;
                });
            
            _schemaValidation.IsValid(converted)
                .Returns(true);

            var convertible = _fixture.Create<Dictionary<string, object>>();
            
            _convertible.Convert(converted)
                .Returns(convertible);

            var info = Substitute.For<ThingActionInformation>();
            
            _factory.Convert(convertible)
                .Returns(info);

            _collection.TryAdd(source, out _).Should().BeTrue();

            var counter = 0;
            _collection.Change += (sender, args) =>
            {
                counter++;
                sender.Should().NotBeNull();
                sender.Should().Be(_collection);

                args.Should().NotBeNull();
                args.Should().Be(info);
            };

            info.StatusChanged += Raise.EventWith((object)info, EventArgs.Empty);
            
            _jsonConvertible
                .Received(1)
                .TryConvert(source, out Arg.Any<object>());
            
            _schemaValidation
                .Received(1)
                .IsValid(converted);
            
            _convertible
                .Received(1)
                .Convert(converted);

            _factory
                .Received(1)
                .Convert(convertible);

            counter.Should().Be(1);

        }
    }
}
