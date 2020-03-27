using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Properties;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Intregration.Test.Factories
{
    public class PropertyTest : IThingContextFactoryTest
    {

        #region Response
        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(char))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        public void ResponseProperty(Type type)
        {
            if(type == typeof(bool))
            {
                TestResponseProperty<bool>("boolean");
                return;
            }

            #region String

            if(type == typeof(Guid))
            {
                
                TestResponseProperty<Guid>("string");
                return;
            }
            
            if(type == typeof(DateTime))
            {
                TestResponseProperty<DateTime>("string");
                return;
            }
            
            if(type == typeof(DateTimeOffset))
            {
                TestResponseProperty<DateTimeOffset>("string");
                return;
            }
            
            if(type == typeof(TimeSpan))
            {
                TestResponseProperty<TimeSpan>("string");
                return;
            }
            
            if(type == typeof(char))
            {
                TestResponseProperty<char>("string");
                return;
            }
            
            #endregion

            #region Integer

            if(type == typeof(byte))
            {
                TestResponseProperty<byte>("integer");
                return;
            }
            
            if(type == typeof(sbyte))
            {
                TestResponseProperty<sbyte>("integer");
                return;
            }
            
            if(type == typeof(short))
            {
                TestResponseProperty<short>("integer");;
                return;
            }
            
            if(type == typeof(ushort))
            {
                TestResponseProperty<ushort>("integer");
                return;
            }
            
            if(type == typeof(int))
            {
                TestResponseProperty<int>("integer");
                return;
            }
            
            if(type == typeof(uint))
            {
                TestResponseProperty<uint>("integer");
                return;
            }
            
            if(type == typeof(long))
            {
                TestResponseProperty<long>("integer");
                return;
            }
            
            if(type == typeof(long))
            {
                TestResponseProperty<ulong>("integer");
                return;
            }

            #endregion

            #region Number

            if(type == typeof(float))
            {
                TestResponseProperty<float>("number");
                return;
            }
            
            if(type == typeof(double))
            {
                TestResponseProperty<double>("number");
                return;
            }
            
            if(type == typeof(decimal))
            {
                TestResponseProperty<decimal>("number");
                return;
            }

            #endregion
        }

        private void TestResponseProperty<T>(string type)
            where T : struct
        {
            var thing = new PropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            var message = JsonSerializer.Serialize(context.Response,
                new ThingOption().ToJsonSerializerOptions());

            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse($@"
{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""properties"": {{
    ""value"": {{
      ""type"": ""{type}"",
      ""isReadOnly"": false,
      ""link"": [
        {{
          ""href"": ""/things/property-thing/properties/value"",
          ""rel"": ""property""
        }}
      ]
    }},
    ""nullableValue"": {{
      ""type"": ""{type}"",
      ""isReadOnly"": false,
      ""link"": [
        {{
          ""href"": ""/things/property-thing/properties/nullableValue"",
          ""rel"": ""property""
        }}
      ]
    }}
  }},
  ""links"": [
    {{
      ""href"": ""properties"",
      ""rel"": ""/things/property-thing/properties""
    }},
    {{
      ""href"": ""events"",
      ""rel"": ""/things/property-thing/events""
    }},
    {{
      ""href"": ""actions"",
      ""rel"": ""/things/property-thing/actions""
    }}
  ]
}}
"));
        }

        #endregion
        
        #region Valid Property

        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(Foo))]
        [InlineData(typeof(char))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        public void ValidProperty(Type type)
        {
            if(type == typeof(bool))
            {
                TestValidProperty<bool>(x => 
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x.ToString().ToLower()} }}")
                    .GetProperty("input"));
                return;
            }

            #region String

            if(type == typeof(Guid))
            {
                TestValidProperty<Guid>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(DateTime))
            {
                TestValidProperty<DateTime>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x:O}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(DateTimeOffset))
            {
                TestValidProperty<DateTimeOffset>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x:O}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(TimeSpan))
            {
                TestValidProperty<TimeSpan>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(char))
            {
                TestValidProperty<char>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(Foo))
            {
                TestValidProperty<Foo>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }

            #endregion

            #region Integer

            if(type == typeof(byte))
            {
                TestValidProperty<byte>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(sbyte))
            {
                TestValidProperty<byte>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(short))
            {
                TestValidProperty<short>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(ushort))
            {
                TestValidProperty<ushort>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(int))
            {
                TestValidProperty<int>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(uint))
            {
                TestValidProperty<uint>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(long))
            {
                TestValidProperty<long>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(long))
            {
                TestValidProperty<uint>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            #endregion

            #region Number

            if(type == typeof(float))
            {
                TestValidProperty<float>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(double))
            {
                TestValidProperty<double>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(decimal))
            {
                TestValidProperty<decimal>(x => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            #endregion
        }


        private void TestValidProperty<T>(Func<T, JsonElement> createJsonElement)
            where T : struct
        {
            var thing = new PropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(2);
            context.Properties.Should().ContainKey(nameof(PropertyThing<T>.Value));
            context.Properties.Should().ContainKey(nameof(PropertyThing<T>.NullableValue));

            var value = Fixture.Create<T>();
            var jsonElement = createJsonElement(value);
            
            context.Properties[nameof(PropertyThing<T>.Value)].SetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().Be(value);
            context.Properties[nameof(PropertyThing<T>.Value)].GetValue().Should().Be(value);
            
            context.Properties[nameof(PropertyThing<T>.NullableValue)].SetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().Be(value);
            context.Properties[nameof(PropertyThing<T>.NullableValue)].GetValue().Should().Be(value);
            
            jsonElement =  JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
            context.Properties[nameof(PropertyThing<T>.NullableValue)].SetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.NullableValue.Should().BeNull();
            context.Properties[nameof(PropertyThing<T>.NullableValue)].GetValue().Should().BeNull();
        }

        #endregion
        
        
        
        public class PropertyThing<T> : Thing
            where T : struct
        {
            public override string Name => "property-thing";
            
            public T Value { get; set; }
            public T? NullableValue { get; set; }
        }
        
        public enum Foo
        {
            A,
            Bar,
            C
        }
    }
}
