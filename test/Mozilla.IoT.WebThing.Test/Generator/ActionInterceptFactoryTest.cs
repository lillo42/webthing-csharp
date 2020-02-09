using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Actions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator
{
    public class ActionInterceptFactoryTest
    {
        private readonly Fixture _fixture;
        private readonly LampThing _thing;
        private readonly ActionInterceptFactory _factory;
        private readonly IServiceProvider _provider;


        public ActionInterceptFactoryTest()
        {
            _fixture = new Fixture();
            _thing = new LampThing();
            _factory = new ActionInterceptFactory(new ThingOption());
            var logger = Substitute.For<ILogger<ActionInfo>>();
            _provider = Substitute.For<IServiceProvider>();

            _provider.GetService(typeof(ILogger<ActionInfo>))
                .Returns(logger);
        }

        [Fact]
        public void Ignore()
        {
            CodeGeneratorFactory.Generate(_thing, new[] {_factory});
            _factory.Actions.Should().NotContainKey(nameof(LampThing.Ignore));
        }


        private ActionInfo CreateAction(string actionName)
        {
            CodeGeneratorFactory.Generate(_thing, new[] {_factory});
            _factory.Actions.Should().ContainKey(actionName);

            _thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(),
                _factory.Actions);
            var actionType = _thing.ThingContext.Actions[actionName].ActionType;
            return (ActionInfo)Activator.CreateInstance(actionType);

        }
        
        private ActionInfo CreateAction(string actionName, int inputValue)
        {
            CodeGeneratorFactory.Generate(_thing, new[] {_factory});
            _factory.Actions.Should().ContainKey(actionName);

            _thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(),
                _factory.Actions);
            var actionType = _thing.ThingContext.Actions[actionName].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);
            
            var inputPropertyType = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputPropertyType.Should().NotBeNull();

            var input = Activator.CreateInstance(inputPropertyType.PropertyType);
            var valueProperty = inputPropertyType.PropertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            valueProperty.Should().NotBeNull();
            
            valueProperty.SetValue(input, inputValue);
            inputPropertyType.SetValue(action, input);
            return action;
        }

        #region Void

        [Fact]
        public async Task VoidWithoutParameter()
        {
            var action = CreateAction(nameof(LampThing.ReturnVoid));
            action.IsValid().Should().BeTrue();

            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();

            _thing.Values.Should().HaveCount(1);
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.ReturnVoid));
        }

        [Fact]
        public async Task VoidWithParameter()
        {
            var value = _fixture.Create<int>();
            var action = CreateAction(nameof(LampThing.ReturnVoidWithParameter), value);
            
            action.IsValid().Should().BeTrue();
            
            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();
            
            _thing.Values.Should().HaveCount(2);
            _thing.Values.First.Value.Should().Be(value.ToString());
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.ReturnVoidWithParameter));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task VoidWithParameterValid(int value)
        {
            var action = CreateAction(nameof(LampThing.ReturnVoidWithParameterWithValidation), value);
            
            action.IsValid().Should().BeTrue();
            
            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();
            
            _thing.Values.Should().HaveCount(2);
            _thing.Values.First.Value.Should().Be(value.ToString());
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.ReturnVoidWithParameterWithValidation));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void VoidWithParameterInvalid(int value)
        {
            var action = CreateAction(nameof(LampThing.ReturnVoidWithParameterWithValidation), value);
            
            action.IsValid().Should().BeFalse();
            
            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            _thing.Values.Should().BeEmpty();
        }
        
        [Fact]
        public async Task Throwable()
        {
            var action = CreateAction(nameof(LampThing.Throwable));
            action.IsValid().Should().BeTrue();

            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();
            
            _thing.Values.Should().HaveCount(1);
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.Throwable));
        }
        
        [Fact]
        public async Task FromService()
        {
            var value = 10;
            var action = CreateAction(nameof(LampThing.ReturnParameterWithValidationAndParameterFromService), value);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            
            var something = Substitute.For<ISomething>();

            _provider.GetService(typeof(ISomething))
                .Returns(something);
            
            action.IsValid().Should().BeTrue();
            
            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();
            
            _thing.Values.Should().HaveCount(2);
            _thing.Values.First.Value.Should().Be(value.ToString());
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.ReturnParameterWithValidationAndParameterFromService));
            
            something
                .Received(1)
                .DoSomething();
        }
        #endregion
        
        #region Task
        [Fact]
        public async Task TaskWithDelay()
        {
            var action = CreateAction(nameof(LampThing.ReturnTaskWithDelay));
            action.IsValid().Should().BeTrue();

            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();

            _thing.Values.Should().HaveCount(1);
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.ReturnTaskWithDelay));
            
        }
        
        [Fact]
        public async Task TaskAction()
        {
            var action = CreateAction(nameof(LampThing.ReturnTask));
            action.IsValid().Should().BeTrue();

            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();

            _thing.Values.Should().HaveCount(1);
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.ReturnTask));
            
        }
        
        #endregion
        
        #region ValueTask
        [Fact]
        public async Task ValueTaskAction()
        {
            var action = CreateAction(nameof(LampThing.ReturnValueTask));
            action.IsValid().Should().BeTrue();

            action.Status.Should().Be(Status.Pending);
            action.TimeCompleted.Should().BeNull();
            await action.ExecuteAsync(_thing, _provider);
            action.Status.Should().Be(Status.Completed);
            action.TimeCompleted.Should().NotBeNull();

            _thing.Values.Should().HaveCount(1);
            _thing.Values.Last.Value.Should().Be(nameof(LampThing.ReturnValueTask));
            
        }
        #endregion
        
        public interface ISomething
        {
            void DoSomething();
        }
        
        public class LampThing : Thing
        {
            public override string Name => nameof(LampThing);
            internal LinkedList<string> Values { get; } = new LinkedList<string>();

            [ThingAction(Ignore = true)]
            public void Ignore() 
                => Values.AddLast(nameof(Ignore));

            #region Void
            public void ReturnVoid()
                => Values.AddLast(nameof(ReturnVoid));
            
            public void ReturnVoidWithParameter(int value)
            {
                Values.AddLast(value.ToString());
                Values.AddLast(nameof(ReturnVoidWithParameter));
            } 
            
            public void ReturnVoidWithParameterWithValidation([ThingParameter(Minimum =  0, Maximum = 100)]int value)
            {
                Values.AddLast(value.ToString());
                Values.AddLast(nameof(ReturnVoidWithParameterWithValidation));
            }
            
            public void ReturnParameterWithValidationAndParameterFromService([ThingParameter(Minimum =  0, Maximum = 100)]int value, 
                [FromServices]ISomething logger)
            {
                logger.DoSomething();
                Values.AddLast(value.ToString());
                Values.AddLast(nameof(ReturnParameterWithValidationAndParameterFromService));
            }
            
            public void Throwable()
            {
                Values.AddLast(nameof(Throwable));
                throw new Exception();
            } 
            #endregion

            #region Task

            public async Task ReturnTaskWithDelay()
            {
                await Task.Delay(100);
                Values.AddLast(nameof(ReturnTaskWithDelay));
            } 
            
            public Task ReturnTask()
            {
                return Task.Factory.StartNew(() =>
                {
                    Values.AddLast(nameof(ReturnTask));
                });
            }
            #endregion
            
            #region ValueTask
            public ValueTask ReturnValueTask()
            {
                return new ValueTask(Task.Factory.StartNew(() =>
                {
                    Values.AddLast(nameof(ReturnValueTask));
                }));
            }

            #endregion
        }
    }
}
