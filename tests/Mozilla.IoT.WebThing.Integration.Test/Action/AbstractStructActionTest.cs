using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
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
            
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(value);
        }
        
        [Fact]
        public async Task InvokeNullable_Should_Execute_When_ParameterIsNull()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeNullable));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.InvokeNullable)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
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
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
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
            
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(value);
        }
        
        [Fact]
        public async Task InvokeNullableWithService_Should_Execute_When_ParameterIsNull()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeNullableWithService));

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.InvokeNullable)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
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
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
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
            
            info.Should().NotBeNull();
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(value);
        }
        
        [Fact]
        public async Task InvokeNullableAsync_Should_Execute_When_ParameterIsNull()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeNullableAsync));

            var jsonElement =  JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.InvokeNullable)].TryAdd(jsonElement, out var info).Should().BeTrue();
            
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(null);
        }

       
        #endregion

        #region Invalid
        [Fact]
        public void Invoke_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.Invoke));
            
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""Invoke"": { ""input"": { ""value"": null } } }").GetProperty("Invoke");
            context.Actions[nameof(ActionThing.Invoke)].TryAdd(jsonElement, out var info).Should().BeFalse();
            thing.Value.Should().NotBe(null);
        }

        [Fact]
        public void InvokeWithService_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeWithService));

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invoke"": { ""input"": { ""value"": null } } }").GetProperty("invoke");
            context.Actions[nameof(ActionThing.InvokeWithService)].TryAdd(jsonElement, out var info).Should().BeFalse();
            thing.Value.Should().NotBeNull();
        }
        
        [Fact]
        public void InvokeAsync_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeAsync));

            var jsonElement =  JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invoke"": { ""input"": { ""value"": null } } }").GetProperty("invoke");
            context.Actions[nameof(ActionThing.InvokeAsync)].TryAdd(jsonElement, out _).Should().BeFalse();
            
            thing.Value.Should().NotBeNull();
        }
        
        [Theory]
        [InlineData(nameof(ActionThing.Invoke))]
        [InlineData(nameof(ActionThing.InvokeNullable))]
        [InlineData(nameof(ActionThing.InvokeWithService))]
        [InlineData(nameof(ActionThing.InvokeNullableWithService))]
        [InlineData(nameof(ActionThing.InvokeAsync))]
        [InlineData(nameof(ActionThing.InvokeNullableAsync))]
        public void ExecuteMethod_Should_ReturnError_When_ParameterIsInvalid(string method)
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;

            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().HaveCount(6);
            context.Actions.Should().ContainKey(method);

            var jsons = CreateInvalidJson();

            foreach (var element in jsons)
            {
                context.Actions[method].TryAdd(element, out _).Should().BeFalse();
            }
        }
        #endregion
        
        [Fact]
        public void Serialize()
        {
            var type = typeof(T).ToJsonType().ToString().ToLower();
            var value = typeof(T).IsEnum
                ? $@" , ""enum"": [""{string.Join(@""" , """, typeof(T).GetEnumNames())}""], "
                : string.Empty;
            TestResponse<ActionThing>(string.Format(s_response, type, value));
        }
        
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

        private const string s_response = @"
{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""security"": ""nosec_sc"",
    ""securityDefinitions"": {{
        ""nosec_sc"": {{
            ""scheme"": ""nosec""
        }}
  }},
  ""actions"": {{
    ""invoke"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/invoke"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""{0}""
            {1}
          }}
        }}
      }}
    }},
    ""invokeNullable"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/invokeNullable"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""{0}""
            {1}
          }}
        }}
      }}
    }},
    ""invokeWithService"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/invokeWithService"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""{0}""
            {1}
          }}
        }}
      }}
    }},
    ""invokeNullableWithService"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/invokeNullableWithService"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""{0}""
            {1}
          }}
        }}
      }}
    }},
    ""invokeAsync"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/invokeAsync"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""{0}""
            {1}
          }}
        }}
      }}
    }},
    ""invokeNullableAsync"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/invokeNullableAsync"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""{0}""
            {1}
          }}
        }}
      }}
    }}
  }},
  ""links"": [
    {{
        ""rel"": ""properties"",
        ""href"": ""/things/action-thing/properties""
    }},
    {{
        ""rel"": ""actions"",
        ""href"": ""/things/action-thing/actions""
    }},
    {{
        ""rel"": ""events"",
        ""href"": ""/things/action-thing/events""
    }}
  ]
}}
";
    }
}
