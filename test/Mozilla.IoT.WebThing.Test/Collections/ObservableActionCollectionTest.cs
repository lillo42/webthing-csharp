using System.Collections.Specialized;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Collections;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Collections
{
    public class ObservableActionCollectionTest
    {
        private readonly Fixture _fixture;

        public ObservableActionCollectionTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Add_Should_EmitAdd_When_ItemAdded()
        {
            var collection = new ObservableActionCollection();

            var actionName = _fixture.Create<string>();
            
            var value = Substitute.For<Action>();
            value.Name.Returns(actionName);

            collection.CollectionChanged += (sender, @event) =>
            {
                sender.Should().Be(collection);
                @event.Action.Should().Be(NotifyCollectionChangedAction.Add);
                @event.NewItems.Should().HaveCount(1);
                @event.NewItems[0].Should().Be(value);
            };
            
            collection.Add(value);
            collection.Should().HaveCount(1);
            collection.Contains(actionName).Should().BeTrue();
        }
    }
}
