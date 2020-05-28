using System;
using System.Linq;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class PropertyBuilderTest
    {
        private readonly PropertyBuilder _builder;
        private readonly PropertyThing _thing;
        private readonly FakePropertyFactory _factory;
        private readonly Fixture _fixture;

        public PropertyBuilderTest()
        {
            _fixture = new Fixture();
            _thing = new PropertyThing();

            _factory = new FakePropertyFactory();
            _builder = new PropertyBuilder(_factory);
        }

        #region Add

        [Fact]
        public void Add_Should_Throw_When_SetThingTypeIsNotCalled()
            => Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<PropertyInfo>(),
                new JsonSchema(Substitute.For<IJsonSchema>(), null, JsonType.String,
                    _fixture.Create<string>(), _fixture.Create<bool>())));

        [Fact]
        public void Add_Should_Throw_When_SetThingOptionIsNotCalled()
        {
            _builder.SetThing(new PropertyThing());
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<PropertyInfo>(),
                new JsonSchema(Substitute.For<IJsonSchema>(), null, JsonType.String,
                    _fixture.Create<string>(), _fixture.Create<bool>())));
        }

        #endregion

        #region Build

        [Fact]
        public void Build_Should_Throw_When_SetThingWasNotCalled() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Build());
        
        [Fact]
        public void Build_Should_Throw_When_SetThingTypeWasNotCalled()
        {
            _builder.SetThing(_thing);
            Assert.Throws<InvalidOperationException>(() => _builder.Build());
        }
        
        [Fact]
        public void BuildForReadOnlyProperty()
        {
            _builder
                .SetThing(_thing)
                .SetThingOption(new ThingOption());

            var propertyName = _fixture.Create<string>();
            
            var schema = Substitute.For<IJsonSchema>();
            schema.IsReadOnly.Returns(true);
            Visit(new JsonSchema(schema, null, JsonType.String,
                propertyName, _fixture.Create<bool>()));

            var properties = _builder.Build();
            properties.Should().NotBeNull();
            properties.Should().NotBeEmpty();
            properties.Should().HaveCount(1);
            properties.Should().ContainKey(propertyName);
        }

        [Fact]
        public void BuildNonReadOnlyProperty()
        {
            _builder
                .SetThing(_thing)
                .SetThingOption(new ThingOption());

            var propertyName = _fixture.Create<string>();
            
            var information = new JsonSchema(Substitute.For<IJsonSchema>(), null, JsonType.String,
                propertyName, _fixture.Create<bool>());
            
            Visit(information);

            var properties = _builder.Build();
            properties.Should().NotBeNull();
            properties.Should().NotBeEmpty();
            properties.Should().HaveCount(1);
            properties.Should().ContainKey(propertyName);

            _factory.JsonSchema.Should().Be(information);
            var value = _fixture.Create<string>();
            _factory.Setter(_thing, value);
            _factory.Getter(_thing).Should().Be(value);
            _thing.Value.Should().Be(value);
        }

        
        #endregion
        
        private void Visit(JsonSchema jsonSchema)
        {
            var properties = _thing.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name));
            
            foreach (var property in properties)
            {
                _builder.Add(property, jsonSchema);
            }
            
            static bool IsThingProperty(string name)
                => name == nameof(Thing.Context)
                   || name == nameof(Thing.Id)
                   || name == nameof(Thing.Name)
                   || name == nameof(Thing.Description)
                   || name == nameof(Thing.Title)
                   || name == nameof(Thing.Type)
                   || name == nameof(Thing.ThingContext)
                   || name == nameof(Thing.Security)
                   || name == nameof(Thing.SecurityDefinitions);
        }
        
        private class FakePropertyFactory : IPropertyFactory
        {
            public JsonSchema JsonSchema { get; private set; }
            public Action<object, object> Setter { get; private set; }
            public Func<object, object> Getter { get; private set; }
            public IThingProperty Create(Type propertyType, JsonSchema jsonSchema, Thing thing, Action<object, object> setter, 
                Func<object, object> getter, string originPropertyName)
            {
                JsonSchema = jsonSchema;
                Setter = setter;
                Getter = getter;

                return Substitute.For<IThingProperty>();
            }
        }
        
        public class PropertyThing : Thing
        {
            public override string Name => "property-thing";
            
            public string Value { get; set; }
        }
    }
}
