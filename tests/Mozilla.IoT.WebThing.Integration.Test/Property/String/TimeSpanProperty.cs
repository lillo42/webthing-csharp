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
    public class TimeSpanProperty : AbstractStructPropertyTest<TimeSpan>
    {
        protected override JsonElement CreateJson(TimeSpan value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}")
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
        [InlineData(nameof(TimeSpanThing.Value), "00:00:10")]
        [InlineData(nameof(TimeSpanThing.Value), "00:00:20")]
        [InlineData(nameof(TimeSpanThing.Value), "00:00:50")]
        [InlineData(nameof(TimeSpanThing.NotAcceptedNullableValue), "00:01:00")]
        [InlineData(nameof(TimeSpanThing.NotAcceptedNullableValue), "00:10:00")]
        [InlineData(nameof(TimeSpanThing.NotAcceptedNullableValue), "00:20:00")]
        [InlineData(nameof(TimeSpanThing.AcceptedNullableValue),  "01:00:00")]
        [InlineData(nameof(TimeSpanThing.AcceptedNullableValue),  "10:00:00")]
        [InlineData(nameof(TimeSpanThing.AcceptedNullableValue),  "20:00:00")]
        [InlineData(nameof(TimeSpanThing.AcceptedNullableValue), null)]
        [InlineData(nameof(TimeSpanThing.NonNullableValue), "10:20:30")]
        [InlineData(nameof(TimeSpanThing.NonNullableValue), "00:35:00")]
        [InlineData(nameof(TimeSpanThing.NonNullableValue), "01:30:45")]
        public void ValidPropertyWithEnum(string propertyName, string value)
        {
            var jsonValue = value != null ? $@" ""{value}""" : "null";

            var thing = new TimeSpanThing();
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
            getValue.Should().Be(value != null ? (object)TimeSpan.Parse(value) : null);
        }


        [Theory]
        [InlineData(nameof(TimeSpanThing.Value), "00:01:00")]
        [InlineData(nameof(TimeSpanThing.Value), "00:10:00")]
        [InlineData(nameof(TimeSpanThing.Value), "00:20:00")]
        [InlineData(nameof(TimeSpanThing.NotAcceptedNullableValue), null)]
        [InlineData(nameof(TimeSpanThing.NotAcceptedNullableValue), "01:00:00")]
        [InlineData(nameof(TimeSpanThing.NotAcceptedNullableValue), "10:00:00")]
        [InlineData(nameof(TimeSpanThing.NotAcceptedNullableValue), "20:00:00")]
        [InlineData(nameof(TimeSpanThing.AcceptedNullableValue), "10:20:30")]
        [InlineData(nameof(TimeSpanThing.AcceptedNullableValue), "00:35:00")]
        [InlineData(nameof(TimeSpanThing.AcceptedNullableValue), "01:30:45")]
        [InlineData(nameof(TimeSpanThing.NonNullableValue), null)]
        public void InvalidPropertyValue(string propertyName, string value)
        {
            var jsonValue = value != null ? $@"""{value}""" : "null";

            var thing = new TimeSpanThing();
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
            getValue.Should().NotBe(value != null ? (object)TimeSpan.Parse(value) : null);
        }

        [Fact]
        public void SerializeTimeSpanThing()
        {
            TestResponseProperty<TimeSpanThing>(ExpectedSerialize);
        }
        
        public class TimeSpanThing : Thing
        {
            public override string Name => "time-span-property";

            [ThingProperty(Enum = new object[] { "00:00:10", "00:00:20", "00:00:50" })]
            public TimeSpan Value { get; set; }

            [ThingProperty(IsNullable = false)] 
            public TimeSpan? NonNullableValue { get; set; } = TimeSpan.Zero;

            [ThingProperty(Enum = new object[] {"00:01:00", "00:10:00", "00:20:00"})]
            public TimeSpan? NotAcceptedNullableValue { get; set; } = TimeSpan.Zero;
            
            [ThingProperty(Enum = new object[] { null, "01:00:00", "10:00:00", "20:00:00" })]
            public TimeSpan? AcceptedNullableValue { get; set; }
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
                ""00:00:10"",
                ""00:00:20"",
                ""00:00:50""
            ],
            ""links"": [
                {
                    ""href"": ""/things/time-span-property/properties/value"",
                    ""rel"": ""property""
                }
            ]
        },
        ""nonNullableValue"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/time-span-property/properties/nonNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""notAcceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                ""00:01:00"",
                ""00:10:00"",
                ""00:20:00""
            ],
            ""links"": [
                {
                    ""href"": ""/things/time-span-property/properties/notAcceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""acceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                null,
                ""01:00:00"",
                ""10:00:00"",
                ""20:00:00""
            ],
            ""links"": [
                {
                    ""href"": ""/things/time-span-property/properties/acceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        }
    },
    ""links"": [
        {
            ""rel"": ""properties"",
            ""href"": ""/things/time-span-property/properties""
        },
        {
            ""rel"": ""actions"",
            ""href"": ""/things/time-span-property/actions""
        },
        {
            ""rel"": ""events"",
            ""href"": ""/things/time-span-property/events""
        }
    ]
}
";
    }
}
