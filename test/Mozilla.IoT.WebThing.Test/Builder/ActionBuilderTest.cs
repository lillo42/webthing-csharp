using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using NSubstitute;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class ActionBuilderTest
    {
        private readonly IJsonSchemaValidationFactory _schemaValidation;
        private readonly IJsonConvertibleFactory _jsonConvertibleFactory;
        private readonly IConvertibleFactory _convertibleFactory;
        private readonly ActionBuilder _builder;
        private readonly Fixture _fixture;

        public ActionBuilderTest()
        {
            _fixture = new Fixture();

            _schemaValidation = Substitute.For<IJsonSchemaValidationFactory>();
            _jsonConvertibleFactory = Substitute.For<IJsonConvertibleFactory>();
            _convertibleFactory = Substitute.For<IConvertibleFactory>();
            
            _builder = new ActionBuilder(_schemaValidation, _jsonConvertibleFactory, _convertibleFactory);
        }

        #region Set

        [Fact]
        public void SetThing()
        {
            _builder.SetThing(Substitute.For<Thing>());
        }

        [Fact]
        public void SetThingType()
        {
            _builder.SetThingType(typeof(ActionThing));
        }

        [Fact]
        public void SetThingOption()
        {
            _builder.SetThingOption(new ThingOption());
        }

        #endregion
        
        #region Add Method

        [Fact]
        public void AddMethod_Should_Throw_When_SetThingIsCalled()
        {
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<MethodInfo>(), null));
        }
        
        [Fact]
        public void AddMethod_Should_Throw_When_SetThingOptionIsCalled()
        {
            _builder.SetThingType(typeof(ActionThing));
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<MethodInfo>(), null));
        }

        [Fact]
        public void AddMethod()
        {
           var result = _builder
                .SetThingType(typeof(ActionThing))
                .SetThing(new ActionThing())
                .SetThingOption(new ThingOption())
                .Add(typeof(ActionThing).GetMethod(nameof(ActionThing.Foo)), null)
                .Build();

           result.Should().NotBeNull();
           result.Should().NotBeEmpty();
           result.Should().ContainKey(nameof(ActionThing.Foo));
        }
        
        [Fact]
        public void AddMethod_Should_Works_When_ItCalledTwice()
        {
            var result = _builder
                .SetThingType(typeof(ActionThing))
                .SetThing(new ActionThing())
                .SetThingOption(new ThingOption())
                .Add(typeof(ActionThing).GetMethod(nameof(ActionThing.Foo))!, null)
                .Add(typeof(ActionThing).GetMethod(nameof(ActionThing.Foo2))!, null)
                .Build();

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().ContainKey(nameof(ActionThing.Foo));
        }

        #endregion

        #region Add Parameter

        [Fact]
        public void AddParameter_Should_Throw_When_AddMethodWasNotCalled()
        {
          Assert.Throws<InvalidOperationException>(() =>  _builder.Add(Substitute.For<ParameterInfo>(), default));
        }
        
        [Fact]
        public void AddParameter()
        {
            var method = typeof(ActionThing).GetMethod(nameof(ActionThing.Foo3));
            
           _builder
                .SetThingType(typeof(ActionThing))
                .SetThing(new ActionThing())
                .SetThingOption(new ThingOption())
                .Add(method, null);

           var parameters = method.GetParameters();
           var parameter = parameters[0];

           var jsonConvertible = Substitute.For<IJsonConvertible>();
           _jsonConvertibleFactory.Create(parameter.ParameterType.ToTypeCode(), parameter.ParameterType)
               .Returns(jsonConvertible);

           var schemaValidation = Substitute.For<IJsonSchemaValidation>();
           _schemaValidation.Create(parameter.ParameterType.ToTypeCode(), Arg.Any<JsonSchema>(), parameter.ParameterType)
               .Returns(schemaValidation);
           
           var convertible = Substitute.For<IConvertible>();
           _convertibleFactory.Create(parameter.ParameterType.ToTypeCode(), parameter.ParameterType)
               .Returns(convertible);

           
           var result = _builder
               .Add(parameter, parameter.GetCustomAttribute<ThingParameterAttribute>()!.ToJsonSchema(parameter))
               .Add(parameters[1], parameters[1].GetCustomAttribute<ThingParameterAttribute>()!.ToJsonSchema(parameters[1]))
               .Add(parameters[2], parameters[2].GetCustomAttribute<ThingParameterAttribute>()!.ToJsonSchema(parameters[2]))
               .Build();

           result.Should().NotBeNull();
           result.Should().NotBeEmpty();
           result.Should().ContainKey(nameof(ActionThing.Foo3));

           _jsonConvertibleFactory
               .Received(1)
               .Create(parameter.ParameterType.ToTypeCode(), parameter.ParameterType);

           _schemaValidation
               .Received(1)
               .Create(parameter.ParameterType.ToTypeCode(), Arg.Any<JsonSchema>(), parameter.ParameterType);

           _convertibleFactory
               .Received(1)
               .Create(parameter.ParameterType.ToTypeCode(), parameter.ParameterType);
        }
        #endregion

        #region Build
        
        [Fact]
        public void Build_Should_Throw_When_SetThingOptionWasNotCalled()
        {
            Assert.Throws<InvalidOperationException>(() => _builder.Build());
        }
        
        [Fact]
        public void Build_Should_Throw_When_SetThingTypeWasNotCalled()
        {
            _builder.SetThingOption(new ThingOption());
            Assert.Throws<InvalidOperationException>(() => _builder.Build());
        }

        #endregion
        
        
        public class ActionThing : Thing
        {
            public override string Name => "action-thing";

            public void Foo()
            {
                
            }
            
            public Task Foo2()
            {
                return Task.CompletedTask;
            }

            public ValueTask Foo3(string value, [FromServices]object service, CancellationToken cancellationToken)
            {
                return new ValueTask();
            }
        }
    }
}
