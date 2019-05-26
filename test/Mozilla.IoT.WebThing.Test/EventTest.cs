using AutoFixture;
using Mozilla.IoT.WebThing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

using static Xunit.Assert;

namespace Mozilla.IoT.WebThing.Test
{
    public class EventTest
    {
        private readonly Fixture _fixture;

        public EventTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void AsEventDescription_Without_Data()
        {
            var @event = new Event<int>(_fixture.Create<Thing>(), _fixture.Create<string>());
            
            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""{@event.Name}"": {{
                    ""timestamp"": ""{@event.Time:yyyy-MM-ddTHH:mm:ss.fffffffZ}""
                }}
            }}");
            
            JObject description = @event.AsEventDescription();
            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public void AsEventDescription_With_Data()
        {
            var @event = new Event<int>(_fixture.Create<Thing>(), _fixture.Create<string>(), _fixture.Create<int>());
            
            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""{@event.Name}"": {{
                    ""timestamp"": ""{@event.Time:yyyy-MM-ddTHH:mm:ss.fffffffZ}"",
                    ""data"": {@event.Data}
                }}
            }}");
            
            JObject description = @event.AsEventDescription();
            True(JToken.DeepEquals(json, description));
        }
    }
}
