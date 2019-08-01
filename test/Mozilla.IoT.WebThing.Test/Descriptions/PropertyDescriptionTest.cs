using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Exceptions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Descriptions
{
    public class PropertyDescriptionTest
    {
        private readonly Fixture _fixture;
        private readonly Property _property;
        private readonly PropertyDescription _propertyDescription;

        public PropertyDescriptionTest()
        {
            _fixture = new Fixture();
            _property = new Property();
            _propertyDescription = new PropertyDescription();
        }

        [Fact]
        public void CreateDescription_Should_ReturnMinimum_When_DoNotHaveMetadata()
        {
            var name = _fixture.Create<string>();
            _property.Name = name;

            var expected = new Dictionary<string, object>
            {
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["rel"] = "property", 
                        ["href"] = $"/properties/{name}"
                    }
                }
            };

            var result = _propertyDescription.CreateDescription(_property);

            expected.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CreateDescription_Should_MetadataAndLink_When_HaveMetadataDoesNotHaveLink()
        {
            _property.Name = _fixture.Create<string>();

            var property = _fixture.Create<string>();
            var propertyValue = _fixture.Create<string>();

            _property.Metadata = new Dictionary<string, object>
            {
                [property] = propertyValue,
            };

            var expected = new Dictionary<string, object>
            {
                [property] = propertyValue,
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["rel"] = "property", 
                        ["href"] = $"/properties/{_property.Name}"
                    }
                }
            };

            var result = _propertyDescription.CreateDescription(_property);

            expected.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CreateDescription_Should_MetadataAndLink_When_HaveMetadataAndLink()
        {
            _property.Name = _fixture.Create<string>();

            var rel = _fixture.Create<string>();
            var hrefLinks = _fixture.Create<string>();
            
            var property = _fixture.Create<string>();
            var propertyValue = _fixture.Create<string>();
            
            var metadata = new Dictionary<string, object>
            {
                [property] = propertyValue,
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["rel"] = rel, 
                        ["href"] = hrefLinks
                    }
                }
            };

            _property.Metadata = metadata;

            var expected = new Dictionary<string, object>
            {
                [property] = propertyValue,
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["rel"] = rel, 
                        ["href"] = hrefLinks
                    },
                    new Dictionary<string, object>
                    {
                        ["rel"] = "property", 
                        ["href"] = $"/properties/{_property.Name}"
                    }
                }
            };

            var result = _propertyDescription.CreateDescription(_property);

            expected.Should().BeEquivalentTo(result);
        }

        [Fact] 
        public void CreateDescription_Should_Throw_When_LinksIsNotACollections()
        {
            _property.Name = _fixture.Create<string>();

            var rel = _fixture.Create<string>();
            var hrefLinks = _fixture.Create<string>();
            
            _property.Metadata = new Dictionary<string, object>
            {
                ["links"] = new Dictionary<string, object>
                {
                    ["rel"] = rel, 
                    ["href"] = hrefLinks
                }
            };
            
            Assert.Throws<DescriptionException>(() => _propertyDescription.CreateDescription(_property));
        }
    }
}
