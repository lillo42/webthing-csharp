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
    public class DateTimeProperty : AbstractStructPropertyTest<DateTime>
    {
        protected override JsonElement CreateJson(DateTime value)
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
        [InlineData(nameof(DateTimeThing.Value), "2013-01-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.Value), "2014-01-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.Value), "2015-01-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.NotAcceptedNullableValue), "2013-02-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.NotAcceptedNullableValue), "2013-03-21T00:00:00Z")]
        //[InlineData(nameof(DateTimeThing.NotAcceptedNullableValue), "2013-04-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.AcceptedNullableValue), "2013-02-22T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.AcceptedNullableValue), "2013-02-23T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.AcceptedNullableValue), "2013-02-24T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.AcceptedNullableValue), null)]
        [InlineData(nameof(DateTimeThing.NonNullableValue), "2013-02-24T01:00:00Z")]
        [InlineData(nameof(DateTimeThing.NonNullableValue), "2013-02-24T02:00:00Z")]
        [InlineData(nameof(DateTimeThing.NonNullableValue), "2013-02-24T03:00:00Z")]
        public void ValidPropertyWithEnum(string propertyName, string value)
        {
            var jsonValue = value != null ? $@" ""{value}""" : "null";

            var thing = new DateTimeThing();
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
            getValue.Should().Be(value != null ? (object)DateTime.Parse(value) : null);
        }


        [Theory]
        [InlineData(nameof(DateTimeThing.Value), "2013-02-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.Value), "2013-03-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.Value), "2013-04-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.NotAcceptedNullableValue), null)]
        [InlineData(nameof(DateTimeThing.NotAcceptedNullableValue), "2013-02-22T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.NotAcceptedNullableValue), "2013-02-23T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.NotAcceptedNullableValue), "2013-02-24T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.AcceptedNullableValue), "2013-01-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.AcceptedNullableValue), "2014-01-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.AcceptedNullableValue), "2015-01-21T00:00:00Z")]
        [InlineData(nameof(DateTimeThing.NonNullableValue), null)]
        public void InvalidPropertyValue(string propertyName, string value)
        {
            var jsonValue = value != null ? $@"""{value}""" : "null";

            var thing = new DateTimeThing();
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
            getValue.Should().NotBe(value != null ? (object)DateTime.Parse(value) : null);
        }

        [Fact]
        public void SerializeDateTimeThing()
        {
            TestResponseProperty<DateTimeThing>(ExpectedSerialize);
        }
        
        public class DateTimeThing : Thing
        {
            public override string Name => "date-time-offset-property";

            [ThingProperty(Enum = new object[] { "2013-01-21T00:00:00Z", "2014-01-21T00:00:00Z", "2015-01-21T00:00:00Z" })]
            public DateTime Value { get; set; }

            [ThingProperty(IsNullable = false)] 
            public DateTime? NonNullableValue { get; set; } = DateTime.Now;

            [ThingProperty(Enum = new object[] {"2013-02-21T00:00:00Z", "2013-03-21T00:00:00Z", "2013-04-21T00:00:00Z"})]
            public DateTime? NotAcceptedNullableValue { get; set; } = DateTime.Now;
            
            [ThingProperty(Enum = new object[] { null, "2013-02-22T00:00:00Z", "2013-02-23T00:00:00Z", "2013-02-24T00:00:00Z" })]
            public DateTime? AcceptedNullableValue { get; set; }
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
                ""2013-01-21T00:00:00Z"",
                ""2014-01-21T00:00:00Z"",
                ""2015-01-21T00:00:00Z""
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
                ""2013-02-21T00:00:00Z"",
                ""2013-03-21T00:00:00Z"",
                ""2013-04-21T00:00:00Z""
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
                ""2013-02-22T00:00:00Z"",
                ""2013-02-23T00:00:00Z"",
                ""2013-02-24T00:00:00Z""
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
