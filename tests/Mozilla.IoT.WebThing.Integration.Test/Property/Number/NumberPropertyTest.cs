using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using System.Text.Json;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.Number
{
    public abstract class AbstractNumberPropertyTest<T> : AbstractStructPropertyTest<T>
        where T : struct
    {
        protected override JsonElement CreateJson(T value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
        
        
        [Theory]
        [InlineData(nameof(NumberThing.Value), 1)]
        [InlineData(nameof(NumberThing.Value), 2)]
        [InlineData(nameof(NumberThing.Value), 3)]
        [InlineData(nameof(NumberThing.NotAcceptedNullableValue), 10)]
        [InlineData(nameof(NumberThing.NotAcceptedNullableValue), 20)]
        [InlineData(nameof(NumberThing.NotAcceptedNullableValue), 30)]
        [InlineData(nameof(NumberThing.AcceptedNullableValue), 100)]
        [InlineData(nameof(NumberThing.AcceptedNullableValue), 110)]
        [InlineData(nameof(NumberThing.AcceptedNullableValue), 120)]
        [InlineData(nameof(NumberThing.AcceptedNullableValue), null)]
        [InlineData(nameof(NumberThing.NonNullableValue), 125)]
        [InlineData(nameof(NumberThing.NonNullableValue), 42)]
        [InlineData(nameof(NumberThing.NonNullableValue), 4)]
        [InlineData(nameof(NumberThing.MinAndMaxValue), 10)]
        [InlineData(nameof(NumberThing.MinAndMaxValue), 50)]
        [InlineData(nameof(NumberThing.MinAndMaxValue), 100)]
        [InlineData(nameof(NumberThing.MultiOfValue), 2)]
        [InlineData(nameof(NumberThing.MultiOfValue), 20)]
        [InlineData(nameof(NumberThing.MultiOfValue), 42)]
        [InlineData(nameof(NumberThing.ExclusiveMinAndMaxValue), 11)]
        [InlineData(nameof(NumberThing.ExclusiveMinAndMaxValue), 64)]
        [InlineData(nameof(NumberThing.ExclusiveMinAndMaxValue), 99)]
        public void ValidPropertyValue(string propertyName, int? value)
        {
            var jsonValue = value != null ? $@"{value}" : "null";

            var thing = new NumberThing();
            var context = Factory.Create(thing, new ThingOption());

            thing.ThingContext = context;

            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().ContainKey(propertyName);

            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {jsonValue} }}")
                .GetProperty("input");

            context.Properties[propertyName].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            context.Properties[propertyName].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().BeEquivalentTo(value);
        }


        [Theory]
        [InlineData(nameof(NumberThing.Value), 10)]
        [InlineData(nameof(NumberThing.Value), 20)]
        [InlineData(nameof(NumberThing.Value), 30)]
        [InlineData(nameof(NumberThing.NotAcceptedNullableValue), null)]
        [InlineData(nameof(NumberThing.NotAcceptedNullableValue), 100)]
        [InlineData(nameof(NumberThing.NotAcceptedNullableValue), 110)]
        [InlineData(nameof(NumberThing.NotAcceptedNullableValue), 120)]
        [InlineData(nameof(NumberThing.AcceptedNullableValue), 1)]
        [InlineData(nameof(NumberThing.AcceptedNullableValue), 2)]
        [InlineData(nameof(NumberThing.AcceptedNullableValue), 3)]
        [InlineData(nameof(NumberThing.NonNullableValue), null)]
        [InlineData(nameof(NumberThing.MinAndMaxValue), 9)]
        [InlineData(nameof(NumberThing.MinAndMaxValue), 101)]
        [InlineData(nameof(NumberThing.MultiOfValue), 3)]
        [InlineData(nameof(NumberThing.MultiOfValue), 21)]
        [InlineData(nameof(NumberThing.ExclusiveMinAndMaxValue), 10)]
        [InlineData(nameof(NumberThing.ExclusiveMinAndMaxValue), 100)]
        public void InvalidPropertyValue(string propertyName, int? value)
        {
            var jsonValue = value != null ? $@"{value}" : "null";

            var thing = new NumberThing();
            var context = Factory.Create(thing, new ThingOption());

            thing.ThingContext = context;

            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().ContainKey(propertyName);

            var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {jsonValue} }}")
                .GetProperty("input");

            context.Properties[propertyName].TrySetValue(jsonElement).Should().Be(SetPropertyResult.InvalidValue);
            context.Properties[propertyName].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().NotBe(value);
        }

        [Fact]
        public void SerializeNumberThing()
        { 
            TestResponseProperty<NumberThing>(ExpectedSerialize);
        }

        public class NumberThing : Thing
        {
            public override string Name => "number-property";

            [ThingProperty(Minimum = 10, Maximum = 100)]
            public T MinAndMaxValue { get; set; }
            
            [ThingProperty(ExclusiveMinimum = 10, ExclusiveMaximum = 100)]
            public T ExclusiveMinAndMaxValue { get; set; }
            
            [ThingProperty(MultipleOf = 2)]
            public T MultiOfValue { get; set; }

            [ThingProperty(Enum = new object[] {1, 2, 3 })]
            public T Value { get; set; }

            [ThingProperty(IsNullable = false)] 
            public T? NonNullableValue { get; set; } = default(T);

            [ThingProperty(Enum = new object[] {10, 20, 30})]
            public T? NotAcceptedNullableValue { get; set; } = default(T);
            
            [ThingProperty(Enum = new object[] { null, 100, 110, 120 })]
            public T? AcceptedNullableValue { get; set; }
        }

        private readonly string ExpectedSerialize = $@"
{{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {{
        ""nosec_sc"": {{
            ""scheme"": ""nosec""
        }}
    }},
    ""properties"": {{
        ""minAndMaxValue"": {{
            ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}"",
            ""minimum"": 10,
            ""maximum"": 100,
            ""links"": [
                {{
                    ""href"": ""/things/number-property/properties/minAndMaxValue"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""exclusiveMinAndMaxValue"": {{
            ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}"",
            ""exclusiveMinimum"": 10,
            ""exclusiveMaximum"": 100,
            ""links"": [
                {{
                    ""href"": ""/things/number-property/properties/exclusiveMinAndMaxValue"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""multiOfValue"": {{
            ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}"",
            ""multipleOf"": 2,
            ""links"": [
                {{
                    ""href"": ""/things/number-property/properties/multiOfValue"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""value"": {{
            ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}"",
            ""enum"": [
                1,
                2,
                3
            ],
            ""links"": [
                {{
                    ""href"": ""/things/number-property/properties/value"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""nonNullableValue"": {{
            ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}"",
            ""links"": [
                {{
                    ""href"": ""/things/number-property/properties/nonNullableValue"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""notAcceptedNullableValue"": {{
            ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}"",
            ""enum"": [
                10,
                20,
                30
            ],
            ""links"": [
                {{
                    ""href"": ""/things/number-property/properties/notAcceptedNullableValue"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""acceptedNullableValue"": {{
            ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}"",
            ""enum"": [
                null,
                100,
                110,
                120
            ],
            ""links"": [
                {{
                    ""href"": ""/things/number-property/properties/acceptedNullableValue"",
                    ""rel"": ""property""
                }}
            ]
        }}
    }},
    ""links"": [
        {{
            ""rel"": ""properties"",
            ""href"": ""/things/number-property/properties""
        }},
        {{
            ""rel"": ""actions"",
            ""href"": ""/things/number-property/actions""
        }},
        {{
            ""rel"": ""events"",
            ""href"": ""/things/number-property/events""
        }}
    ]
}}
";
    }
    
    public class ByteProperty : AbstractNumberPropertyTest<byte> { }
    public class SByteProperty : AbstractNumberPropertyTest<sbyte> { }
    
    public class ShortProperty : AbstractNumberPropertyTest<short> { }
    public class UShortProperty : AbstractNumberPropertyTest<ushort> { }
    
    public class IntProperty : AbstractNumberPropertyTest<int> { }
    public class UIntProperty : AbstractNumberPropertyTest<uint> { }
    
    public class LongProperty : AbstractNumberPropertyTest<long> { }
    public class ULongProperty : AbstractNumberPropertyTest<ulong> { }
    
    public class FloatProperty : AbstractNumberPropertyTest<float> { }
    public class DoubleProperty : AbstractNumberPropertyTest<double> { }
    public class DecimalProperty : AbstractNumberPropertyTest<decimal> { }
}
