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
    public class DateTimeActionTest : AbstractStructActionTest<DateTime>
    {
        protected override JsonElement CreateJson(DateTime value)
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
        [InlineData("2013-01-21")]
        [InlineData("2014-01-22")]
        [InlineData("2015-01-23")]
        public async Task Enum_Should_Execute_When_ParameterIsValid(string dateTimeString)
        {
            var value = DateTime.Parse(dateTimeString);
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeActionThing.Enum)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
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
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.NonNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeActionThing.NonNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("2013-01-21")]
        [InlineData("2013-02-21")]
        [InlineData("2013-03-21")]
        public async Task NotAcceptedNullableValue_Should_Execute_When_ParameterIsValid(string dateTimeString)
        {
            var value = DateTime.Parse(dateTimeString);
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.NotAcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("2013-01-21")]
        [InlineData("2014-01-21")]
        [InlineData("2015-01-21")]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsValid(string dateTimeString)
        {
            var value = DateTime.Parse(dateTimeString);
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(DateTimeActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }

        [Fact]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsNull()
        {
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(DateTimeActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(null);
        }

        #endregion

        #region Invalid

        [Theory]
        [InlineData("2014-01-21")]
        [InlineData("2015-02-21")]
        [InlineData("2016-03-21")]
        public void Enum_Should_ReturnError_When_ParameterIsInvalid(string dateTimeString)
        {
            var value = DateTime.Parse(dateTimeString);
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NonNullableValue_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.NonNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeActionThing.NonNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("2013-01-25")]
        [InlineData("2013-01-26")]
        [InlineData("2013-01-27")]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(string dateTimeString)
        {
            var value = DateTime.Parse(dateTimeString);
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.NotAcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("2013-10-21")]
        [InlineData("2014-11-21")]
        [InlineData("2015-12-21")]
        public void AcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(string dateTimeString)
        {
            var value = DateTime.Parse(dateTimeString);
            var thing = new DateTimeActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(DateTimeActionThing.AcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value:O}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(DateTimeActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
        #endregion
        
        public class DateTimeActionThing : Thing
        {
            public override string Name => "date-time-action-thing";
            
            [ThingProperty(Ignore = true)]
            public DateTime? Value { get; private set; }
            
            public void Enum([ThingParameter(Enum = new object[] { "2013-01-21", "2014-01-22", "2015-01-23" })]DateTime value)
            {
                Value = value;
            }
            
            public void NonNullableValue([ThingParameter(IsNullable = false)]DateTime? value)
            {
                Value = value;
            }
            
            public void NotAcceptedNullableValue([ThingParameter(Enum = new object[] { "2013-01-21", "2013-02-21", "2013-03-21" })]DateTime? value)
            {
                Value = value;
            }
            
            public void AcceptedNullableValue([ThingParameter(Enum = new object[] { null, "2013-01-21", "2014-01-21", "2015-01-21" })]DateTime? value)
            {
                Value = value;
            }
        }
    }
}
