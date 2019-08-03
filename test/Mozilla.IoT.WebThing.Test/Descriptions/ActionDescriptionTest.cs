using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Description;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Descriptions
{
    public class ActionDescriptionTest
    {
        private readonly Fixture _fixture;
        private readonly Action _action;
        private readonly ActionDescriptor _descriptor;

        public ActionDescriptionTest()
        {
            _fixture = new Fixture();
            _descriptor = new ActionDescriptor();
            _action = Substitute.For<Action>();
        }

        [Fact]
        public void CreateDescription_Should_ReturnWithMinimumValue_When_ActionIsCreated()
        {
            string href = _fixture.Create<string>();
            string hrefPrefix = _fixture.Create<string>();

            _action.HrefPrefix.Returns(hrefPrefix);
            _action.Href.Returns(href);
            _action.Status.Returns(Status.Created);
            _action.TimeRequested.Returns(DateTime.UtcNow);

            var expected = new Dictionary<string, object>
            {
                ["href"] = $"{_action.HrefPrefix.JoinUrl(_action.Href)}",
                ["timeRequested"] = _action.TimeRequested,
                ["status"] = "created"
            };

            var result = _descriptor.CreateDescription(_action);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void CreateDescription_Should_ReturnWitInput_When_ActionHaveAction()
        {
            string href = _fixture.Create<string>();
            string hrefPrefix = _fixture.Create<string>();
            var input = new Dictionary<string, object> {["level"] = 50, ["duration"] = 2_000};

            _action.HrefPrefix.Returns(hrefPrefix);
            _action.Href.Returns(href);
            _action.Status.Returns(Status.Created);
            _action.TimeRequested.Returns(DateTime.UtcNow);
            _action.Input.Returns(input);

            var expected = new Dictionary<string, object>
            {
                ["href"] = $"{_action.HrefPrefix.JoinUrl(_action.Href)}",
                ["timeRequested"] = _action.TimeRequested,
                ["status"] = "created",
                ["input"] = input
            };

            var result = _descriptor.CreateDescription(_action);

            result.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public void CreateDescription_Should_ChangeStatus_When_StatusChange()
        {
            string href = _fixture.Create<string>();
            string hrefPrefix = _fixture.Create<string>();
            var input = new Dictionary<string, object> {["level"] = 50, ["duration"] = 2_000};

            _action.HrefPrefix.Returns(hrefPrefix);
            _action.Href.Returns(href);
            _action.TimeRequested.Returns(DateTime.UtcNow);
            _action.Input.Returns(input);
            _action.Status.Returns(Status.Pending);

            var expected = new Dictionary<string, object>
            {
                ["href"] = $"{_action.HrefPrefix.JoinUrl(_action.Href)}",
                ["timeRequested"] = _action.TimeRequested,
                ["status"] = "pending",
                ["input"] = input
            };

            var result = _descriptor.CreateDescription(_action);

            result.Should().BeEquivalentTo(expected);

            _action.TimeCompleted.Returns(DateTime.UtcNow);
            _action.Status.Returns(Status.Completed);

            expected = new Dictionary<string, object>
            {
                ["href"] = $"{_action.HrefPrefix.JoinUrl(_action.Href)}",
                ["timeRequested"] = _action.TimeRequested,
                ["timeCompleted"] = _action.TimeCompleted,
                ["status"] = "completed",
                ["input"] = input
            };

            result = _descriptor.CreateDescription(_action);

            result.Should().BeEquivalentTo(expected);
        }
    }
}
