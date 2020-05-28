using System.Collections.Generic;
using AutoFixture;
using System.Text.Json;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.String
{
    public class CharProperty : AbstractStructPropertyTest<char>
    {
        protected override char CreateValue()
        {
            return Fixture.Create<string>()[0];
        }

        protected override JsonElement CreateJson(char value)
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
                JsonSerializer
                    .Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                    .GetProperty("input")
            };

            return result.ToArray();
        }

        [Theory]
        [InlineData(nameof(CharThing.Value), 'A')]
        [InlineData(nameof(CharThing.Value), 'B')]
        [InlineData(nameof(CharThing.Value), 'C')]
        [InlineData(nameof(CharThing.NotAcceptedNullableValue), 'D')]
        [InlineData(nameof(CharThing.NotAcceptedNullableValue), 'E')]
        [InlineData(nameof(CharThing.NotAcceptedNullableValue), 'F')]
        [InlineData(nameof(CharThing.AcceptedNullableValue), 'G')]
        [InlineData(nameof(CharThing.AcceptedNullableValue), 'H')]
        [InlineData(nameof(CharThing.AcceptedNullableValue), 'I')]
        [InlineData(nameof(CharThing.AcceptedNullableValue), null)]
        [InlineData(nameof(CharThing.NonNullableValue), 'J')]
        [InlineData(nameof(CharThing.NonNullableValue), 'K')]
        [InlineData(nameof(CharThing.NonNullableValue), 'L')]
        public void ValidPropertyWithEnum(string propertyName, char? value)
        {
            var jsonValue = value.HasValue ? $@" ""{value.Value}""" : "null";

            var thing = new CharThing();
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
        [InlineData(nameof(CharThing.Value), 'D')]
        [InlineData(nameof(CharThing.Value), 'E')]
        [InlineData(nameof(CharThing.Value), 'F')]
        [InlineData(nameof(CharThing.NotAcceptedNullableValue), null)]
        [InlineData(nameof(CharThing.NotAcceptedNullableValue), 'G')]
        [InlineData(nameof(CharThing.NotAcceptedNullableValue), 'H')]
        [InlineData(nameof(CharThing.NotAcceptedNullableValue), 'I')]
        [InlineData(nameof(CharThing.AcceptedNullableValue), 'J')]
        [InlineData(nameof(CharThing.AcceptedNullableValue), 'K')]
        [InlineData(nameof(CharThing.AcceptedNullableValue), 'L')]
        [InlineData(nameof(CharThing.NonNullableValue), null)]
        public void InvalidPropertyValue(string propertyName, char? value)
        {
            var jsonValue = value.HasValue ? $@"""{value.Value}""" : "null";

            var thing = new CharThing();
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
        public void SerializeCharThing()
        {
            TestResponseProperty<CharThing>(Expectederialize);
        }

        public class CharThing : Thing
        {
            public override string Name => "char-property";

            [ThingProperty(Enum = new object[] { 'A', 'B', 'C' })]
            public char Value { get; set; }

            [ThingProperty(IsNullable = false)] 
            public char? NonNullableValue { get; set; } = 'T';

            [ThingProperty(Enum = new object[] {'D', 'E', 'F'})]
            public char? NotAcceptedNullableValue { get; set; } = 'P';
            
            [ThingProperty(Enum = new object[] { null, 'G', 'H', 'I' })]
            public char? AcceptedNullableValue { get; set; }
        }
        
        private const string Expectederialize = @"
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
                ""A"",
                ""B"",
                ""C""
            ],
            ""links"": [
                {
                    ""href"": ""/things/char-property/properties/value"",
                    ""rel"": ""property""
                }
            ]
        },
        ""nonNullableValue"": {
            ""type"": ""string"",
            ""links"": [
                {
                    ""href"": ""/things/char-property/properties/nonNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""notAcceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                ""D"",
                ""E"",
                ""F""
            ],
            ""links"": [
                {
                    ""href"": ""/things/char-property/properties/notAcceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        },
        ""acceptedNullableValue"": {
            ""type"": ""string"",
            ""enum"": [
                null,
                ""G"",
                ""H"",
                ""I""
            ],
            ""links"": [
                {
                    ""href"": ""/things/char-property/properties/acceptedNullableValue"",
                    ""rel"": ""property""
                }
            ]
        }
    },
    ""links"": [
        {
            ""rel"": ""properties"",
            ""href"": ""/things/char-property/properties""
        },
        {
            ""rel"": ""actions"",
            ""href"": ""/things/char-property/actions""
        },
        {
            ""rel"": ""events"",
            ""href"": ""/things/char-property/events""
        }
    ]
}
";
    }
}
