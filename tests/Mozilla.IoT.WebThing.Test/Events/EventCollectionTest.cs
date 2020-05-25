using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Events;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test
{
    public class EventCollectionTest
    {
        private readonly Fixture _fixture;

        public EventCollectionTest()
        {
            _fixture = new Fixture();
        }


        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        public void MaxSize(int size)
        {
            var collection = new EventCollection(size);
            var data = new LinkedList<Event>();
            
            for (var i = 0; i < size; i++)
            {
                var @event = new Event(_fixture.Create<object>());
                data.AddLast(@event);
                collection.Enqueue(@event, "", Substitute.For<Thing>());
            }

            collection.ToArray().Length.Should().Be(size);
            collection.ToArray().Should().BeEquivalentTo(data);

            var event2 = new Event(_fixture.Create<object>());
            data.AddLast(@event2);
            data.RemoveFirst();
            collection.Enqueue(@event2, "", Substitute.For<Thing>());
            
            collection.ToArray().Length.Should().Be(size);
            collection.ToArray().Should().BeEquivalentTo(data);
        }
    }
}
