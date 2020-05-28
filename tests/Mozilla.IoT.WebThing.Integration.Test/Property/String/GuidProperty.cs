using AutoFixture;
using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.String
{
    public class GuidProperty : AbstractStructPropertyTest<Guid>
    {
        protected override JsonElement CreateJson(Guid value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<TimeSpan>()}"" }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
        
        
        [Theory]
        [InlineData(nameof(GuidThing.Value), "be5cdf5b-4c72-4db9-8646-d20ffb94caec")]
        [InlineData(nameof(GuidThing.Value), "fb4d04e2-8e5a-4202-b0c5-d5fb5e68cfe2")]
        [InlineData(nameof(GuidThing.Value), "035a29bb-bccd-4816-a4fd-3bdfd1e32acd")]
        [InlineData(nameof(GuidThing.NotAcceptedNullableValue), "7539e439-fa4b-4f1f-8a75-7712a483ffa1")]
        [InlineData(nameof(GuidThing.NotAcceptedNullableValue), "7526477b-d125-4a93-ab3d-5760bbd2d2f7")]
        [InlineData(nameof(GuidThing.NotAcceptedNullableValue), "4af37617-7254-40df-8231-04055cf1c3a3")]
        [InlineData(nameof(GuidThing.AcceptedNullableValue), "34167747-61c6-4580-8e91-116071918690")]
        [InlineData(nameof(GuidThing.AcceptedNullableValue), "b26958c9-da73-4987-b7cf-5a98f58acfe2")]
        [InlineData(nameof(GuidThing.AcceptedNullableValue), "0f530eca-d670-4495-b4c0-d5b37acf56ef")]
        [InlineData(nameof(GuidThing.AcceptedNullableValue), null)]
        [InlineData(nameof(GuidThing.NonNullableValue), "cce26bdf-25f8-479d-b2e0-b5a3f19d4f37")]
        [InlineData(nameof(GuidThing.NonNullableValue), "a4bb2581-700d-4033-9339-1f4ee48e1632")]
        [InlineData(nameof(GuidThing.NonNullableValue), "eab8f293-2c2d-4a73-bc2f-fcd8032967ab")]
        public void ValidPropertyWithEnum(string propertyName, string value)
        {
            var jsonValue = value != null ? $@" ""{value}""" : "null";

            var thing = new GuidThing();
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
            getValue.Should().Be(value != null ? (object)Guid.Parse(value) : null);
        }


        [Theory]
        [InlineData(nameof(GuidThing.Value), "7539e439-fa4b-4f1f-8a75-7712a483ffa1")]
        [InlineData(nameof(GuidThing.Value), "7526477b-d125-4a93-ab3d-5760bbd2d2f7")]
        [InlineData(nameof(GuidThing.Value), "4af37617-7254-40df-8231-04055cf1c3a3")]
        [InlineData(nameof(GuidThing.NotAcceptedNullableValue), null)]
        [InlineData(nameof(GuidThing.NotAcceptedNullableValue), "34167747-61c6-4580-8e91-116071918690")]
        [InlineData(nameof(GuidThing.NotAcceptedNullableValue), "b26958c9-da73-4987-b7cf-5a98f58acfe2")]
        [InlineData(nameof(GuidThing.NotAcceptedNullableValue), "0f530eca-d670-4495-b4c0-d5b37acf56ef")]
        [InlineData(nameof(GuidThing.AcceptedNullableValue), "cce26bdf-25f8-479d-b2e0-b5a3f19d4f37")]
        [InlineData(nameof(GuidThing.AcceptedNullableValue), "a4bb2581-700d-4033-9339-1f4ee48e1632")]
        [InlineData(nameof(GuidThing.AcceptedNullableValue), "eab8f293-2c2d-4a73-bc2f-fcd8032967ab")]
        [InlineData(nameof(GuidThing.NonNullableValue), null)]
        public void InvalidPropertyValue(string propertyName, string value)
        {
            var jsonValue = value != null ? $@"""{value}""" : "null";

            var thing = new GuidThing();
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
            getValue.Should().NotBe(value != null ? (object)Guid.Parse(value) : null);
        }

        [Fact]
        public void SerializeGuidThing()
        {
            TestResponseProperty<GuidThing>(ExpectedSerialize);
        }
        
        public class GuidThing : Thing
        {
            public override string Name => "guid-property";

            [ThingProperty(Enum = new object[] { "be5cdf5b-4c72-4db9-8646-d20ffb94caec", "fb4d04e2-8e5a-4202-b0c5-d5fb5e68cfe2", "035a29bb-bccd-4816-a4fd-3bdfd1e32acd" })]
            public Guid Value { get; set; }

            [ThingProperty(IsNullable = false)] 
            public Guid? NonNullableValue { get; set; } = Guid.NewGuid();

            [ThingProperty(Enum = new object[] {"7539e439-fa4b-4f1f-8a75-7712a483ffa1", "7526477b-d125-4a93-ab3d-5760bbd2d2f7", "4af37617-7254-40df-8231-04055cf1c3a3"})]
            public Guid? NotAcceptedNullableValue { get; set; } = Guid.NewGuid();
            
            [ThingProperty(Enum = new object[] { null, "34167747-61c6-4580-8e91-116071918690", "b26958c9-da73-4987-b7cf-5a98f58acfe2", "0f530eca-d670-4495-b4c0-d5b37acf56ef" })]
            public Guid? AcceptedNullableValue { get; set; }
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
        ""value"": {
            ""type"": ""string"",
            ""enum"": [
                ""be5cdf5b-4c72-4db9-8646-d20ffb94caec"",
                ""fb4d04e2-8e5a-4202-b0c5-d5fb5e68cfe2"",
                ""035a29bb-bccd-4816-a4fd-3bdfd1e32acd""
            ],
            ""links"": [
                {
                    ""href"": ""/things/guid-property/properties/value"",
                    ""rel"": ""property""
                }
            ]
        },
        ""nonNullableValue"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/guid-property/properties/nonNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""notAcceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                ""7539e439-fa4b-4f1f-8a75-7712a483ffa1"",
                ""7526477b-d125-4a93-ab3d-5760bbd2d2f7"",
                ""4af37617-7254-40df-8231-04055cf1c3a3""
            ],
            ""links"": [
                {
                    ""href"": ""/things/guid-property/properties/notAcceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""acceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                null,
                ""34167747-61c6-4580-8e91-116071918690"",
                ""b26958c9-da73-4987-b7cf-5a98f58acfe2"",
                ""0f530eca-d670-4495-b4c0-d5b37acf56ef""
            ],
            ""links"": [
                {
                    ""href"": ""/things/guid-property/properties/acceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        }
    },
    ""links"": [
        {
            ""rel"": ""properties"",
            ""href"": ""/things/guid-property/properties""
        },
        {
            ""rel"": ""actions"",
            ""href"": ""/things/guid-property/actions""
        },
        {
            ""rel"": ""events"",
            ""href"": ""/things/guid-property/events""
        }
    ]
}
";
    }
}
