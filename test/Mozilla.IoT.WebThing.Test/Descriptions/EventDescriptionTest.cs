using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Description;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Descriptions
{
    public class EventDescriptionTest
    {
        private readonly Fixture _fixture;
        private readonly Event _event;
        private readonly EventDescription _description;

        public EventDescriptionTest()
        {
            _fixture = new Fixture();
            _description = new EventDescription();
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
            
            var result = _description.CreateDescription(_event);
            
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
            
            var result = _description.CreateDescription(_event);
            
            result.Should().BeEquivalentTo(expected);
        }
    }
}
