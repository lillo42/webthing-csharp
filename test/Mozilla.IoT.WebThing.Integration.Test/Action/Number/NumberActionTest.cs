using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Action.Number
{
    public abstract class NumberActionTest<T> : AbstractStructActionTest<T>
        where T : struct
    {
        protected override JsonElement CreateJson(T value)
        {
            return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {value} }} }} }}").GetProperty("action");
        }

        protected override IEnumerable<JsonElement> CreateInvalidJson()
        {
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{Fixture.Create<string>()}"" }} }} }}").GetProperty("action");
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<bool>().ToString().ToLower()} }} }} }}").GetProperty("action");
            
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": [ {Fixture.Create<int>()} ] }} }} }}").GetProperty("action");
        }

        #region Valid
        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task MinAndMax_Should_Execute_When_ParameterIsValid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.MinAndMax));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");
            
            context.Actions[nameof(NumberActionThing.MinAndMax)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(11)]
        [InlineData(50)]
        [InlineData(99)]
        public async Task ExclusiveMinAndMax_Should_Execute_When_ParameterIsValid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.ExclusiveMinAndMax));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");
            
            context.Actions[nameof(NumberActionThing.ExclusiveMinAndMax)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(20)]
        public async Task MultiOf_Should_Execute_When_ParameterIsValid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.MultiOf));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");
            
            context.Actions[nameof(NumberActionThing.MultiOf)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Enum_Should_Execute_When_ParameterIsValid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");
            
            context.Actions[nameof(NumberActionThing.Enum)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(20)]
        [InlineData(40)]
        public async Task NonNullableValue_Should_Execute_When_ParameterIsValid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.NonNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");
            
            context.Actions[nameof(NumberActionThing.NonNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(30)]
        public async Task NotAcceptedNullableValue_Should_Execute_When_ParameterIsValid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.NotAcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");
            
            context.Actions[nameof(NumberActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(value);
        }
        
        [Theory]
        [InlineData(100)]
        [InlineData(110)]
        [InlineData(120)]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsValid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");
            
            context.Actions[nameof(NumberActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(value);
        }

        [Fact]
        public async Task AcceptedNullableValue_Should_Execute_When_ParameterIsNull()
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.AcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.AcceptedNullableValue)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Number.Should().Be(null);
        }
        #endregion
        
        #region Invalid
        [Theory]
        [InlineData(9)]
        [InlineData(101)]
        public void MinAndMax_Should_ReturnError_When_ParameterIsInvalid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.MinAndMax));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.MinAndMax)].TryAdd(jsonElement, out _).Should().BeTrue();
            thing.Number.Should().NotBe(value);
        }
        
        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        public void ExclusiveMinAndMax_Should_ReturnError_When_ParameterIsInvalid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.ExclusiveMinAndMax));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.ExclusiveMinAndMax)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Number.Should().NotBe(value);
        }
        
        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        public void MultiOf_Should_ReturnError_When_ParameterIsInvalid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.MultiOf));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.MultiOf)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Number.Should().NotBe(value);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(10)]
        public void Enum_Should_ReturnError_When_ParameterIsInvalid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Number.Should().NotBe(value);
        }
        
        [Fact]
        public void NonNullableValue_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.NonNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.NonNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(11)]
        [InlineData(19)]
        [InlineData(29)]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Number.Should().NotBe(value);
        }
        
        [Fact]
        public void NotAcceptedNullableValue_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.NotAcceptedNullableValue));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": null }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.NotAcceptedNullableValue)].TryAdd(jsonElement, out _).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(111)]
        [InlineData(19)]
        [InlineData(200)]
        public void AcceptedNullableValue_Should_ReturnError_When_ParameterIsInvalid(int value)
        {
            var thing = new NumberActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(NumberActionThing.Enum));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""number"": {value} }} }} }}").GetProperty("action");

            context.Actions[nameof(NumberActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
            thing.Number.Should().NotBe(value);
        }

        #endregion
        
        public class NumberActionThing : Thing
        {
            public override string Name => "number-action-thing";

            [ThingProperty(Ignore = true)]
            public T? Number { get; private set; }
            
            public void MinAndMax([ThingParameter(Minimum = 10, Maximum = 100)]T number)
            {
                Number = number;
            }
            
            public void ExclusiveMinAndMax([ThingParameter(ExclusiveMinimum = 10, ExclusiveMaximum = 100)]T number)
            {
                Number = number;
            }
            
            public void MultiOf([ThingParameter(MultipleOf = 2)]T number)
            {
                Number = number;
            }
            
            public void Enum([ThingParameter(Enum = new object[] {1, 2, 3 })]T number)
            {
                Number = number;
            }
            
            public void NonNullableValue([ThingParameter(IsNullable = false)]T? number)
            {
                Number = number;
            }
            
            public void NotAcceptedNullableValue([ThingParameter(Enum = new object[] {10, 20, 30})]T? number)
            {
                Number = number;
            }
            
            public void AcceptedNullableValue([ThingParameter(Enum = new object[] { null, 100, 110, 120 })]T? number)
            {
                Number = number;
            }
        }
    }
    
    public class ByteProperty : NumberActionTest<byte> { }
    public class SByteProperty : NumberActionTest<sbyte> { }
    
    public class ShortProperty : NumberActionTest<short> { }
    public class UShortProperty : NumberActionTest<ushort> { }
    
    public class IntProperty : NumberActionTest<int> { }
    public class UIntProperty : NumberActionTest<uint> { }
    
    public class LongProperty : NumberActionTest<long> { }
    public class ULongProperty : NumberActionTest<ulong> { }
    
    public class FloatProperty : NumberActionTest<float> { }
    public class DoubleProperty : NumberActionTest<double> { }
    public class DecimalProperty : NumberActionTest<decimal> { }
    
}
