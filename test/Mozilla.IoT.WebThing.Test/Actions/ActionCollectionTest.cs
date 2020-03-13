using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Actions
{
    public class ActionCollectionTest
    {
        private readonly Fixture _fixture;
        private readonly IActionInfoFactory _factory;
        private readonly JsonElement _input;
        private readonly string _parameterName;
        private readonly int _parameterValue;
        private readonly IActionParameter _parameter;
        private readonly Dictionary<string, IActionParameter> _parameters;
        private readonly ActionInfoConvert _convert;
        private readonly ActionCollection _collection;

        public ActionCollectionTest()
        {
            _fixture = new Fixture();
            _parameterName = _fixture.Create<string>();
            _parameterValue = _fixture.Create<int>();
            
            _factory = Substitute.For<IActionInfoFactory>();
            _parameter = Substitute.For<IActionParameter>();
            _parameters = new Dictionary<string, IActionParameter>
            {
                [_parameterName] = _parameter
            };

            _input = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {{ 
                    ""{_parameterName}"": {_parameterValue}
                }} 
            }}");
            
            _convert = new ActionInfoConvert(_parameters); 
            _collection = new ActionCollection(_convert, _factory);
        }

        #region TryAdd

        [Fact]
        public void TryAddWithSuccess()
        {
            _parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = _parameterValue;
                    return true;
                });

            var actionInfo = Substitute.For<ActionInfo>();
            
            _factory.CreateActionInfo(Arg.Any<Dictionary<string, object>>())
                .Returns(actionInfo);
            
            _collection.TryAdd(_input, out var action).Should().BeTrue();
            action.Should().NotBeNull();

            _parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>());

            _factory
                .Received(1)
                .CreateActionInfo(Arg.Any<Dictionary<string, object>>());
        }
        
        [Fact]
        public void TryAddWhenInputNotExist()
        {
            var input = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{_parameterName}"": {_parameterValue} }}");
            
            _collection.TryAdd(input, out var action).Should().BeFalse();
            
            action.Should().BeNull();

            _parameter
                .DidNotReceive()
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>());

            _factory
                .DidNotReceive()
                .CreateActionInfo(Arg.Any<Dictionary<string, object>>());
        }
        
        [Fact]
        public void TryAddWhenCouldNotConvert()
        {
            _parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = null;
                    return false;
                });

            _collection.TryAdd(_input, out var action).Should().BeFalse();
            action.Should().BeNull();

            _parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>());

            _factory
                .DidNotReceive()
                .CreateActionInfo(Arg.Any<Dictionary<string, object>>());
        }

        #endregion

        #region TryGetValue

        [Fact]
        public void TryGetWithSuccess()
        {
            _parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = _parameterValue;
                    return true;
                });

            var actionInfo = Substitute.For<ActionInfo>();
            
            _factory.CreateActionInfo(Arg.Any<Dictionary<string, object>>())
                .Returns(actionInfo);
            
            _collection.TryAdd(_input, out var action).Should().BeTrue();
            action.Should().NotBeNull();

            _collection.TryGetValue(action.GetId(), out var getAction).Should().BeTrue();

            getAction.Should().NotBeNull();
            getAction.Should().Be(action);

            _parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>());

            _factory
                .Received(1)
                .CreateActionInfo(Arg.Any<Dictionary<string, object>>());
        }
        
        [Fact]
        public void TryGetWhenActionDoesNotExist()
        {
            _collection.TryGetValue(_fixture.Create<Guid>(), out var action).Should().BeFalse();
            action.Should().BeNull();
        }

        #endregion
        
        #region TryRemove

        [Fact]
        public void TryRemoveWithSuccess()
        {
            _parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = _parameterValue;
                    return true;
                });

            var actionInfo = Substitute.For<ActionInfo>();
            
            _factory.CreateActionInfo(Arg.Any<Dictionary<string, object>>())
                .Returns(actionInfo);
            
            _collection.TryAdd(_input, out var action).Should().BeTrue();
            action.Should().NotBeNull();

            _collection.TryRemove(action.GetId(), out var getAction).Should().BeTrue();

            getAction.Should().NotBeNull();
            getAction.Should().Be(action);

            _parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>());

            _factory
                .Received(1)
                .CreateActionInfo(Arg.Any<Dictionary<string, object>>());
        }
        
        [Fact]
        public void TryRemoveWhenActionDoesNotExist()
        {
            _collection.TryRemove(_fixture.Create<Guid>(), out var action).Should().BeFalse();
            action.Should().BeNull();
        }

        #endregion

        #region OnStatusChange

        [Fact]
        public void OnStatusChange()
        {
            var counter = 0;
            _collection.Change += OnActionStatusChange;
            _parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>())
                .Returns(x =>
                {
                    x[1] = _parameterValue;
                    return true;
                });

            var actionInfo = new VoidActionInfo();
            
            _factory.CreateActionInfo(Arg.Any<Dictionary<string, object>>())
                .Returns(actionInfo);
            
            _collection.TryAdd(_input, out var action).Should().BeTrue();
            action.Should().NotBeNull();

            var provider = Substitute.For<IServiceProvider>();
            provider.GetService(typeof(ILogger<ActionInfo>))
                .Returns(Substitute.For<ILogger<ActionInfo>>());

            actionInfo.ExecuteAsync(Substitute.For<Thing>(), provider);
            
            counter.Should().Be(2);

            counter = 0;

            _collection.TryRemove(actionInfo.GetId(), out _);
            
            actionInfo.ExecuteAsync(Substitute.For<Thing>(), provider);
            counter.Should().Be(0);
            
            _parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object>());

            _factory
                .Received(1)
                .CreateActionInfo(Arg.Any<Dictionary<string, object>>());

            

            void OnActionStatusChange(object sender, ActionInfo info)
            {
                counter++;
            }
        }

        #endregion
        
        public class VoidActionInfo : ActionInfo
        {
            public List<string> Logs { get; } = new List<string>();
            protected override ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider)
            {
                Logs.Add(nameof(VoidActionInfo));
                return new ValueTask();
            }

            public override string GetActionName()
                => "void-action";
        }
    }
}
