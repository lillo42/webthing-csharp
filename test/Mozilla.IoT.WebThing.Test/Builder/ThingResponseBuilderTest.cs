using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
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

            var message = System.Text.Json.JsonSerializer.Serialize(response, response.GetType(), _option.ToJsonSerializerOptions());
            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(@"
{
    ""events"": {
        ""int"": {
            ""link"": [
                {
                    ""href"": ""/thing/event-thing/events/int"",
                    ""rel"": ""event""
                }
            ]
        },
        ""test"": {
            ""title"": ""Bar"",
            ""description"": ""Foo"",
            ""unit"": ""milli"",
            ""link"": [
                {
                    ""href"": ""/thing/event-thing/events/test"",
                    ""rel"": ""event""
                }
            ]
        }
    },
    ""@context"": ""https://iot.mozilla.org/schemas""
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
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/bool"",
                    ""rel"": ""property""
                }
            ]
        },
        ""guid"": {
            ""type"": ""string"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/guid"",
                    ""rel"": ""property""
                }
            ]
        },
        ""timeSpan"": {
            ""type"": ""string"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/timeSpan"",
                    ""rel"": ""property""
                }
            ]
        },
        ""dateTime"": {
            ""type"": ""string"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/dateTime"",
                    ""rel"": ""property""
                }
            ]
        },
        ""dateTimeOffset"": {
            ""type"": ""string"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/dateTimeOffset"",
                    ""rel"": ""property""
                }
            ]
        },
        ""enum"": {
            ""type"": ""string"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/enum"",
                    ""rel"": ""property""
                }
            ]
        },
        ""string"": {
            ""type"": ""string"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/string"",
                    ""rel"": ""property""
                }
            ]
        },
        ""byte"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/byte"",
                    ""rel"": ""property""
                }
            ]
        },
        ""sbyte"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/sbyte"",
                    ""rel"": ""property""
                }
            ]
        },
        ""short"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/short"",
                    ""rel"": ""property""
                }
            ]
        },
        ""ushort"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/ushort"",
                    ""rel"": ""property""
                }
            ]
        },
        ""int"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/int"",
                    ""rel"": ""property""
                }
            ]
        },
        ""uint"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/uint"",
                    ""rel"": ""property""
                }
            ]
        },
        ""long"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/long"",
                    ""rel"": ""property""
                }
            ]
        },
        ""ulong"": {
            ""type"": ""integer"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/ulong"",
                    ""rel"": ""property""
                }
            ]
        },
        ""float"": {
            ""type"": ""number"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/float"",
                    ""rel"": ""property""
                }
            ]
        },
        ""double"": {
            ""type"": ""number"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/double"",
                    ""rel"": ""property""
                }
            ]
        },
        ""decimal"": {
            ""type"": ""number"",
            ""isReadOnly"": false,
            ""link"": [
                {
                    ""href"": ""/thing/property-thing/properties/decimal"",
                    ""rel"": ""property""
                }
            ]
        }
    },
    ""@context"": ""https://iot.mozilla.org/schemas""
}
"));
            
            void Visit(Type thingType)
            {
                var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => !IsThingProperty(x.Name));

                foreach (var property in properties)
                {
                    _builder.Add(property, null, 
                        new Information(null, null, null, null, null,
                            null, null, null, null, false, 
                            property.Name, _fixture.Create<bool>()));
                }
            }
            
            static bool IsThingProperty(string name)
                => name == nameof(Thing.Context)
                   || name == nameof(Thing.Name)
                   || name == nameof(Thing.Description)
                   || name == nameof(Thing.Title)
                   || name == nameof(Thing.Type)
                   || name == nameof(Thing.ThingContext);
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
  ""properties"": {
    ""bool2"": {
      ""title"": ""Boo Title"",
      ""description"": ""Bool test"",
      ""type"": ""boolean"",
      ""isReadOnly"": false,
      ""link"": [
        {
          ""href"": ""/thing/property-thing/properties/bool2"",
          ""rel"": ""property""
        }
      ]
    },
    ""guid2"": {
      ""type"": ""string"",
      ""isReadOnly"": true,
      ""link"": [
        {
          ""href"": ""/thing/property-thing/properties/guid2"",
          ""rel"": ""property""
        }
      ]
    },
    ""string2"": {
      ""title"": ""String title"",
      ""description"": ""String Description"",
      ""@type"": [""ABC"",""DEF""],
      ""type"": ""string"",
      ""isReadOnly"": false,
      ""minimumLength"": 1,
      ""maximumLength"": 100,
      ""pattern"": ""^([a-zA-Z0-9_\\-\\.]\u002B)@([a-zA-Z0-9_\\-\\.]\u002B)\\.([a-zA-Z]{2,5})$"",
      ""enums"": [ ""test@outlook.com"", ""test@gmail.com"", ""test@tese.com""],
      ""link"": [
        {
          ""href"": ""/thing/property-thing/properties/string2"",
          ""rel"": ""property""
        }
      ]
    },
    ""int2"": {
      ""title"": ""Int title"",
      ""description"": ""int Description"",
      ""@type"": ""ABC"",
      ""type"": ""integer"",
      ""isReadOnly"": false,
      ""minimum"": 1,
      ""enums"": [1, 2, 3],
      ""link"": [
        {
          ""href"": ""/thing/property-thing/properties/int2"",
          ""rel"": ""property""
        }
      ]
    },
    ""double2"": {
      ""title"": ""Double title"",
      ""description"": ""Double Description"",
      ""@type"": ""ABC"",
      ""type"": ""number"",
      ""isReadOnly"": false,
      ""exclusiveMinimum"": 1,
      ""exclusiveMaximum"": 100,
      ""enums"": [1.1, 2.3 ,3],
      ""link"": [
        {
          ""href"": ""/thing/property-thing/properties/double2"",
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
                    _builder.Add(property, attribute, ToInformation(attribute));
                }
            }
            
            static Information ToInformation(ThingPropertyAttribute attribute)
            {
                return new Information(attribute.MinimumValue, attribute.MaximumValue,
                    attribute.ExclusiveMinimumValue, attribute.ExclusiveMaximumValue,
                    attribute.MultipleOfValue, attribute.MinimumLengthValue, 
                    attribute.MaximumLengthValue, attribute.Pattern, attribute.Enum, 
                    attribute.IsReadOnly, attribute.Name!, false);
            }
        }

        public class EventThing : Thing
        {
            public override string Name => "event-thing";

            public event EventHandler<int> Int;
            
            [ThingEvent(Name = "Test", Description = "Foo", Title = "Bar", Unit = "milli")]
            public event EventHandler<string> String;
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
                MaximumLength = 100,
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
        
        public enum Foo
        {
            A,
            Bar,
            C
        }
    }
}
