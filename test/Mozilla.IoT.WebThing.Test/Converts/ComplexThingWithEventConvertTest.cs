using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Converts
{
    public class ComplexThingWithEventConvertTest
    {
        private readonly Fixture _fixture;
        private readonly ThingConverterFactory _generate;
        private readonly JsonSerializerOptions _option;

        public ComplexThingWithEventConvertTest()
        {
            _fixture = new Fixture();
            _generate = new ThingConverterFactory();
            _option = new JsonSerializerOptions 
            {
                IgnoreNullValues =  true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        
        [Fact]
        public void ThingWithEvent_Convert()
        {
            var thing = new ThingWithEvent
            {
                Prefix = new Uri("https://mywebthingserver.com/")
            };
            var convert = new ThingConverter(
                new Dictionary<string, IThingConverter>
                {
                    ["ThingWithEvent"] =  _generate.Create(thing, _option)
                });
            
            _option.Converters.Clear();
            _option.Converters.Add(convert);
            var result = JsonSerializer.Serialize(thing, _option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse(@"{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""https://mywebthingserver.com/things/ThingWithEvent"",
                ""events"": { 
                    ""overheated"": {
                        ""type"": ""number"",
                        ""links"": [{
                            ""href"": ""/things/ThingWithEvent/events/overheated""
                        }]
                    },
                    ""Something"": {
                        ""links"": [{
                            ""href"": ""/things/ThingWithEvent/events/something""
                        }]
                    }
                },
                ""links"": [{
                    ""rel"": ""properties"",
                 ""href"": ""/things/ThingWithEvent/properties""
                   }, {
                    ""rel"": ""actions"",
                    ""href"": ""/things/ThingWithEvent/actions""
                },{
                    ""rel"": ""events"",
                    ""href"": ""/things/ThingWithEvent/events""
                }, {
                    ""rel"": ""alternate"",
                    ""href"": ""wss://mywebthingserver.com/things/ThingWithEvent""
                }
              ]
            }"));
        }
        
        [Fact]
        public void ThingWithEventAttribute_Convert()
        {
            var thing = new ThingWithEventAttribute
            {
                Prefix = new Uri("https://mywebthingserver.com/")
            };
            var convert = new ThingConverter(
            new Dictionary<string, IThingConverter>
                {
                    ["ThingWithEventAttribute"] =  _generate.Create(thing, _option)
                });
            
            _option.Converters.Clear();
            _option.Converters.Add(convert);

            var result = JsonSerializer.Serialize(thing,_option);

            var token = JToken.Parse(result);
            
            token.Should().BeEquivalentTo(JToken.Parse(@"{
                ""@context"": ""https://iot.mozilla.org/schemas"",
                ""id"": ""https://mywebthingserver.com/things/ThingWithEventAttribute"",
                ""events"": {
                    ""test"": {
                        ""title"": ""Overheated"",
                        ""description"": ""The lamp has exceeded its safe operating temperature"",
                        ""@type"": ""OverheatedEvent"",
                        ""unit"": ""degree celsius"",
                        ""type"": ""number"",
                        ""links"": [{
                            ""href"": ""/things/ThingWithEventAttribute/events/test""
                        }]
                    }
                },
                ""links"": [{
                    ""rel"": ""properties"",
                    ""href"": ""/things/ThingWithEventAttribute/properties""
                   }, {
                    ""rel"": ""actions"",
                    ""href"": ""/things/ThingWithEventAttribute/actions""
                   },{
                    ""rel"": ""events"",
                    ""href"": ""/things/ThingWithEventAttribute/events""
                   }, {
                    ""rel"": ""alternate"",
                    ""href"": ""wss://mywebthingserver.com/things/ThingWithEventAttribute""
                }]
            }"));
        }
        
        public class ThingWithEvent : Thing
        {
            public override string Name => "ThingWithEvent";

            public event EventHandler<double> Overheated;
            
            public event EventHandler Something;
        }
        
        public class ThingWithEventAttribute : Thing
        {
            public override string Name => "ThingWithEventAttribute";

            [ThingEvent(Name = "Test", Title = "Overheated", Description = "The lamp has exceeded its safe operating temperature", 
                Type = new []{ "OverheatedEvent" }, Unit = "degree celsius")]
            public event EventHandler<double> Overheated;
            
            [ThingEvent(Ignore = true)]
            public event EventHandler Ignore;
        }
    }
}
