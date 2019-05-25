using System.Threading.Tasks;
using AutoFixture;
using Mozzila.IoT.WebThing;
using Mozzila.IoT.WebThing.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

using static Xunit.Assert;

namespace WebThing.Test
{
    public class PropertyTest
    {
        private readonly Fixture _fixture;
        private readonly Thing _thing;

        public PropertyTest()
        {
            _fixture = new Fixture();
            _thing = _fixture.Create<Thing>();
        }

        [Fact]
        public async Task OnPropertyChanged()
        {
            var property = new Property<int>(_thing, _fixture.Create<string>(), _fixture.Create<int>());

            int other = _fixture.Create<int>();
            int currentValue = property.Value;
            bool change = false;
            property.ValuedChanged += (sender, @event) =>
            {
                Equal(property, sender);
                False(@event.Value == currentValue);

                change = true;
            };

            property.Value = other;

            while (!change)
            {
                await Task.Delay(1);
            }
        }
        
        [Fact]
        public void OnPropertyChanged_Read_Only()
        {
            var metadata = JsonConvert.DeserializeObject<JObject>($@"{{
                ""readOnly"": true,
                ""links"":[ {{
                    ""rel"": ""property"",
                    ""href"": ""Test""
                }}]
            }}");
            
            var property = new Property<int>(_thing, _fixture.Create<string>(), _fixture.Create<int>(), metadata);

            Throws<PropertyException>(() => property.Value = _fixture.Create<int>());
        }
        
        [Fact]
        public void OnPropertyChanged_Type_Bool()
        {
            var metadata = JsonConvert.DeserializeObject<JObject>($@"{{
                ""type"": ""boolean"",
                ""links"":[ {{
                    ""rel"": ""property"",
                    ""href"": ""Test""
                }}]
            }}");
            
            var property = new Property<int>(_thing, _fixture.Create<string>(), _fixture.Create<int>(), metadata);

            Throws<PropertyException>(() => property.Value = _fixture.Create<int>());
        }
        
        [Fact]
        public async Task OnPropertyChanged_With_Metadata()
        {
            var metadata = JsonConvert.DeserializeObject<JObject>($@"{{
                ""links"":[ {{
                    ""rel"": ""property"",
                    ""href"": ""Test""
                }}]
            }}");
            
            var property = new Property<int>(_thing, _fixture.Create<string>(), _fixture.Create<int>(), metadata);

            int other = _fixture.Create<int>();
            int currentValue = property.Value;
            bool change = false;
            property.ValuedChanged += (sender, @event) =>
            {
                Equal(property, sender);
                False(@event.Value == currentValue);

                change = true;
            };

            property.Value = other;

            while (!change)
            {
                await Task.Delay(1);
            }
        }

        [Fact]
        public void AsPropertyDescription()
        {
            var property = new Property<int>(_thing, _fixture.Create<string>(), _fixture.Create<int>());
            
            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""links"":[ {{
                    ""rel"": ""property"",
                    ""href"": ""{property.HrefPrefix + property.Href}""
                }}]
            }}");
            
            JObject description = property.AsPropertyDescription();
            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public void AsPropertyDescription_With_Metadata()
        {
            var metadata = JsonConvert.DeserializeObject<JObject>($@"{{
                ""links"":[ {{
                    ""rel"": ""property"",
                    ""href"": ""Test""
                }}]
            }}");
            
            var property = new Property<int>(_thing, _fixture.Create<string>(), _fixture.Create<int>(), metadata);
            
            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""links"":[{{
                    ""rel"": ""property"",
                    ""href"": ""Test""
                 }}, {{
                    ""rel"": ""property"",
                    ""href"": ""{property.HrefPrefix + property.Href}""
                }}]
            }}");
            
            JObject description = property.AsPropertyDescription();
            True(JToken.DeepEquals(json, description));
        }
    }
}
