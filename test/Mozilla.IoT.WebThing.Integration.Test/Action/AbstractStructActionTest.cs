using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Action
{
    public abstract class AbstractStructActionTest<T> : AbstractActionTest<T>
        where T : struct
    {

        #region Valid

        [Fact]
        public async Task Invoke_Should_Execute_When_ParameterIsValid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.Invoke));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            context.Actions[nameof(ActionThing.Invoke)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Fact]
        public async Task InvokeNullable_Should_Execute_When_ParameterIsValid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeNullable));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            context.Actions[nameof(ActionThing.InvokeNullable)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(value);

            jsonElement =  jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.InvokeNullable)].TryAdd(jsonElement, out info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(null);
        }
        
        [Fact]
        public async Task InvokeWithService_Should_Execute_When_ParameterIsValid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeWithService));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            context.Actions[nameof(ActionThing.InvokeWithService)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Fact]
        public async Task InvokeNullableWithService_Should_Execute_When_ParameterIsValid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeNullableWithService));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            context.Actions[nameof(ActionThing.InvokeNullableWithService)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(value);

            jsonElement =  jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.InvokeNullable)].TryAdd(jsonElement, out info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(null);
        }

        [Fact]
        public async Task InvokeAsync_Should_Execute_When_ParameterIsValid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeAsync));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            context.Actions[nameof(ActionThing.InvokeAsync)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Fact]
        public async Task InvokeNullableAsync_Should_Execute_When_ParameterIsValid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeNullableAsync));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            context.Actions[nameof(ActionThing.InvokeNullableAsync)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(value);

            jsonElement =  jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.InvokeNullable)].TryAdd(jsonElement, out info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Pending);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(null);
        }

        [Fact]
        public void Serialize()
        {
            var type = typeof(T).ToJsonType().ToString().ToLower();
            var value = typeof(T).IsEnum
                ? $@" ""enum"": [""{string.Join(@""" , """, typeof(T).GetEnumNames())}""], "
                : string.Empty;
            TestResponse<ActionThing>(string.Format(RESPONSE, type, value));
        }
        
        #endregion
        

        
        protected override void ConfigureServiceCollection(IServiceCollection collection)
        {
            collection.AddScoped<IFoo, Foo>();
        }

        public class ActionThing : Thing
        {
            public override string Name => "action-thing";
            
            [ThingProperty(Ignore = true)]
            public T Value { get; private set; }

            [ThingProperty(Ignore = true)]
            public T? NullableValue { get; private set; }
            
            public IFoo Foo { get; private set; }
            
            public void Invoke(T value)
            {
                Value = value;
            }
            
            public void InvokeNullable(T? value)
            {
                NullableValue = value;
            }
            
            public void InvokeWithService(T value, [FromServices]IFoo foo)
            {
                Value = value;
                Foo = foo;
            }
            
            public void InvokeNullableWithService(T? value, [FromServices]IFoo foo)
            {
                NullableValue = value;
                Foo = foo;
            }
            
            public async Task InvokeAsync(T value, CancellationToken cancellationToken = default)
            {
                Value = value;
                await Task.Delay(1_000, cancellationToken)
                    .ConfigureAwait(false);
            }
            
            public async Task InvokeNullableAsync(T? value, CancellationToken cancellationToken = default)
            {
                NullableValue = value;
                await Task.Delay(1_000, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        
        public class Foo : IFoo
        {
            public Guid Id { get; } = Guid.NewGuid();
        }
        
        public interface IFoo
        {
            Guid Id { get; } 
        }
        
        private const string RESPONSE = @"{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""properties"": {{
    ""value"": {{
      ""type"": ""{0}"",
      {1}
      ""links"": [
        {{
          ""href"": ""/things/property-thing/properties/value"",
          ""rel"": ""property""
        }}
      ]
    }},
    ""nullableValue"": {{
      ""type"": ""{0}"",
      {1}
      ""links"": [
        {{
          ""href"": ""/things/property-thing/properties/nullableValue"",
          ""rel"": ""property""
        }}
      ]
    }}
  }},
  ""links"": [
    {{
      ""href"": ""properties"",
      ""rel"": ""/things/property-thing/properties""
    }},
    {{
      ""href"": ""events"",
      ""rel"": ""/things/property-thing/events""
    }},
    {{
      ""href"": ""actions"",
      ""rel"": ""/things/property-thing/actions""
    }}
  ]
}}";
    }
}
