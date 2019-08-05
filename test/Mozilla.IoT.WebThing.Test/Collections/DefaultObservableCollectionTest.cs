using System.Collections.Specialized;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Collections;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Collections
{
    public class DefaultObservableCollectionTest
    {
        private readonly Fixture _fixture;

        public DefaultObservableCollectionTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Add_Should_EmitAddEvent_When_ItemAdded()
        {
            var collection = new DefaultObservableCollection<string>();
            string value = _fixture.Create<string>();

            collection.CollectionChanged += (sender, @event) =>
            {
                sender.Should().Be(collection);
                @event.Action.Should().Be(NotifyCollectionChangedAction.Add);
                @event.NewItems.Should().HaveCount(1);
                @event.NewItems[0].Should().BeOfType<string>();
                @event.NewItems[0].Should().Be(value);
            };
            
            collection.Add(value);
            collection.Should().HaveCount(1);
        }
        
        [Fact]
        public void Clear_Should_EmitRestEvent()
        {
            var collection = new DefaultObservableCollection<string>
            {
                _fixture.Create<string>()
            };

            collection.CollectionChanged += (sender, @event) =>
            {
                sender.Should().Be(collection);
                @event.Action.Should().Be(NotifyCollectionChangedAction.Reset);
                @event.NewItems.Should().BeNull();
            };
            
            collection.Clear();
        }
        
        [Fact]
        public void Remove_Should_EmitRemoveEvent_When_ItemIsRemoved()
        {
            var value = _fixture.Create<string>();
            var collection = new DefaultObservableCollection<string>
            {
                value
            };

            collection.CollectionChanged += (sender, @event) =>
            {
                sender.Should().Be(collection);
                @event.Action.Should().Be(NotifyCollectionChangedAction.Remove);
                @event.OldItems.Should().NotBeNull();
                @event.OldItems.Should().HaveCount(1);
                @event.OldItems[0].Should().BeOfType<string>();
                @event.OldItems[0].Should().Be(value);
            };
            
            collection.Remove(value).Should().BeTrue();
        }

        [Fact]
        public void Remove_Should_NotEmitRemoveEvent_When_ItemIsNotRemoved()
        {
            var collection = new DefaultObservableCollection<string>
            {
                _fixture.Create<string>()
            };

            collection.CollectionChanged += (sender, @event) =>
            {
                false.Should().BeTrue();
            };
            
            collection.Remove(_fixture.Create<string>()).Should().BeFalse();
        }
        
        [Fact]
        public void Contains_Should_ReturnTrue_When_ContainsItem()
        {
            var value = _fixture.Create<string>();
            var collection = new DefaultObservableCollection<string>
            {
                value
            };

            collection.Contains(value).Should().BeTrue();
        }
        
        [Fact]
        public void Contains_Should_ReturnFalse_When_NotContainsItem()
        {
            var collection = new DefaultObservableCollection<string>
            {
                _fixture.Create<string>()
            };

            collection.Contains(_fixture.Create<string>()).Should().BeFalse();
        }
    }
}
