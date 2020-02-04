using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Actions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator
{
    public class ActionInterceptFactoryTest
    {
        private readonly Fixture _fixture;
        private readonly ILogger<ActionInfo> _logger;

        public ActionInterceptFactoryTest()
        {
            _fixture = new Fixture();
            _logger = Substitute.For<ILogger<ActionInfo>>();
        }

        [Fact]
        public void Ignore()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            actionFactory.Actions.Should().NotContainKey(nameof(LampThing.Ignore));
        }

        #region Void
        [Fact]
        public async Task VoidWithoutParameter()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.WithoutParameter));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.WithoutParameter)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(1);
            thing.Values.Last.Value.Should().Be(nameof(LampThing.WithoutParameter));
        }
        
        [Fact]
        public async Task VoidWithParameter()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.WithParameter));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.WithParameter)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var idProperty = inputProperty.PropertyType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            idProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            var id = _fixture.Create<string>();
            idProperty.SetValue(input, id);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(2);
            thing.Values.First.Value.Should().Be(id);
            thing.Values.Last.Value.Should().Be(nameof(LampThing.WithParameter));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task VoidWithParameterValid(int value)
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.ParameterWithValidation));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.ParameterWithValidation)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var valueProperty = inputProperty.PropertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            valueProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            valueProperty.SetValue(input, value);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(2);
            thing.Values.First.Value.Should().Be(value.ToString());
            thing.Values.Last.Value.Should().Be(nameof(LampThing.ParameterWithValidation));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void VoidWithParameterInvalid(int value)
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.ParameterWithValidation));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.ParameterWithValidation)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var valueProperty = inputProperty.PropertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            valueProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            valueProperty.SetValue(input, value);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeFalse();

            thing.Values.Should().BeEmpty();
        }
        
        [Fact]
        public async Task Throwable()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.WithParameter));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.Throwable)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();
            
            var input = Activator.CreateInstance(inputProperty.PropertyType);

            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);

            thing.Values.Should().BeEmpty();
        }
        #endregion
        
        #region Task
        [Fact]
        public async Task TaskWithoutParameter()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.TaskWithoutParameter));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.TaskWithoutParameter)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(1);
            thing.Values.Last.Value.Should().Be(nameof(LampThing.TaskWithoutParameter));
        }
        
        [Fact]
        public async Task TaskWithParameter()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.TaskWithParameter));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.TaskWithParameter)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var idProperty = inputProperty.PropertyType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            idProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            var id = _fixture.Create<string>();
            idProperty.SetValue(input, id);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(2);
            thing.Values.First.Value.Should().Be(id);
            thing.Values.Last.Value.Should().Be(nameof(LampThing.TaskWithParameter));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task TaskWithParameterValid(int value)
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.TaskParameterWithValidation));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.TaskParameterWithValidation)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var valueProperty = inputProperty.PropertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            valueProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            valueProperty.SetValue(input, value);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(2);
            thing.Values.First.Value.Should().Be(value.ToString());
            thing.Values.Last.Value.Should().Be(nameof(LampThing.TaskParameterWithValidation));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void TaskWithParameterInvalid(int value)
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.TaskParameterWithValidation));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.TaskParameterWithValidation)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var valueProperty = inputProperty.PropertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            valueProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            valueProperty.SetValue(input, value);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeFalse();

            thing.Values.Should().BeEmpty();
        }
        
        [Fact]
        public async Task TaskThrowable()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.TaskThrowable));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.TaskThrowable)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();
            
            var input = Activator.CreateInstance(inputProperty.PropertyType);

            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);

            thing.Values.Should().BeEmpty();
        }
        #endregion
        
        #region ValueTask
        [Fact]
        public async Task ValueTaskWithoutParameter()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.ValueTaskWithoutParameter));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.ValueTaskWithoutParameter)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(1);
            thing.Values.Last.Value.Should().Be(nameof(LampThing.ValueTaskWithoutParameter));
        }
        
        [Fact]
        public async Task ValueTaskWithParameter()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.ValueTaskWithParameter));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.ValueTaskWithParameter)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var idProperty = inputProperty.PropertyType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            idProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            var id = _fixture.Create<string>();
            idProperty.SetValue(input, id);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(2);
            thing.Values.First.Value.Should().Be(id);
            thing.Values.Last.Value.Should().Be(nameof(LampThing.ValueTaskWithParameter));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task ValueTaskWithParameterValid(int value)
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.ValueTaskParameterWithValidation));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.ValueTaskParameterWithValidation)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var valueProperty = inputProperty.PropertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            valueProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            valueProperty.SetValue(input, value);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);
            
            thing.Values.Should().HaveCount(2);
            thing.Values.First.Value.Should().Be(value.ToString());
            thing.Values.Last.Value.Should().Be(nameof(LampThing.ValueTaskParameterWithValidation));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void ValueTaskWithParameterInvalid(int value)
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.ValueTaskParameterWithValidation));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.ValueTaskParameterWithValidation)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();

            var valueProperty = inputProperty.PropertyType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            valueProperty.Should().NotBeNull();

            var input = Activator.CreateInstance(inputProperty.PropertyType);
            valueProperty.SetValue(input, value);
            
            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeFalse();

            thing.Values.Should().BeEmpty();
        }
        
        [Fact]
        public async Task ValueTaskThrowable()
        {
            var thing = new LampThing();
            var actionFactory = new ActionInterceptFactory();
            
            CodeGeneratorFactory.Generate(thing, new []{ actionFactory });
            
            actionFactory.Actions.Should().ContainKey(nameof(LampThing.ValueTaskThrowable));
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IProperties>(),
                new Dictionary<string, EventCollection>(), 
                actionFactory.Actions);
            
            var actionType = thing.ThingContext.Actions[nameof(LampThing.ValueTaskThrowable)].ActionType;
            var action = (ActionInfo)Activator.CreateInstance(actionType);

            var inputProperty = actionType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
            inputProperty.Should().NotBeNull();
            
            var input = Activator.CreateInstance(inputProperty.PropertyType);

            inputProperty.SetValue(action, input);
            
            action.Status.Should().Be(Status.Pending);
            action.IsValid().Should().BeTrue();
            await action.ExecuteAsync(thing, _logger);
            action.Status.Should().Be(Status.Completed);

            thing.Values.Should().BeEmpty();
        }
        #endregion
        
        public class LampThing : Thing
        {
            public override string Name => nameof(LampThing);
            internal LinkedList<string> Values { get; } = new LinkedList<string>();

            [ThingAction(Ignore = true)]
            public void Ignore() 
                => Values.AddLast(nameof(Ignore));

            #region Void
            public void WithoutParameter()
                => Values.AddLast(nameof(WithoutParameter));
            
            public void WithParameter(string id)
            {
                Values.AddLast(id);
                Values.AddLast(nameof(WithParameter));
            } 
            
            public void ParameterWithValidation([ThingParameter(Minimum =  0, Maximum = 100)]int value)
            {
                Values.AddLast(value.ToString());
                Values.AddLast(nameof(ParameterWithValidation));
            } 
            
            public void Throwable()
            {
                throw new Exception();
            } 
            #endregion

            #region Task

            public async Task TaskWithoutParameter()
            {
                await Task.Delay(100);
                Values.AddLast(nameof(TaskWithoutParameter));
            } 
            
            public Task TaskWithParameter(string id)
            {
                return Task.Factory.StartNew(() =>
                {
                    Values.AddLast(id);
                    Values.AddLast(nameof(TaskWithParameter));
                });
            } 
            
            public async Task TaskParameterWithValidation([ThingParameter(Minimum =  0, Maximum = 100)]int value)
            {
                await Task.Delay(100)
                    .ConfigureAwait(false);
                
                Values.AddLast(value.ToString());
                Values.AddLast(nameof(TaskParameterWithValidation));
            }

            public async Task TaskThrowable()
            {
                await Task.Delay(100)
                    .ConfigureAwait(false);
                throw new Exception();
            }

            #endregion
            
            #region ValueTask

            public ValueTask ValueTaskWithoutParameter()
            {
                Values.AddLast(nameof(ValueTaskWithoutParameter));
                return new ValueTask();
            } 
            
            public ValueTask ValueTaskWithParameter(string id)
            {
                return new ValueTask(Task.Factory.StartNew(() =>
                {
                    Values.AddLast(id);
                    Values.AddLast(nameof(ValueTaskWithParameter));
                }));
            } 
            
            public async ValueTask ValueTaskParameterWithValidation([ThingParameter(Minimum =  0, Maximum = 100)]int value)
            {
                await Task.Delay(100)
                    .ConfigureAwait(false);
                
                Values.AddLast(value.ToString());
                Values.AddLast(nameof(ValueTaskParameterWithValidation));
            }

            public ValueTask ValueTaskThrowable()
            {
                throw new Exception();
            }

            #endregion
        }
    }
}
