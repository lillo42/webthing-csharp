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
    public class DateTimeOffsetProperty : AbstractStructPropertyTest<DateTimeOffset>
    {
        protected override JsonElement CreateJson(DateTimeOffset value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value:O}"" }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
        
        [Theory]
        [InlineData(nameof(DateTimeOffsetThing.Value), "2013-01-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.Value), "2014-01-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.Value), "2015-01-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.NotAcceptedNullableValue), "2013-01-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.NotAcceptedNullableValue), "2013-02-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.NotAcceptedNullableValue), "2013-03-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.AcceptedNullableValue), "2013-01-21T00:00:00+02:00")]
        [InlineData(nameof(DateTimeOffsetThing.AcceptedNullableValue), "2013-01-21T00:00:00+03:00")]
        [InlineData(nameof(DateTimeOffsetThing.AcceptedNullableValue), "2013-01-21T00:00:00+04:00")]
        [InlineData(nameof(DateTimeOffsetThing.AcceptedNullableValue), null)]
        [InlineData(nameof(DateTimeOffsetThing.NonNullableValue), "2013-01-22T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.NonNullableValue), "2013-01-23T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.NonNullableValue), "2013-01-24T00:00:00+01:00")]
        public void ValidPropertyWithEnum(string propertyName, string value)
        {
            var jsonValue = value != null ? $@" ""{value}""" : "null";

            var thing = new DateTimeOffsetThing();
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
            getValue.Should().Be(value != null ? (object)DateTimeOffset.Parse(value) : null);
        }


        [Theory]
        [InlineData(nameof(DateTimeOffsetThing.Value), "2013-01-21T00:00:01+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.Value), "2013-02-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.Value), "2013-03-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.NotAcceptedNullableValue), null)]
        [InlineData(nameof(DateTimeOffsetThing.NotAcceptedNullableValue), "2013-01-21T00:00:00+02:00")]
        [InlineData(nameof(DateTimeOffsetThing.NotAcceptedNullableValue), "2013-01-21T00:00:00+03:00")]
        [InlineData(nameof(DateTimeOffsetThing.NotAcceptedNullableValue), "2013-01-21T00:00:00+04:00")]
        [InlineData(nameof(DateTimeOffsetThing.AcceptedNullableValue), "2013-01-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.AcceptedNullableValue), "2014-01-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.AcceptedNullableValue), "2015-01-21T00:00:00+01:00")]
        [InlineData(nameof(DateTimeOffsetThing.NonNullableValue), null)]
        public void InvalidPropertyValue(string propertyName, string value)
        {
            var jsonValue = value != null ? $@"""{value}""" : "null";

            var thing = new DateTimeOffsetThing();
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
            getValue.Should().NotBe(value != null ? (object)DateTimeOffset.Parse(value) : null);
        }

        [Fact]
        public void SerializeDateTimeOffsetThing()
        {
            TestResponseProperty<DateTimeOffsetThing>(ExpectedSerialize);
        }
        
        public class DateTimeOffsetThing : Thing
        {
            public override string Name => "date-time-offset-property";

            [ThingProperty(Enum = new object[] { "2013-01-21T00:00:00+01:00", "2014-01-21T00:00:00+01:00", "2015-01-21T00:00:00+01:00" })]
            public DateTimeOffset Value { get; set; }

            [ThingProperty(IsNullable = false)] 
            public DateTimeOffset? NonNullableValue { get; set; } = DateTimeOffset.Now;

            [ThingProperty(Enum = new object[] {"2013-01-21T00:00:00+01:00", "2013-02-21T00:00:00+01:00", "2013-03-21T00:00:00+01:00"})]
            public DateTimeOffset? NotAcceptedNullableValue { get; set; } = DateTimeOffset.Now;
            
            [ThingProperty(Enum = new object[] { null, "2013-01-21T00:00:00+02:00", "2013-01-21T00:00:00+03:00", "2013-01-21T00:00:00+04:00" })]
            public DateTimeOffset? AcceptedNullableValue { get; set; }
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
                ""2013-01-21T00:00:00\u002B01:00"",
                ""2014-01-21T00:00:00\u002B01:00"",
                ""2015-01-21T00:00:00\u002B01:00""
            ],
            ""links"": [
                {
                    ""href"": ""/things/date-time-offset-property/properties/value"",
                    ""rel"": ""property""
                }
            ]
        },
        ""nonNullableValue"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/date-time-offset-property/properties/nonNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""notAcceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                ""2013-01-21T00:00:00\u002B01:00"",
                ""2013-02-21T00:00:00\u002B01:00"",
                ""2013-03-21T00:00:00\u002B01:00""
            ],
            ""links"": [
                {
                    ""href"": ""/things/date-time-offset-property/properties/notAcceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""acceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                null,
                ""2013-01-21T00:00:00\u002B02:00"",
                ""2013-01-21T00:00:00\u002B03:00"",
                ""2013-01-21T00:00:00\u002B04:00""
            ],
            ""links"": [
                {
                    ""href"": ""/things/date-time-offset-property/properties/acceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        }
    },
    ""links"": [
        {
            ""rel"": ""properties"",
            ""href"": ""/things/date-time-offset-property/properties""
        },
        {
            ""rel"": ""actions"",
            ""href"": ""/things/date-time-offset-property/actions""
        },
        {
            ""rel"": ""events"",
            ""href"": ""/things/date-time-offset-property/events""
        }
    ]
}
";
    }
}
