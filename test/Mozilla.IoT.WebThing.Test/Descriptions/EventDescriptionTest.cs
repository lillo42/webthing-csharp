using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Descriptor;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Descriptions
{
    public class EventDescriptionTest
    {
        private readonly Fixture _fixture;
        private readonly Event _event;
        private readonly EventDescriptor _descriptor;

        public EventDescriptionTest()
        {
            _fixture = new Fixture();
            _descriptor = new EventDescriptor();
            _event = Substitute.For<Event>();
        }
        
        [Fact]
        public void CreateDescription_Should_ReturnOnlyTimestamp_When_DoesNotHaveData()
        {
            _event.Time.Returns(DateTime.UtcNow);

            var expected = new Dictionary<string, object>
            {
                ["timestamp"] =_event.Time 
            };
            
            var result = _descriptor.CreateDescription(_event);
            
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void CreateDescription_Should_ReturnFullData_When_HaveData()
        {
            _event.Time.Returns(DateTime.UtcNow);
            _event.Data.Returns(_fixture.Create<string>());

            var expected = new Dictionary<string, object>
            {
                ["timestamp"] =_event.Time,
                ["data"] = _event.Data
            };
            
            var result = _descriptor.CreateDescription(_event);
            
            result.Should().BeEquivalentTo(expected);
        }
    }
}
