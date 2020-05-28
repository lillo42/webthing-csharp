using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Action.String
{
    public class DateTimeOffsetActionTest : AbstractStructActionTest<DateTimeOffset>
    {
        protected override JsonElement CreateJson(DateTimeOffset value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}")
                .GetProperty("action");
        }

        protected override IEnumerable<JsonElement> CreateInvalidJson()
        {
            yield return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{Fixture.Create<string>()}"" }} }} }}")
                .GetProperty("action");
            
            yield return JsonSerializer
                .Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<bool>().ToString().ToLower()} }} }} }}")
                .GetProperty("action");
            
            yield return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<int>()} }} }} }}")
                .GetProperty("action");
        }


        #region Valid
        
        [Theory]
        [InlineData("2013-01-21T00:00:00+01:00")]
        [InlineData("2014-01-21T00:00:00+01:00")]
        [InlineData("2015-01-21T00:00:00+01:00")]
        public async Task Enum_Should_Execute_When_ParameterIsValid(string dateTimeString)
        {
            var value = DateTimeOffset.Parse(dateTimeString);
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeOffsetActionThing.Enum)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Fact]
        public async Task NonNullableValue_Should_Execute_When_ParameterIsValid()
        {
            var value = CreateValue();
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.NonNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeOffsetActionThing.NonNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("2013-01-21T00:00:00+01:00")]
        [InlineData("2013-02-21T00:00:00+01:00")]
        [InlineData("2013-03-21T00:00:00+01:00")]
        public async Task NotAcceptedNullableValue_Should_Execute_When_ParameterIsValid(string dateTimeString)
        {
            var value = DateTimeOffset.Parse(dateTimeString);
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.NotAcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeOffsetActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("2013-01-21T00:00:00+02:00")]
        [InlineData("2013-01-21T00:00:00+03:00")]
        [InlineData("2013-01-21T00:00:00+04:00")]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsValid(string dateTimeString)
        {
            var value = DateTimeOffset.Parse(dateTimeString);
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeOffsetActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }

        [Fact]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsNull()
        {
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(DateTimeOffsetActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(null);
        }

        #endregion

        #region Invalid

        [Theory]
        [InlineData("2013-01-21T00:00:00+02:00")]
        [InlineData("2013-02-21T00:00:00+01:00")]
        [InlineData("2013-03-21T00:00:00+01:00")]
        public void Enum_Should_ReturnError_When_ParameterIsInvalid(string dateTimeString)
        {
            var value = DateTimeOffset.Parse(dateTimeString);
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeOffsetActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NonNullableValue_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.NonNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeOffsetActionThing.NonNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("2013-01-21T00:00:00+02:00")]
        [InlineData("2013-01-21T00:00:00+03:00")]
        [InlineData("2013-01-21T00:00:00+04:00")]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(string dateTimeString)
        {
            var value = DateTimeOffset.Parse(dateTimeString);
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeOffsetActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.NotAcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeOffsetActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("2013-01-21T00:00:00+01:00")]
        [InlineData("2014-01-21T00:00:00+01:00")]
        [InlineData("2015-01-21T00:00:00+01:00")]
        public void AcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(string dateTimeString)
        {
            var value = DateTimeOffset.Parse(dateTimeString);
            var thing = new DateTimeOffsetActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeOffsetActionThing.AcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeOffsetActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
        #endregion
        
        public class DateTimeOffsetActionThing : Thing
        {
            public override string Name => "date-time-action-thing";
            
            [ThingProperty(Ignore = true)]
            public DateTimeOffset? Value { get; private set; }
            
            public void Enum([ThingParameter(Enum = new object[] { "2013-01-21T00:00:00+01:00", "2014-01-21T00:00:00+01:00", "2015-01-21T00:00:00+01:00" })]DateTimeOffset value)
            {
                Value = value;
            }
            
            public void NonNullableValue([ThingParameter(IsNullable = false)]DateTimeOffset? value)
            {
                Value = value;
            }
            
            public void NotAcceptedNullableValue([ThingParameter(Enum = new object[] { "2013-01-21T00:00:00+01:00", "2013-02-21T00:00:00+01:00", "2013-03-21T00:00:00+01:00" })]DateTimeOffset? value)
            {
                Value = value;
            }
            
            public void AcceptedNullableValue([ThingParameter(Enum = new object[] { null, "2013-01-21T00:00:00+02:00", "2013-01-21T00:00:00+03:00", "2013-01-21T00:00:00+04:00" })]DateTimeOffset? value)
            {
                Value = value;
            }
        }
    }
}
