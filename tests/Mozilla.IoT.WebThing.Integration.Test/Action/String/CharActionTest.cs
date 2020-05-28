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
    public class CharActionTest : AbstractStructActionTest<char>
    {
        protected override char CreateValue()
        {
            return Fixture.Create<string>()[0];
        }

        protected override JsonElement CreateJson(char value)
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
        [InlineData('A')]
        [InlineData('B')]
        [InlineData('C')]
        public async Task Enum_Should_Execute_When_ParameterIsValid(char value)
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(CharActionThing.Enum)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
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
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.NonNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(CharActionThing.NonNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData('D')]
        [InlineData('E')]
        [InlineData('F')]
        public async Task NotAcceptedNullableValue_Should_Execute_When_ParameterIsValid(char value)
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.NotAcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(CharActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData('G')]
        [InlineData('H')]
        [InlineData('I')]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsValid(char value)
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
            
            context.Actions[nameof(CharActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }

        [Fact]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsNull()
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(CharActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(null);
        }

        #endregion

        #region Invalid

        [Theory]
        [InlineData('D')]
        [InlineData('E')]
        [InlineData('F')]
        public void Enum_Should_ReturnError_When_ParameterIsInvalid(char value)
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.Enum));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(CharActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NonNullableValue_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.NonNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(CharActionThing.NonNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData('H')]
        [InlineData('I')]
        [InlineData('K')]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(char value)
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.NotAcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(CharActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
                
        [Fact]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.NotAcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action");
        
            context.Actions[nameof(CharActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
                
        [Theory]
        [InlineData('X')]
        [InlineData('Y')]
        [InlineData('Z')]
        public void AcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(char value)
        {
            var thing = new CharActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(CharActionThing.NotAcceptedNullableValue));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}").GetProperty("action");
        
            context.Actions[nameof(CharActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Value.Should().NotBe(value);
        }
        #endregion
        
        public class CharActionThing : Thing
        {
            public override string Name => "char-action-thing";
            
            [ThingProperty(Ignore = true)]
            public char? Value { get; private set; }
            
            public void Enum([ThingParameter(Enum = new object[] { 'A', 'B', 'C' })]char value)
            {
                Value = value;
            }
            
            public void NonNullableValue([ThingParameter(IsNullable = false)]char? value)
            {
                Value = value;
            }
            
            public void NotAcceptedNullableValue([ThingParameter(Enum = new object[] {'D', 'E', 'F'})]char? value)
            {
                Value = value;
            }
            
            public void AcceptedNullableValue([ThingParameter(Enum = new object[] { null, 'G', 'H', 'I' })]char? value)
            {
                Value = value;
            }
        }
    }
}
