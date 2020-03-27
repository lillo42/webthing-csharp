using System;
using System.Linq;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Properties;
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

        [Fact]
        public void TryAddWhenSetThingTypeIsNotCalled()
            => Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<PropertyInfo>(),
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
        public void BuildReadOnlyProperty()
        {
            _builder
                .SetThing(_thing)
                .SetThingOption(new ThingOption());

            var propertyName = _fixture.Create<string>();
            
            Visit(new Information(null, null, null, null, null,
                null, null, null, null, true,
                propertyName, _fixture.Create<bool>()));

            var properties = _builder.Build();
            properties.Should().NotBeNull();
            properties.Should().NotBeEmpty();
            properties.Should().HaveCount(1);
            properties.Should().ContainKey(propertyName);
            properties[propertyName].Should().BeAssignableTo<PropertyReadOnly>();
            _thing.Value = _fixture.Create<string>();
            properties[propertyName].GetValue().Should().Be(_thing.Value);
        }
        
        [Fact]
        public void BuildNonReadOnlyProperty()
        {
            _builder
                .SetThing(_thing)
                .SetThingOption(new ThingOption());

            var propertyName = _fixture.Create<string>();
            
            var information = new Information(null, null, null, null, null,
                null, null, null, null, false,
                propertyName, _fixture.Create<bool>());
            
            Visit(information);

            var properties = _builder.Build();
            properties.Should().NotBeNull();
            properties.Should().NotBeEmpty();
            properties.Should().HaveCount(1);
            properties.Should().ContainKey(propertyName);
            properties[propertyName].Should().NotBeAssignableTo<PropertyReadOnly>();

            _factory.Information.Should().Be(information);
            var value = _fixture.Create<string>();
            _factory.Setter(_thing, value);
            _factory.Getter(_thing).Should().Be(value);
            _thing.Value.Should().Be(value);
        }
        
        private void Visit(Information information)
        {
            var properties = _thing.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name));
            
            foreach (var property in properties)
            {
                _builder.Add(property, information);
            }
            
            static bool IsThingProperty(string name)
                => name == nameof(Thing.Context)
                   || name == nameof(Thing.Name)
                   || name == nameof(Thing.Description)
                   || name == nameof(Thing.Title)
                   || name == nameof(Thing.Type)
                   || name == nameof(Thing.ThingContext);
        }
        
        private class FakePropertyFactory : IPropertyFactory
        {
            public Information Information { get; private set; }
            public Action<object, object> Setter { get; private set; }
            public Func<object, object> Getter { get; private set; }
            public IProperty Create(Type propertyType, Information information, Thing thing, Action<object, object> setter, 
                Func<object, object> getter)
            {
                Information = information;
                Setter = setter;
                Getter = getter;

                return Substitute.For<IProperty>();
            }
        }
        
        public class PropertyThing : Thing
        {
            public override string Name => "property-thing";
            
            public string Value { get; set; }
        }
    }
}
