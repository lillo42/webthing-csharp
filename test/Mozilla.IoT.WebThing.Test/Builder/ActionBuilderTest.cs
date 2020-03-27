using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
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
                new Information(null, null, null, null, null,
                    null, null, null, null, _fixture.Create<bool>(), 
                    _fixture.Create<string>(), _fixture.Create<bool>())));
        
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
            
            var information = new Information(null, null, null, null, null,
                null, null, null, null, false,
                nameof(ActionThing.NoParameter), _fixture.Create<bool>());
            
            _builder.Add(method!, information);

            var actions = _builder.Build();
            actions.Should().NotBeNull();
            actions.Should().NotBeEmpty();
            actions.Should().HaveCount(1);
            actions.Should().ContainKey(nameof(ActionThing.NoParameter));

            _factory.DidNotReceive().Create(Arg.Any<Type>(), Arg.Any<Information>());
        }
        
        [Fact]
        public void BuildWithActionWithParameter()
        {
            _builder.SetThing(_thing)
                .SetThingType(_thing.GetType())
                .SetThingOption(new ThingOption());

            var method = typeof(ActionThing).GetMethod(nameof(ActionThing.WithParameter));
            
            var information = new Information(null, null, null, null, null,
                null, null, null, null, false,
                nameof(ActionThing.WithParameter), _fixture.Create<bool>());
            
            _builder.Add(method!, information);

            var parameters = new List<(ParameterInfo, Information)>();
            
            foreach (var parameter in method.GetParameters())
            {
                var info = new Information(null, null, null, null, null,
                    null, null, null, null, false,
                    parameter.Name!, _fixture.Create<bool>());
                
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
            
            var informationWithNoParameter = new Information(null, null, null, null, null,
                null, null, null, null, false,
                nameof(ActionThing.WithParameter), _fixture.Create<bool>());
            
            _builder.Add(withParameter!, informationWithNoParameter);

            var parameters = new List<(ParameterInfo, Information)>();
            
            foreach (var parameter in withParameter.GetParameters())
            {
                var info = new Information(null, null, null, null, null,
                    null, null, null, null, false,
                    parameter.Name!, _fixture.Create<bool>());
                
                parameters.Add((parameter, info));
                _factory
                    .Create(parameter.ParameterType, info)
                    .Returns(Substitute.For<IActionParameter>());
                
                _builder.Add(parameter, info);
            }

            var noParameter = typeof(ActionThing).GetMethod(nameof(ActionThing.NoParameter));
            
            var informationNoParameter = new Information(null, null, null, null, null,
                null, null, null, null, false,
                nameof(ActionThing.NoParameter), _fixture.Create<bool>());
            
            _builder.Add(noParameter!, informationNoParameter);

            
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
