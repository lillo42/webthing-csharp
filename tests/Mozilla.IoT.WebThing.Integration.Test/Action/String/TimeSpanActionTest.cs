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
    public class TimeSpanActionTest : AbstractStructActionTest<TimeSpan>
    {
        protected override JsonElement CreateJson(TimeSpan value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}")
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
        [InlineData("00:00:10")]
        [InlineData("00:00:20")]
        [InlineData("00:00:50")]
        public async Task Enum_Should_Execute_When_ParameterIsValid(string timeSpanString)
        {
            var value = TimeSpan.Parse(timeSpanString);   
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(TimeSpanActionThing.Enum)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
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
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.NonNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(TimeSpanActionThing.NonNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("00:01:00")]
        [InlineData("00:10:00")]
        [InlineData("00:20:00")]
        public async Task NotAcceptedNullableValue_Should_Execute_When_ParameterIsValid(string timeSpanString)
        {
            var value = TimeSpan.Parse(timeSpanString);   
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.NotAcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(TimeSpanActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("01:00:00")]
        [InlineData("10:00:00")]
        [InlineData("20:00:00")]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsValid(string timeSpanString)
        {
            var value = TimeSpan.Parse(timeSpanString);
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(TimeSpanActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }

        [Fact]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsNull()
        {
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(TimeSpanActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(null);
        }

        #endregion

        #region Invalid

        [Theory]
        [InlineData("00:01:00")]
        [InlineData("00:10:00")]
        [InlineData("00:20:00")]
        public void Enum_Should_ReturnError_When_ParameterIsInvalid(string timeSpanString)
        {
            var value = TimeSpan.Parse(timeSpanString);
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(TimeSpanActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NonNullableValue_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.NonNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(TimeSpanActionThing.NonNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("01:00:00")]
        [InlineData("10:00:00")]
        [InlineData("20:00:00")]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(string timeSpanString)
        {
            var value = TimeSpan.Parse(timeSpanString);
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(TimeSpanActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.NotAcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(TimeSpanActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("00:00:10")]
        [InlineData("00:00:20")]
        [InlineData("00:00:50")]
        public void AcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(string timeSpanString)
        {
            var value = TimeSpan.Parse(timeSpanString);
            var thing = new TimeSpanActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(TimeSpanActionThing.AcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(TimeSpanActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
        #endregion
        
        public class TimeSpanActionThing : Thing
        {
            public override string Name => "guid-action-thing";
            
            [ThingProperty(Ignore = true)]
            public TimeSpan? Value { get; private set; }
            
            public void Enum([ThingParameter(Enum = new object[] { "00:00:10", "00:00:20", "00:00:50" })]TimeSpan value)
            {
                Value = value;
            }
            
            public void NonNullableValue([ThingParameter(IsNullable = false)]TimeSpan? value)
            {
                Value = value;
            }
            
            public void NotAcceptedNullableValue([ThingParameter(Enum = new object[] {"00:01:00", "00:10:00", "00:20:00"})]TimeSpan? value)
            {
                Value = value;
            }
            
            public void AcceptedNullableValue([ThingParameter(Enum = new object[] { null, "01:00:00", "10:00:00", "20:00:00"})]TimeSpan? value)
            {
                Value = value;
            }
        }
    }
}
