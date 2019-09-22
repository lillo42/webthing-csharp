using System;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Collections;
using NSubstitute;
using Xunit;
using static Xunit.Assert;

namespace Mozilla.IoT.WebThing.Test.Collections
{
    public class PropertyCollectionTest
    {
        private readonly Fixture _fixture;
        private readonly Thing _thing;
        private readonly PropertyCollection _collection;

        public PropertyCollectionTest()
        {
            _fixture = new Fixture();
            _thing = Substitute.For<Thing>();
            _collection = new PropertyCollection(_thing);
        }

        [Fact]
        public void Add_Should_Throw_When_Null()
        {
            Throws<ArgumentNullException>(() => _collection.Add(null));
        }

        [Fact]
        public void Add_Should_AddThing_When_ThingIsNull()
        {
            var href = _fixture.Create<string>();
            _thing.HrefPrefix.Returns(href);

            var property = new Property();

            _collection.Add(property);
            property.Thing.Should().NotBeNull();
            property.HrefPrefix.Should().Be(href);
            _collection.Count.Should().Be(1);
        }
    }
}
