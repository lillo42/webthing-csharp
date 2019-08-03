using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Activator
{
    public class ActionActivatorTest
    {
        private readonly Fixture _fixture;
        private readonly Thing _thing;
        private readonly IActionActivator _activator;
        private readonly IServiceProvider _service;

        public ActionActivatorTest()
        {
            _fixture = new Fixture();
            _activator = new ActionActivator();
            _thing = new Thing();
            _service = Substitute.For<IServiceProvider>();
        }

        [Fact]
        public void CreateInstance_Should_ReturnNull_When_ActionIsNotRegister()
        {
            var action = _activator.CreateInstance(_service, _thing, _fixture.Create<string>(), null);
            action.Should().BeNull();
        }
        
        [Fact]
        public void CreateInstance_Should_ReturnNewAction_When_ActionIsRegister()
        {
            string actionName = _fixture.Create<string>();
            string hrefPrefix = _fixture.Create<string>();
            
            _thing.HrefPrefix = hrefPrefix;
            _thing.AddAction<TestAction>(actionName);
            _service.GetService(Arg.Any<Type>()).Returns(new TestAction());

            var action = _activator.CreateInstance(_service, _thing, actionName, null);
            
            action.Should().NotBeNull();
            action.Thing.Should().Be(_thing);
            action.Name.Should().Be(actionName);
            action.HrefPrefix.Should().Be(hrefPrefix);
            action.Href.Should().Be($"/actions/{actionName}/{action.Id}");
            action.Input.Should().BeNull();
            _thing.Actions.Contains(actionName).Should().BeTrue();
        }
        
        private class TestAction : Action
        {
            
            protected override Task ExecuteAsync(CancellationToken cancellation) 
                => Task.CompletedTask;
        }
    }
}
