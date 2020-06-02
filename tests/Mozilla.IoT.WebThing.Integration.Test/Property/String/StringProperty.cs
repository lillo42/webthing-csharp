using System.Collections.Generic;
using AutoFixture;
using System.Text.Json;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.String
{
    public class StringProperty : AbstractClassPropertyTest<string>
    {
        protected override JsonElement CreateJson(string value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
        
        [Theory]
        [InlineData(nameof(StringThing.MinAndMax), "1")]
        [InlineData(nameof(StringThing.MinAndMax), "1234567890")]
        [InlineData(nameof(StringThing.Mail), "test@test.com")]
        [InlineData(nameof(StringThing.Enum), "Lorem")]
        [InlineData(nameof(StringThing.Enum), "ipsum etiam")]
        [InlineData(nameof(StringThing.Enum), "nec litora placerat")]
        [InlineData(nameof(StringThing.NotAcceptedNullableEnumValue), "viverra sem tempor vitae")]
        [InlineData(nameof(StringThing.NotAcceptedNullableEnumValue), "donec nisi,")]
        [InlineData(nameof(StringThing.NotAcceptedNullableEnumValue), "suspendisse")]
        [InlineData(nameof(StringThing.AcceptedNullableEnumValue), "elit velit")]
        [InlineData(nameof(StringThing.AcceptedNullableEnumValue), "erat eleifend integer enim")]
        [InlineData(nameof(StringThing.AcceptedNullableEnumValue), "porta praesent dictum")]
        [InlineData(nameof(StringThing.AcceptedNullableEnumValue), null)]
        [InlineData(nameof(StringThing.NonNullableEnumValue), "auctor laoreet ut ornare")]
        [InlineData(nameof(StringThing.NonNullableEnumValue), "ad justo")]
        [InlineData(nameof(StringThing.NonNullableEnumValue), "cras curabitur integer")]
        public void ValidPropertyValue(string propertyName, string value)
        {
            var jsonValue = value != null ? $@" ""{value}""" : "null";

            var thing = new StringThing();
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
            getValue.Should().Be(value);
        }
        
        [Theory]
        [InlineData(nameof(StringThing.MinAndMax), "")]
        [InlineData(nameof(StringThing.MinAndMax), "12345678901")]
        [InlineData(nameof(StringThing.Mail), "test")]
        [InlineData(nameof(StringThing.Mail), "test@")]
        [InlineData(nameof(StringThing.Mail), "test@test")]
        [InlineData(nameof(StringThing.Mail), "")]
        [InlineData(nameof(StringThing.Enum), "viverra sem tempor vitae")]
        [InlineData(nameof(StringThing.Enum), "donec nisi,")]
        [InlineData(nameof(StringThing.Enum), "suspendisse")]
        [InlineData(nameof(StringThing.NotAcceptedNullableEnumValue), null)]
        [InlineData(nameof(StringThing.NotAcceptedNullableEnumValue), "elit velit")]
        [InlineData(nameof(StringThing.NotAcceptedNullableEnumValue), "erat eleifend integer enim")]
        [InlineData(nameof(StringThing.NotAcceptedNullableEnumValue), "porta praesent dictum")]
        [InlineData(nameof(StringThing.AcceptedNullableEnumValue), "Lorem")]
        [InlineData(nameof(StringThing.AcceptedNullableEnumValue), "ipsum etiam")]
        [InlineData(nameof(StringThing.AcceptedNullableEnumValue),  "nec litora placerat")]
        [InlineData(nameof(StringThing.NonNullableEnumValue), null)]
        public void InvalidPropertyValue(string propertyName, string value)
        {
            var jsonValue = value != null ? $@"""{value}""" : "null";

            var thing = new StringThing();
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
        public void SerializeGuidThing()
        {
            TestResponseProperty<StringThing>(ExpectedSerialize);
        }
        
        public class StringThing : Thing
        {
            public override string Name => "sting-property";
            
            [ThingProperty(MinimumLength = 1, MaximumLength = 10)]
            public string MinAndMax { get; set; }
            
            [ThingProperty(Pattern = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$")]
            public string Mail { get; set; }

            [ThingProperty(Enum = new object[] { "Lorem", "ipsum etiam", "nec litora placerat" })]
            public string Enum { get; set; }

            [ThingProperty(IsNullable = false)] 
            public string NonNullableEnumValue { get; set; } = string.Empty;

            [ThingProperty(Enum = new object[] {"viverra sem tempor vitae", "donec nisi,", "suspendisse"})]
            public string NotAcceptedNullableEnumValue { get; set; } = string.Empty;
            
            [ThingProperty(Enum = new object[] { null, "elit velit", "erat eleifend integer enim", "porta praesent dictum" })]
            public string AcceptedNullableEnumValue { get; set; }
        }

        private const string ExpectedSerialize = @"
{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {
        ""nosec_sc"": {
            ""scheme"": ""nosec""
        }
    },
    ""properties"": {
        ""minAndMax"": {
            ""type"": ""string"",
            ""minLength"": 1,
            ""maxLength"": 10,
            ""links"": [
                {
                    ""href"": ""/things/sting-property/properties/minAndMax"",
                    ""rel"": ""property""
                }
            ]
        },
        ""mail"": {
            ""type"": ""string"",
            ""pattern"": ""^\\w\u002B@[a-zA-Z_]\u002B?\\.[a-zA-Z]{2,3}$"",
            ""links"": [
                {
                    ""href"": ""/things/sting-property/properties/mail"",
                    ""rel"": ""property""
                }
            ]
        },
        ""enum"": {
            ""type"": ""string"",
            ""enum"": [
                ""Lorem"",
                ""ipsum etiam"",
                ""nec litora placerat""
            ],
            ""links"": [
                {
                    ""href"": ""/things/sting-property/properties/enum"",
                    ""rel"": ""property""
                }
            ]
        },
        ""nonNullableEnumValue"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/sting-property/properties/nonNullableEnumValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""notAcceptedNullableEnumValue"": {
            ""type"": ""string"",
            ""enum"": [
                ""viverra sem tempor vitae"",
                ""donec nisi,"",
                ""suspendisse""
            ],
            ""links"": [
                {
                    ""href"": ""/things/sting-property/properties/notAcceptedNullableEnumValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""acceptedNullableEnumValue"": {
            ""type"": ""string"",
            ""enum"": [
                null,
                ""elit velit"",
                ""erat eleifend integer enim"",
                ""porta praesent dictum""
            ],
            ""links"": [
                {
                    ""href"": ""/things/sting-property/properties/acceptedNullableEnumValue"",
                    ""rel"": ""property""
                }
            ]
        }
    },
    ""links"": [
        {
            ""rel"": ""properties"",
            ""href"": ""/things/sting-property/properties""
        },
        {
            ""rel"": ""actions"",
            ""href"": ""/things/sting-property/actions""
        },
        {
            ""rel"": ""events"",
            ""href"": ""/things/sting-property/events""
        }
    ]
}
";
    }
}
