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
    public class GuidActionTest : AbstractStructActionTest<Guid>
    {
        protected override JsonElement CreateJson(Guid value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}")
                .GetProperty("action");
        }

        protected override IEnumerable<JsonElement> CreateInvalidJson()
        {
            yield return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{Fixture.Create<DateTime>()}"" }} }} }}")
                .GetProperty("action");
            
            yield return JsonSerializer
                .Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<bool>().ToString().ToLower()} }} }} }}")
                .GetProperty("action");
            
            yield return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<int>()} }} }} }}")
                .GetProperty("action");
        }


        #region Valid
        
        [Theory]
        [InlineData("be5cdf5b-4c72-4db9-8646-d20ffb94caec")]
        [InlineData("fb4d04e2-8e5a-4202-b0c5-d5fb5e68cfe2")]
        [InlineData("035a29bb-bccd-4816-a4fd-3bdfd1e32acd")]
        public async Task Enum_Should_Execute_When_ParameterIsValid(string guidString)
        {
            var value = Guid.Parse(guidString);   
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(GuidActionThing.Enum)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
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
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.NonNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(GuidActionThing.NonNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("7539e439-fa4b-4f1f-8a75-7712a483ffa1")]
        [InlineData("7526477b-d125-4a93-ab3d-5760bbd2d2f7")]
        [InlineData("4af37617-7254-40df-8231-04055cf1c3a3")]
        public async Task NotAcceptedNullableValue_Should_Execute_When_ParameterIsValid(string guidString)
        {
            var value = Guid.Parse(guidString);   
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.NotAcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(GuidActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("34167747-61c6-4580-8e91-116071918690")]
        [InlineData("b26958c9-da73-4987-b7cf-5a98f58acfe2")]
        [InlineData("0f530eca-d670-4495-b4c0-d5b37acf56ef")]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsValid(string guidString)
        {
            var value = Guid.Parse(guidString);
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(GuidActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }

        [Fact]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsNull()
        {
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(GuidActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(null);
        }

        #endregion

        #region Invalid

        [Theory]
        [InlineData("7539e439-fa4b-4f1f-8a75-7712a483ffa1")]
        [InlineData("7526477b-d125-4a93-ab3d-5760bbd2d2f7")]
        [InlineData("4af37617-7254-40df-8231-04055cf1c3a3")]
        public void Enum_Should_ReturnError_When_ParameterIsInvalid(string guidString)
        {
            var value = Guid.Parse(guidString);
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(GuidActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NonNullableValue_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.NonNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(GuidActionThing.NonNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("34167747-61c6-4580-8e91-116071918690")]
        [InlineData("b26958c9-da73-4987-b7cf-5a98f58acfe2")]
        [InlineData("0f530eca-d670-4495-b4c0-d5b37acf56ef")]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(string guidString)
        {
            var value = Guid.Parse(guidString);
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(GuidActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.NotAcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(GuidActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData("be5cdf5b-4c72-4db9-8646-d20ffb94caec")]
        [InlineData("fb4d04e2-8e5a-4202-b0c5-d5fb5e68cfe2")]
        [InlineData("035a29bb-bccd-4816-a4fd-3bdfd1e32acd")]
        public void AcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(Guid value)
        {
            var thing = new GuidActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(GuidActionThing.AcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(GuidActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
        #endregion
        
        public class GuidActionThing : Thing
        {
            public override string Name => "guid-action-thing";
            
            [ThingProperty(Ignore = true)]
            public Guid? Value { get; private set; }
            
            public void Enum([ThingParameter(Enum = new object[] { "be5cdf5b-4c72-4db9-8646-d20ffb94caec", "fb4d04e2-8e5a-4202-b0c5-d5fb5e68cfe2", "035a29bb-bccd-4816-a4fd-3bdfd1e32acd" })]Guid value)
            {
                Value = value;
            }
            
            public void NonNullableValue([ThingParameter(IsNullable = false)]Guid? value)
            {
                Value = value;
            }
            
            public void NotAcceptedNullableValue([ThingParameter(Enum = new object[] {"7539e439-fa4b-4f1f-8a75-7712a483ffa1", "7526477b-d125-4a93-ab3d-5760bbd2d2f7", "4af37617-7254-40df-8231-04055cf1c3a3"})]Guid? value)
            {
                Value = value;
            }
            
            public void AcceptedNullableValue([ThingParameter(Enum = new object[] { null, "34167747-61c6-4580-8e91-116071918690", "b26958c9-da73-4987-b7cf-5a98f58acfe2", "0f530eca-d670-4495-b4c0-d5b37acf56ef" })]Guid? value)
            {
                Value = value;
            }
        }
    }
}
