using FluentAssertions;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator;
using Mozilla.IoT.WebThing.Factories.Generator.Properties;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator.Properties
{
    public class PropertiesInterceptFactoryTest
    {
        private readonly PropertiesInterceptFactory _factory;

        public PropertiesInterceptFactoryTest()
        {
            _factory = new PropertiesInterceptFactory(new ThingOption());
        }

        
        [Fact]
        public void CreatePropertyIntercept()
        {
            var result = _factory.CreatePropertyIntercept();
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PropertiesIntercept>();
        }
        
        [Fact]
        public void CreateThingIntercept()
        {
            var result = _factory.CreateThingIntercept();
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<EmptyIntercept>();
        }
        
        [Fact]
        public void CreatActionIntercept()
        {
            var result = _factory.CreatActionIntercept();
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<EmptyIntercept>();
        }
        
        [Fact]
        public void CreatEventIntercept()
        {
            var result = _factory.CreatEventIntercept();
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<EmptyIntercept>();
        }
    }
}
