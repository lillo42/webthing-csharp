using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Builder
{
    public class ThingResponseBuilderTest
    {
        private readonly ThingOption _option;
        private readonly ThingResponseBuilder _builder;
        private readonly Fixture _fixture;

        public ThingResponseBuilderTest()
        {
            _builder = new ThingResponseBuilder();
            _fixture = new Fixture();
            _option = new ThingOption
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        #region Set

        [Fact]
        public void SetThing()
        {
            _builder.SetThing(new PropertyThing());
        }
        
        [Fact]
        public void SetThingAfterSetThingOption()
        {
            _builder.SetThingOption(new ThingOption())
                .SetThing(new PropertyThing());
        }
        
        [Fact]
        public void SetThingOption()
        {
            _builder.SetThingOption(new ThingOption());
        }
        
        [Fact]
        public void SetThingOptionSetThingAfter()
        {
            _builder.SetThing(new PropertyThing())
                .SetThingOption(new ThingOption());
        }
        

        #endregion

        #region Event

        [Fact]
        public void AddEvent_Should_Throw_When_SetThingWasNotCalled()
        {
            Assert.Throws<InvalidOperationException>(() => _builder.Add(Substitute.For<EventInfo>(), null));
        }
        
        [Fact]
        public void AddEvent_Should_Throw_When_SetThingOptionWasNotCalled()
        {
            Assert.Throws<InvalidOperationException>(() => _builder
                .SetThing(new PropertyThing())
                .Add(Substitute.For<EventInfo>(), null));
        }

        #endregion

        [Fact]
        public void TryAddWhenSetThingIsNotCalled() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Add(Substitute.For<EventInfo>(), null));
        
        [Fact]
        public void TryBuildWhenSetThingIsNotCalled() 
            => Assert.Throws<InvalidOperationException>(() =>  _builder.Build());
        
        [Fact]
        public void BuildWithEvent()
        {
            var thing = new EventThing();
            _builder
                .SetThing(thing)
                .SetThingOption(_option);
            
            Visit(thing.GetType());
            
            var response = _builder.Build();
            response.Should().NotBeNull();

            var message = JsonSerializer.Serialize(response, response.GetType(), _option.ToJsonSerializerOptions());
            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(@"
{
    ""events"": {
        ""int"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/event-thing/events/int"",
                    ""rel"": ""event""
                }
            ]
        },
        ""test"": {
            ""title"": ""Bar"",
            ""description"": ""Foo"",
            ""unit"": ""milli"",
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/event-thing/events/test"",
                    ""rel"": ""event""
                }
            ]
        }
    },
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {
        ""nosec_sc"": {
            ""scheme"": ""nosec""
        }
    },
    ""links"": [{
        ""rel"": ""properties"",
        ""href"": ""/things/event-thing/properties""
      },{
        ""rel"": ""actions"",
        ""href"": ""/things/event-thing/actions""
      },{
        ""rel"": ""events"",
        ""href"": ""/things/event-thing/events""
    }]
}
"));
            
            void Visit(Type thingType)
            {
                var events = thingType.GetEvents(BindingFlags.Public | BindingFlags.Instance);

                foreach (var @event in events)
                {
                    var args = @event.EventHandlerType!.GetGenericArguments();
                    if (args.Length > 1)
                    {
                        continue;
                    }

                    if ((args.Length == 0 && @event.EventHandlerType != typeof(EventHandler))
                        || (args.Length == 1 && @event.EventHandlerType != typeof(EventHandler<>).MakeGenericType(args[0])))
                    {
                        continue;
                    }
                
                    _builder.Add(@event, @event.GetCustomAttribute<ThingEventAttribute>());
                }
            }
        }
        
        [Fact]
        public void BuildWithProperties()
        {
            var thing = new PropertyThing();
            _builder
                .SetThing(thing)
                .SetThingOption(_option);
            
            Visit(thing.GetType());
            
            var response = _builder.Build();
            response.Should().NotBeNull();

            var message = JsonSerializer.Serialize(response, response.GetType(), _option.ToJsonSerializerOptions());
            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(@"
{
    ""properties"": {
        ""bool"": {
            ""type"": ""boolean"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/bool"",
                    ""rel"": ""property""
                }
            ]
        },
        ""guid"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/guid"",
                    ""rel"": ""property""
                }
            ]
        },
        ""timeSpan"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/timeSpan"",
                    ""rel"": ""property""
                }
            ]
        },
        ""dateTime"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/dateTime"",
                    ""rel"": ""property""
                }
            ]
        },
        ""dateTimeOffset"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/dateTimeOffset"",
                    ""rel"": ""property""
                }
            ]
        },
        ""enum"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/enum"",
                    ""rel"": ""property""
                }
            ]
        },
        ""string"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/string"",
                    ""rel"": ""property""
                }
            ]
        },
        ""byte"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/byte"",
                    ""rel"": ""property""
                }
            ]
        },
        ""sbyte"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/sbyte"",
                    ""rel"": ""property""
                }
            ]
        },
        ""short"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/short"",
                    ""rel"": ""property""
                }
            ]
        },
        ""ushort"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/ushort"",
                    ""rel"": ""property""
                }
            ]
        },
        ""int"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/int"",
                    ""rel"": ""property""
                }
            ]
        },
        ""uint"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/uint"",
                    ""rel"": ""property""
                }
            ]
        },
        ""long"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/long"",
                    ""rel"": ""property""
                }
            ]
        },
        ""ulong"": {
            ""type"": ""integer"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/ulong"",
                    ""rel"": ""property""
                }
            ]
        },
        ""float"": {
            ""type"": ""number"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/float"",
                    ""rel"": ""property""
                }
            ]
        },
        ""double"": {
            ""type"": ""number"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/double"",
                    ""rel"": ""property""
                }
            ]
        },
        ""decimal"": {
            ""type"": ""number"",
            ""links"": [
                {
                    ""href"": ""/things/property-thing/properties/decimal"",
                    ""rel"": ""property""
                }
            ]
        }
    },
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {
        ""nosec_sc"": {
            ""scheme"": ""nosec""
        }
    },
    ""links"": [{
        ""rel"": ""properties"",
        ""href"": ""/things/property-thing/properties""
      },{
        ""rel"": ""actions"",
        ""href"": ""/things/property-thing/actions""
      },{
        ""rel"": ""events"",
        ""href"": ""/things/property-thing/events""
    }]
}
"));
            
            void Visit(Type thingType)
            {
                var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => !IsThingProperty(x.Name));

                var schema = Substitute.For<IJsonSchema>();
                schema.Pattern.Returns((string)null);
                
                foreach (var property in properties)
                {
                    _builder.Add(property, null, 
                        new JsonSchema(schema, null, property.PropertyType.ToJsonType(),
                            property.Name!, _fixture.Create<bool>()));
                }
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

        [Fact]
        public void BuildWithPropertiesInformation()
        {
            var thing = new PropertyThing();

            _builder
                .SetThing(thing)
                .SetThingOption(_option);

            Visit(thing.GetType());

            var response = _builder.Build();
            response.Should().NotBeNull();

            var message = JsonSerializer.Serialize(response, response.GetType(), _option.ToJsonSerializerOptions());
            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(@"
{
  ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {
        ""nosec_sc"": {
            ""scheme"": ""nosec""
        }
    },
   ""links"": [{
        ""rel"": ""properties"",
        ""href"": ""/things/property-thing/properties""
      },{
        ""rel"": ""actions"",
        ""href"": ""/things/property-thing/actions""
      },{
        
        ""rel"": ""events"",
        ""href"": ""/things/property-thing/events""
   }],
   ""properties"": {
    ""bool2"": {
      ""title"": ""Boo Title"",
      ""description"": ""Bool test"",
      ""type"": ""boolean"",
      ""links"": [
        {
          ""href"": ""/things/property-thing/properties/bool2"",
          ""rel"": ""property""
        }
      ]
    },
    ""guid2"": {
      ""type"": ""string"",
      ""readOnly"": true,
      ""links"": [
        {
          ""href"": ""/things/property-thing/properties/guid2"",
          ""rel"": ""property""
        }
      ]
    },
    ""string2"": {
      ""title"": ""String title"",
      ""description"": ""String Description"",
      ""@type"": [""ABC"",""DEF""],
      ""type"": ""string"",
      ""minLength"": 1,
      ""maxLength"": 100,
      ""pattern"": ""^([a-zA-Z0-9_\\-\\.]\u002B)@([a-zA-Z0-9_\\-\\.]\u002B)\\.([a-zA-Z]{2,5})$"",
      ""enum"": [ ""test@outlook.com"", ""test@gmail.com"", ""test@tese.com""],
      ""links"": [
        {
          ""href"": ""/things/property-thing/properties/string2"",
          ""rel"": ""property""
        }
      ]
    },
    ""int2"": {
      ""title"": ""Int title"",
      ""description"": ""int Description"",
      ""@type"": ""ABC"",
      ""type"": ""integer"",
      ""minimum"": 1,
      ""maximum"": 100,
      ""enum"": [1, 2, 3],
      ""links"": [
        {
          ""href"": ""/things/property-thing/properties/int2"",
          ""rel"": ""property""
        }
      ]
    },
    ""double2"": {
      ""title"": ""Double title"",
      ""description"": ""Double Description"",
      ""@type"": ""ABC"",
      ""type"": ""number"",
      ""exclusiveMinimum"": 1,
      ""exclusiveMaximum"": 100,
      ""enum"": [1.1, 2.3 ,3],
      ""links"": [
        {
          ""href"": ""/things/property-thing/properties/double2"",
          ""rel"": ""property""
        }
      ]
    }
  }
}
"));
            
            void Visit(Type thingType)
            {
                var p = new[] {  
                    nameof(PropertyThing.Bool), 
                    nameof(PropertyThing.Guid), 
                    nameof(PropertyThing.String), 
                    nameof(PropertyThing.Int), 
                    nameof(PropertyThing.Double) 
                };
                var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => p.Contains(x.Name));

                foreach (var property in properties)
                {
                    var attribute = property.GetCustomAttribute<ThingPropertyAttribute>();
                    _builder.Add(property, attribute, ToInformation(attribute, property.PropertyType.ToJsonType()));
                }
            }
            
            static JsonSchema ToInformation(ThingPropertyAttribute attribute, JsonType jsonType)
            {
                return new JsonSchema(attribute, attribute.Enum, jsonType, attribute.Name!, false);
            }
        }
        
        [Fact]
        public void BuildWithActions()
        {
            var thing = new ActionThing();
            
            _builder
                .SetThing(thing)
                .SetThingOption(_option);
            
            Visit(thing.GetType());
            
            var response = _builder.Build();
            response.Should().NotBeNull();

            var message = JsonSerializer.Serialize(response, response.GetType(), _option.ToJsonSerializerOptions());
            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(@"
{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {
        ""nosec_sc"": {
            ""scheme"": ""nosec""
        }
    },
    ""links"": [{
        ""rel"": ""properties"",
        ""href"": ""/things/action-thing/properties""
      },{
        ""rel"": ""actions"",
        ""href"": ""/things/action-thing/actions""
      },{        
        ""rel"": ""events"",
        ""href"": ""/things/action-thing/events""
    }],
    ""actions"": {
      ""noParameter"": {
        ""links"": [
          {
            ""href"": ""/things/action-thing/actions/noParameter"",
            ""rel"": ""action""
          }
        ],
        ""input"": {
          ""type"": ""object"",
          ""properties"": {}
        }
      },
      ""withParameter"": {
        ""links"": [
          {
            ""href"": ""/things/action-thing/actions/withParameter"",
            ""rel"": ""action""
          }
        ],
        ""input"": {
          ""type"": ""object"",
          ""properties"": {
            ""bool"": {
              ""type"": ""boolean""
            },
            ""guid"": {
              ""type"": ""string""
            },
            ""timeSpan"": {
              ""type"": ""string""
            },
            ""dateTime"": {
              ""type"": ""string""
            },
            ""dateTimeOffset"": {
              ""type"": ""string""
            },
            ""foo"": {
              ""type"": ""string""
            },
            ""string"": {
              ""type"": ""string""
            },
            ""byte"": {
              ""type"": ""integer""
            },
            ""sbyte"": {
              ""type"": ""integer""
            },
            ""short"": {
              ""type"": ""integer""
            },
            ""ushort"": {
              ""type"": ""integer""
            },
            ""int"": {
              ""type"": ""integer""
            },
            ""uint"": {
              ""type"": ""integer""
            },
            ""long"": {
              ""type"": ""integer""
            },
            ""ulong"": {
              ""type"": ""integer""
            },
            ""float"": {
              ""type"": ""number""
            },
            ""double"": {
              ""type"": ""number""
            },
            ""decimal"": {
              ""type"": ""number""
            }
          }
        }
      }
    }
  }
"));
            
            void Visit(Type thingType)
            {
                var methods = thingType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where( x => !x.IsSpecialName
                                 && x.Name != nameof(Equals) && x.Name != nameof(GetType) 
                                 && x.Name != nameof(GetHashCode) && x.Name != nameof(ToString));

                foreach (var method in methods)
                {
                    _builder.Add(method, null);

                    var schema = Substitute.For<IJsonSchema>();
                    schema.Pattern.Returns((string)null);
                    
                    foreach (var parameter in method.GetParameters())
                    {
                        _builder.Add(parameter, null, 
                            new JsonSchema(schema, null, parameter.ParameterType.ToJsonType(),
                                parameter.Name!, _fixture.Create<bool>()));
                    }
                }
            }
        }
        
        [Fact]
        public void BuildWithActionsWithInformation()
        {
            var thing = new ActionThing();
            
            _builder
                .SetThing(thing)
                .SetThingOption(_option);
            
            Visit(thing.GetType());
            
            var response = _builder.Build();
            response.Should().NotBeNull();

            var message = JsonSerializer.Serialize(response, response.GetType(), _option.ToJsonSerializerOptions());
            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(@"
{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {
        ""nosec_sc"": {
            ""scheme"": ""nosec""
        }
    },
    ""links"": [{
        ""rel"": ""properties"",
        ""href"": ""/things/action-thing/properties""
      },{
        ""rel"": ""actions"",
        ""href"": ""/things/action-thing/actions""
      },{
        ""rel"": ""events"",
        ""href"": ""/things/action-thing/events""
    }],
    ""actions"": {
      ""test"": {
        ""title"": ""Ola"",
        ""description"": ""teste 2"",
        ""links"": [
          {
            ""href"": ""/things/action-thing/actions/test"",
            ""rel"": ""action""
          }
        ],
        ""input"": {
          ""@type"": ""ABC"",
          ""type"": ""object"",
          ""properties"": {}
        }
      },
      ""withParameter"": {
        ""links"": [
          {
            ""href"": ""/things/action-thing/actions/withParameter"",
            ""rel"": ""action""
          }
        ],
        ""input"": {
          ""@type"": [
            ""ABC"",
            ""DEF""
          ],
          ""type"": ""object"",
          ""properties"": {
            ""bool2"": {
              ""title"": ""Boo Title"",
              ""description"": ""Bool test"",
              ""type"": ""boolean""
            },
            ""guid2"": {
              ""type"": ""string""
            },
            ""timeSpan"": {
              ""type"": ""string""
            },
            ""dateTime"": {
              ""type"": ""string""
            },
            ""dateTimeOffset"": {
              ""type"": ""string""
            },
            ""foo"": {
              ""type"": ""string""
            },
            ""string2"": {
              ""title"": ""String title"",
              ""description"": ""String Description"",
              ""type"": ""string"",
              ""minLength"": 1,
              ""maxLength"": 100,
              ""pattern"": ""^([a-zA-Z0-9_\\-\\.]\u002B)@([a-zA-Z0-9_\\-\\.]\u002B)\\.([a-zA-Z]{2,5})$"",
              ""enum"": [
                ""test@outlook.com"",
                ""test@gmail.com"",
                ""test@tese.com""
              ]
            },
            ""byte"": {
              ""type"": ""integer""
            },
            ""sbyte"": {
              ""type"": ""integer""
            },
            ""short"": {
              ""type"": ""integer""
            },
            ""ushort"": {
              ""type"": ""integer""
            },
            ""int2"": {
              ""title"": ""Int title"",
              ""description"": ""int Description"",
              ""type"": ""integer"",
              ""minimum"": 1,
              ""maximum"": 100,
              ""enum"": [
                1,
                2,
                3
              ]
            },
            ""uint"": {
              ""type"": ""integer""
            },
            ""long"": {
              ""type"": ""integer""
            },
            ""ulong"": {
              ""type"": ""integer""
            },
            ""float"": {
              ""type"": ""number""
            },
            ""double2"": {
              ""title"": ""Double title"",
              ""description"": ""Double Description"",
              ""type"": ""number"",
              ""exclusiveMinimum"": 1,
              ""exclusiveMaximum"": 100,
              ""enum"": [
                1.1,
                2.3,
                3
              ]
            },
            ""decimal"": {
              ""type"": ""number""
            }
          }
        }
      }
    }
  }
"));
            
            void Visit(Type thingType)
            {
                var methods = thingType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where( x => !x.IsSpecialName
                                 && x.Name != nameof(Equals) && x.Name != nameof(GetType) 
                                 && x.Name != nameof(GetHashCode) && x.Name != nameof(ToString));

                foreach (var method in methods)
                {
                    _builder.Add(method, method.GetCustomAttribute<ThingActionAttribute>());

                    foreach (var parameter in method.GetParameters())
                    {
                        _builder.Add(parameter, parameter.GetCustomAttribute<ThingParameterAttribute>(), 
                            ToInformation(parameter.GetCustomAttribute<ThingParameterAttribute>(),
                                parameter.ParameterType.ToJsonType(),
                                parameter.Name));
                    }
                }
            }
            
            JsonSchema ToInformation(ThingParameterAttribute attribute, JsonType jsonType, string name)
            {
                return new JsonSchema(attribute, attribute?.Enum, jsonType, 
                    attribute?.Name ?? name, _fixture.Create<bool>());
            }
        }

        #region Thing
        
        public class EventThing : Thing
        {
            public override string Name => "event-thing";
#pragma warning disable 67
            public event EventHandler<int> Int;
            
            [ThingEvent(Name = "Test", Description = "Foo", Title = "Bar", Unit = "milli")]
            public event EventHandler<string> String;
#pragma warning restore 67
        }
        
        public class PropertyThing : Thing
        {
            public override string Name => "property-thing";
            
            [ThingProperty(Name = "bool2",
                Title = "Boo Title",
                Description = "Bool test")]
            public bool Bool { get; set; }
            
            #region String
            [ThingProperty(Name = "Guid2", IsReadOnly = true)]
            public Guid Guid { get; set; }
            public TimeSpan TimeSpan { get; set; }
            public DateTime DateTime { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
            public Foo Enum { get; set; }
            
            [ThingProperty(Name = "String2", 
                Title = "String title",
                Description = "String Description",
                MinimumLength = 1,
                MaximumLength = 100,
                Pattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$",
                Enum = new object[]{ "test@outlook.com", "test@gmail.com", "test@tese.com" },
                Type = new[] { "ABC", "DEF"})]
            public string String { get; set; }
            #endregion

            #region Number
            public byte Byte { get; set; }
            public sbyte Sbyte { get; set; }
            public short Short { get; set; }
            public ushort Ushort { get; set; }
            
            [ThingProperty(Name = "Int2", 
                Title = "Int title",
                Description = "int Description",
                Minimum = 1,
                Maximum = 100,
                Enum = new object[]{ 1, 2, 3 },
                Type = new[] { "ABC" })]
            public int Int { get; set; }
            public uint Uint { get; set; }
            public long Long { get; set; }
            public ulong Ulong { get; set; }
            public float Float { get; set; }
            
            [ThingProperty(Name = "Double2", 
                Title = "Double title",
                Description = "Double Description",
                ExclusiveMinimum = 1,
                ExclusiveMaximum = 100,
                Enum = new object[]{ 1.1, 2.3, 3 },
                Type = new[] { "ABC" })]
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            #endregion
        }
        
        public class ActionThing : Thing
        {
            public override string Name => "action-thing";

            [ThingAction(Name = "test", Description = "teste 2", Title = "Ola", Type = new []{ "ABC" })]
            public void NoParameter()
            {
                
            }

            [ThingAction(Type = new []{ "ABC", "DEF" })]
            public void WithParameter(
                [ThingParameter(Name = "bool2", Title = "Boo Title", Description = "Bool test")]bool @bool,
                [ThingParameter(Name = "Guid2")]Guid guid,
                TimeSpan timeSpan,
                DateTime dateTime,
                DateTimeOffset dateTimeOffset,
                Foo foo,
                [ThingParameter(Name = "String2", Title = "String title", Description = "String Description",
                    MinimumLength = 1, MaximumLength = 100, Pattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$",
                    Enum = new object[]{ "test@outlook.com", "test@gmail.com", "test@tese.com" })]string @string,
                byte @byte,
                sbyte @sbyte,
                short @short,
                ushort @ushort,
                [ThingParameter(Name = "Int2", Title = "Int title", Description = "int Description",
                    Minimum = 1, Maximum = 100, Enum = new object[]{ 1, 2, 3 })]int @int,
                uint @uint,
                long @long,
                ulong @ulong,
                float @float,
                [ThingParameter(Name = "Double2", Title = "Double title", Description = "Double Description",
                    ExclusiveMinimum = 1, ExclusiveMaximum = 100, Enum = new object[]{ 1.1, 2.3, 3 })]double @double,
                decimal @decimal
            )
            {
            }
        }
        
        public enum Foo
        {
            A,
            Bar,
            C
        }
        
        #endregion
    }
}
