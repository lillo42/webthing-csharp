using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class ActionBuilderTest
    {
        private readonly Fixture _fixture;
        private readonly IActionParameterFactory _factory;
        private readonly ActionBuilder _builder;
        private readonly ActionThing _thing;

        public ActionBuilderTest()
        {
            _fixture = new Fixture();
            _thing = new ActionThing();
            _factory = Substitute.For<IActionParameterFactory>();
            _builder = new ActionBuilder(_factory);
        }


        [Fact]
        public void TryAddWhenSetThingTypeIsNotCalled()
            => Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<MethodInfo>(),
               Substitute.For<ThingActionAttribute>()));
        
        [Fact]
        public void TryBuildWhenIsNotSetSetThing() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Build());

        [Fact]
        public void TryBuildWhenIsNotSetThingType()
        {
            _builder.SetThing(_thing);
            Assert.Throws<InvalidOperationException>(() => _builder.Build());
        }

        [Fact]
        public void BuildWithActionWithNoParameter()
        {
            _builder.SetThing(_thing)
                .SetThingType(_thing.GetType())
                .SetThingOption(new ThingOption());

            var method = typeof(ActionThing).GetMethod(nameof(ActionThing.NoParameter));

            _builder.Add(method!, null);

            var actions = _builder.Build();
            actions.Should().NotBeNull();
            actions.Should().NotBeEmpty();
            actions.Should().HaveCount(1);
            actions.Should().ContainKey(nameof(ActionThing.NoParameter));

            _factory.DidNotReceive().Create(Arg.Any<Type>(), Arg.Any<JsonSchema>());
        }
        
        [Fact]
        public void BuildWithActionWithParameter()
        {
            _builder.SetThing(_thing)
                .SetThingType(_thing.GetType())
                .SetThingOption(new ThingOption());

            var method = typeof(ActionThing).GetMethod(nameof(ActionThing.WithParameter));

            _builder.Add(method!, null);

            var parameters = new List<(ParameterInfo, JsonSchema)>();
            
            foreach (var parameter in method.GetParameters())
            {
                var info = new JsonSchema(Substitute.For<IJsonSchema>(), null, parameter.ParameterType.ToJsonType(),
                    parameter.Name!, _fixture.Create<bool>());;
                
                parameters.Add((parameter, info));
                _factory
                    .Create(parameter.ParameterType, info)
                    .Returns(Substitute.For<IActionParameter>());
                
                _builder.Add(parameter, info);
            }

            var actions = _builder.Build();
            actions.Should().NotBeNull();
            actions.Should().NotBeEmpty();
            actions.Should().HaveCount(1);
            actions.Should().ContainKey(nameof(ActionThing.WithParameter));

            foreach (var (parameter, info) in parameters)
            {
                _factory.Received(1)
                    .Create(parameter.ParameterType, info );
            }
        }
        
        [Fact]
        public void BuildWithMultiActions()
        {
            _builder.SetThing(_thing)
                .SetThingType(_thing.GetType())
                .SetThingOption(new ThingOption());

            var withParameter = typeof(ActionThing).GetMethod(nameof(ActionThing.WithParameter));

            _builder.Add(withParameter!, null);

            var parameters = new List<(ParameterInfo, JsonSchema)>();
            
            foreach (var parameter in withParameter.GetParameters())
            {
                var info = new JsonSchema(Substitute.For<IJsonSchema>(), null, parameter.ParameterType.ToJsonType(),
                    parameter.Name!, _fixture.Create<bool>());
                
                parameters.Add((parameter, info));
                _factory
                    .Create(parameter.ParameterType, info)
                    .Returns(Substitute.For<IActionParameter>());
                
                _builder.Add(parameter, info);
            }

            var noParameter = typeof(ActionThing).GetMethod(nameof(ActionThing.NoParameter));
            _builder.Add(noParameter!, null);

            
            var actions = _builder.Build();
            actions.Should().NotBeNull();
            actions.Should().NotBeEmpty();
            actions.Should().HaveCount(2);
            actions.Should().ContainKey(nameof(ActionThing.NoParameter));
            actions.Should().ContainKey(nameof(ActionThing.WithParameter));

            foreach (var (parameter, info) in parameters)
            {
                _factory.Received(1)
                    .Create(parameter.ParameterType, info );
            }
        }
        
        public class ActionThing : Thing
        {
            public override string Name => "action-thing";
            

            public void NoParameter()
            {
                
            }
            
            public void WithParameter(string value, int id)
            {
            }
        }
    }
}
