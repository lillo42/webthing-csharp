using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Mozilla.IoT.WebThing.Test
{
    public class ThingEventCollectionTest
    {
        private readonly Fixture _fixture;

        public ThingEventCollectionTest()
        {
            _fixture = new Fixture();
        }


        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        public void MaxSize(int size)
        {
            var collection = new ThingEventCollection(size);
            var data = new LinkedList<ThingEvent>();
            
            for (var i = 0; i < size; i++)
            {
                var @event = new ThingEvent(_fixture.Create<object>());
                data.AddLast(@event);
                collection.Add(@event);
            }

            collection.ToArray().Length.Should().Be(size);
            collection.ToArray().Should().BeEquivalentTo(data);

            var event2 = new ThingEvent(_fixture.Create<object>());
            data.AddLast(@event2);
            data.RemoveFirst();
            collection.Add(@event2);
            
            collection.ToArray().Length.Should().Be(size);
            collection.ToArray().Should().BeEquivalentTo(data);
        }
    }
}
