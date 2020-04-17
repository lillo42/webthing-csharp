using System;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Extensions;
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
        [InlineData(typeof(Foo))]
        [InlineData(typeof(string))]
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
            if (type == typeof(bool))
            {
                TestResponseStructProperty<bool>("boolean");
                return;
            }

            #region String

            if (type == typeof(Guid))
            {

                TestResponseStructProperty<Guid>("string");
                return;
            }

            if (type == typeof(DateTime))
            {
                TestResponseStructProperty<DateTime>("string");
                return;
            }

            if (type == typeof(DateTimeOffset))
            {
                TestResponseStructProperty<DateTimeOffset>("string");
                return;
            }

            if (type == typeof(TimeSpan))
            {
                TestResponseStructProperty<TimeSpan>("string");
                return;
            }

            if (type == typeof(char))
            {
                TestResponseStructProperty<char>("string");
                return;
            }

            if (type == typeof(string))
            {
                TestResponseNullableProperty<string>("string");
                return;
            }

            if (type == typeof(string))
            {
                TestResponseNullableProperty<string>("string");
                return;
            }

            #endregion

            #region Integer

            if (type == typeof(byte))
            {
                TestResponseStructProperty<byte>("integer");
                return;
            }

            if (type == typeof(sbyte))
            {
                TestResponseStructProperty<sbyte>("integer");
                return;
            }

            if (type == typeof(short))
            {
                TestResponseStructProperty<short>("integer");
                ;
                return;
            }

            if (type == typeof(ushort))
            {
                TestResponseStructProperty<ushort>("integer");
                return;
            }

            if (type == typeof(int))
            {
                TestResponseStructProperty<int>("integer");
                return;
            }

            if (type == typeof(uint))
            {
                TestResponseStructProperty<uint>("integer");
                return;
            }

            if (type == typeof(long))
            {
                TestResponseStructProperty<long>("integer");
                return;
            }

            if (type == typeof(long))
            {
                TestResponseStructProperty<ulong>("integer");
                return;
            }

            #endregion

            #region Number

            if (type == typeof(float))
            {
                TestResponseStructProperty<float>("number");
                return;
            }

            if (type == typeof(double))
            {
                TestResponseStructProperty<double>("number");
                return;
            }

            if (type == typeof(decimal))
            {
                TestResponseStructProperty<decimal>("number");
                return;
            }

            #endregion
        }

        private void TestResponseStructProperty<T>(string type)
            where T : struct
        {
            TestResponseProperty<StructPropertyThing<T>>(type, string.Format(RESPONSE_WITH_NULLABLE, type,
                typeof(T).IsEnum
                    ? $@" ""enums"": [""{string.Join(@""" , """, typeof(T).GetEnumNames())}""] "
                    : string.Empty));
        }

        private void TestResponseNullableProperty<T>(string type)
            => TestResponseProperty<NullablePropertyThing<T>>(type,
                string.Format(RESPONSE_WITHOUT_NULLABLE, type, string.Empty));

        private void TestResponseProperty<T>(string type, string response)
            where T : Thing, new()
        {
            var thing = new T();
            var context = Factory.Create(thing, new ThingOption());

            var message = JsonSerializer.Serialize(context.Response,
                new ThingOption().ToJsonSerializerOptions());

            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(response));
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
        [InlineData(typeof(string))]
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
            if (type == typeof(bool))
            {
                TestValidProperty<bool>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x.ToString().ToLower()} }}")
                        .GetProperty("input"));
                return;
            }

            #region String

            if (type == typeof(Guid))
            {
                TestValidProperty<Guid>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(DateTime))
            {
                TestValidProperty<DateTime>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x:O}"" }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(DateTimeOffset))
            {
                TestValidProperty<DateTimeOffset>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x:O}"" }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(TimeSpan))
            {
                TestValidProperty<TimeSpan>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(char))
            {
                TestValidProperty<char>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(Foo))
            {
                TestValidProperty<Foo>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(string))
            {
                TestValidNullableProperty<string>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{x}"" }}")
                        .GetProperty("input"));
                return;
            }

            #endregion

            #region Integer

            if (type == typeof(byte))
            {
                TestValidProperty<byte>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(sbyte))
            {
                TestValidProperty<byte>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(short))
            {
                TestValidProperty<short>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(ushort))
            {
                TestValidProperty<ushort>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(int))
            {
                TestValidProperty<int>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(uint))
            {
                TestValidProperty<uint>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(long))
            {
                TestValidProperty<long>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(long))
            {
                TestValidProperty<uint>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            #endregion

            #region Number

            if (type == typeof(float))
            {
                TestValidProperty<float>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(double))
            {
                TestValidProperty<double>(x =>
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {x} }}")
                        .GetProperty("input"));
                return;
            }

            if (type == typeof(decimal))
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
            var thing = new StructPropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(2);
            context.Properties.Should().ContainKey(nameof(StructPropertyThing<T>.Value));
            context.Properties.Should().ContainKey(nameof(StructPropertyThing<T>.NullableValue));

            var value = Fixture.Create<T>();
            var jsonElement = createJsonElement(value);
            
            context.Properties[nameof(StructPropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().Be(value);
            context.Properties[nameof(StructPropertyThing<T>.Value)].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().Be(value);
            
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.NullableValue.Should().Be(value);
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().Be(value);
            
            jsonElement =  JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.NullableValue.Should().BeNull();
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().BeNull();
        }
        
        private void TestValidNullableProperty<T>(Func<T, JsonElement> createJsonElement)
        {
            var thing = new NullablePropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(1);
            context.Properties.Should().ContainKey(nameof(NullablePropertyThing<T>.Value));

            var value = Fixture.Create<T>();
            var jsonElement = createJsonElement(value);
            
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().Be(value);
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().Be(value);

            jsonElement =  JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().BeNull();
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().BeNull();
        }

        #endregion

        #region Invalid Property

        [Theory]
        [InlineData(typeof(bool))]
        
        [InlineData(typeof(Guid))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(Foo))]
        
        [InlineData(typeof(char))]
        [InlineData(typeof(string))]
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
        public void InvalidProperty(Type type)
        {
            if(type == typeof(bool))
            {
                TestInvalidValidProperty<bool>(() =>
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                    .GetProperty("input"));
                return;
            }

            #region String

            if(type == typeof(Guid))
            {
                TestInvalidValidProperty<Guid>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<DateTime>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(DateTime))
            {
                TestInvalidValidProperty<DateTime>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(DateTimeOffset))
            {
                TestInvalidValidProperty<DateTimeOffset>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(TimeSpan))
            {
                TestInvalidValidProperty<TimeSpan>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(char))
            {
                TestInvalidValidProperty<char>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(Foo))
            {
                TestInvalidValidProperty<Foo>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(string))
            {
                TestInvalidNullableProperty<string>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                        .GetProperty("input"));
                return;
            }

            #endregion

            #region Integer

            if(type == typeof(byte))
            {
                TestInvalidValidProperty<byte>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(sbyte))
            {
                TestInvalidValidProperty<sbyte>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(short))
            {
                TestInvalidValidProperty<short>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(ushort))
            {
                TestInvalidValidProperty<ushort>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(int))
            {
                TestInvalidValidProperty<int>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(uint))
            {
                TestInvalidValidProperty<uint>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(long))
            {
                TestInvalidValidProperty<long>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(ulong))
            {
                TestInvalidValidProperty<ulong>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }

            #endregion

            #region Number

            if(type == typeof(float))
            {
                TestInvalidValidProperty<float>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(double))
            {
                TestInvalidValidProperty<double>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }
            
            if(type == typeof(decimal))
            {
                TestInvalidValidProperty<decimal>(() => 
                    JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                        .GetProperty("input"));
                return;
            }

            #endregion

            #region Array

            if (type == typeof(bool[]))
            {
                
            }

            #endregion
        }
        
        private void TestInvalidValidProperty<T>(Func<JsonElement> createJsonElement)
            where T : struct
        {
            var thing = new StructPropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(2);
            context.Properties.Should().ContainKey(nameof(StructPropertyThing<T>.Value));
            context.Properties.Should().ContainKey(nameof(StructPropertyThing<T>.NullableValue));

            var value = Fixture.Create<T>();
            var jsonElement = createJsonElement();
            
            var defaultValue = Fixture.Create<T>();
            thing.Value = defaultValue;
            context.Properties[nameof(StructPropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.InvalidValue);
            thing.Value.Should().NotBe(value);
            thing.Value.Should().Be(defaultValue);
            context.Properties[nameof(StructPropertyThing<T>.Value)].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().Be(defaultValue);

            thing.NullableValue = defaultValue;
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.InvalidValue);
            thing.NullableValue.Should().NotBe(value);
            thing.NullableValue.Should().Be(defaultValue);
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().Be(defaultValue);
        }
        
        private void TestInvalidNullableProperty<T>(Func<JsonElement> createJsonElement)
        {
            var thing = new NullablePropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(1);
            context.Properties.Should().ContainKey(nameof(NullablePropertyThing<T>.Value));

            var value = Fixture.Create<T>();
            var jsonElement = createJsonElement();

            var defaultValue = Fixture.Create<T>();
            thing.Value = defaultValue;
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.InvalidValue);
            thing.Value.Should().NotBe(value);
            thing.Value.Should().Be(defaultValue);
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().Be(defaultValue);
        }
        #endregion
        
        public class StructPropertyThing<T> : Thing
            where T : struct
        {
            public override string Name => "property-thing";
            
            public T Value { get; set; }
            public T? NullableValue { get; set; }
        }
        
        public class NullablePropertyThing<T> : Thing
        {
            public override string Name => "property-thing";
            public T Value { get; set; }
        }

        public enum Foo
        {
            A,
            Bar,
            C
        }

        private const string RESPONSE_WITH_NULLABLE = @"{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""properties"": {{
    ""value"": {{
      ""type"": ""{0}"",
      ""isReadOnly"": false,
      {1}
      ""link"": [
        {{
          ""href"": ""/things/property-thing/properties/value"",
          ""rel"": ""property""
        }}
      ]
    }},
    ""nullableValue"": {{
      ""type"": ""{0}"",
      ""isReadOnly"": false,
      {1}
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
}}";
        
        private const string RESPONSE_WITHOUT_NULLABLE = @"{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""properties"": {{
    ""value"": {{
      ""type"": ""{0}"",
      ""isReadOnly"": false,
      {1}
      ""link"": [
        {{
          ""href"": ""/things/property-thing/properties/value"",
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
}}";
    }
}
