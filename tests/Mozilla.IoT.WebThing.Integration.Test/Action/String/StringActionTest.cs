using System;
using System.Collections.Generic;
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

namespace Mozilla.IoT.WebThing.Integration.Test.Action.String
{
    public class StringActionTest : AbstractActionTest<string>
    {
        protected override JsonElement CreateJson(string value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}")
                .GetProperty("action");
        }

        protected override IEnumerable<JsonElement> CreateInvalidJson()
        {
            yield return JsonSerializer
                .Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<bool>().ToString().ToLower()} }} }} }}")
                .GetProperty("action");
            
            yield return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<int>()} }} }} }}")
                .GetProperty("action");
        }
        
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
            
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeNullableAsync));
        
            var jsonElement =  JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.InvokeNullableAsync)].TryAdd(jsonElement, out var info).Should().BeTrue();
                    
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(null);
        }
        
        [Fact]
        public async Task Pattern_Should_Execute_When_ParameterIsValid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.Pattern));

            var jsonElement = CreateJson("test@test.com");
            
            context.Actions[nameof(ActionThing.Pattern)].TryAdd(jsonElement, out var info).Should().BeTrue();
                    
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be("test@test.com");
        }
        
        [Theory]
        [InlineData("Lorem")]
        [InlineData("ipsum etiam")]
        [InlineData("nec litora placerat")]
        public async Task Enum_Should_Execute_When_ParameterIsValid(string value)
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.Enum));

            var jsonElement = CreateJson(value);
            
            context.Actions[nameof(ActionThing.Enum)].TryAdd(jsonElement, out var info).Should().BeTrue();
                    
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("elit velit")]
        [InlineData("erat eleifend integer enim")]
        [InlineData("porta praesent dictum")]
        public async Task EnumNull_Should_Execute_When_ParameterIsValid(string value)
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.EnumNull));

            var jsonElement = CreateJson(value);
            
            context.Actions[nameof(ActionThing.EnumNull)].TryAdd(jsonElement, out var info).Should().BeTrue();
                    
            info.Status.Should().Be(ActionStatus.Created);
            await info.ExecuteAsync(thing, Provider).ConfigureAwait(false);
            info.Status.Should().Be(ActionStatus.Completed);
            thing.NullableValue.Should().Be(value);
        }
        
        [Fact]
        public async Task EnumNull_Should_Execute_When_ParameterIsNull()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.EnumNull));
        
            var jsonElement =  JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invokeNullable"": { ""input"": { ""value"": null } } }").GetProperty("invokeNullable");
            context.Actions[nameof(ActionThing.EnumNull)].TryAdd(jsonElement, out var info).Should().BeTrue();
                    
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
            var thing = new ActionThing
            {
                Value = Fixture.Create<string>()
            };
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(nameof(ActionThing.Invoke));
                    
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""Invoke"": { ""input"": { ""value"": null } } }").GetProperty("Invoke");
            context.Actions[nameof(ActionThing.Invoke)].TryAdd(jsonElement, out var info).Should().BeFalse();
            thing.Value.Should().NotBe(null);
        }
        
        [Fact]
        public void InvokeWithService_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new ActionThing()
            {
                Value = Fixture.Create<string>()
            };
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeWithService));
        
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invoke"": { ""input"": { ""value"": null } } }").GetProperty("invoke");
            context.Actions[nameof(ActionThing.InvokeWithService)].TryAdd(jsonElement, out var info).Should().BeFalse();
            thing.Value.Should().NotBeNull();
        }
                
        [Fact]
        public void InvokeAsync_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new ActionThing
            {
                Value = Fixture.Create<string>()
            };
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.InvokeAsync));
        
            var jsonElement =  JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invoke"": { ""input"": { ""value"": null } } }").GetProperty("invoke");
            context.Actions[nameof(ActionThing.InvokeAsync)].TryAdd(jsonElement, out _).Should().BeFalse();
                    
            thing.Value.Should().NotBeNull();
        }
        
        [Fact]
        public void Enum_Should_ReturnError_When_ParameterIsNull()
        {
            var thing = new ActionThing
            {
                Value = Fixture.Create<string>()
            };
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.Enum));
        
            var jsonElement =  JsonSerializer.Deserialize<JsonElement>(
                @"{ ""invoke"": { ""input"": { ""value"": null } } }").GetProperty("invoke");
            context.Actions[nameof(ActionThing.Enum)].TryAdd(jsonElement, out _).Should().BeFalse();
                    
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
            
            context.Actions.Should().ContainKey(method);
        
            var jsons = CreateInvalidJson();
        
            foreach (var element in jsons)
            {
                context.Actions[method].TryAdd(element, out _).Should().BeFalse();
            }
        }
        
        [Fact]
        public void Pattern_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.Pattern));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            
            context.Actions[nameof(ActionThing.Pattern)].TryAdd(jsonElement, out var info).Should().BeFalse();
        }
        
        [Fact]
        public void EnumNull_Should_ReturnError_When_ParameterIsInvalid()
        {
            var thing = new ActionThing();
            var context = Factory.Create(thing, new ThingOption());
            thing.ThingContext = context;
        
            context.Events.Should().BeEmpty();
            context.Properties.Should().BeEmpty();
        
            context.Actions.Should().NotBeEmpty();
            
            context.Actions.Should().ContainKey(nameof(ActionThing.EnumNull));

            var value = CreateValue();
            var jsonElement = CreateJson(value);
            
            context.Actions[nameof(ActionThing.EnumNull)].TryAdd(jsonElement, out var info).Should().BeFalse();
        }
        
        
        #endregion
        
        [Fact]
        public void Serialize()
        {
            TestResponse<ActionThing>(string.Format(s_response));
        }
        
        protected override void ConfigureServiceCollection(IServiceCollection collection)
        {
            collection.AddScoped<IFoo, Foo>();
        }
        
        public class ActionThing : Thing
        {
            public override string Name => "action-thing";
            
            [ThingProperty(Ignore = true)]
            public string Value { get; set; }

            [ThingProperty(Ignore = true)]
            public string NullableValue { get; set; }
            
            public IFoo Foo { get; private set; }
            
            public void Invoke([ThingParameter(IsNullable = false)]string value)
            {
                Value = value;
            }
            
            public void InvokeNullable(string value)
            {
                NullableValue = value;
            }
            
            public void InvokeWithService([ThingParameter(IsNullable = false)]string value, [FromServices]IFoo foo)
            {
                Value = value;
                Foo = foo;
            }
            
            public void InvokeNullableWithService(string value, [FromServices]IFoo foo)
            {
                NullableValue = value;
                Foo = foo;
            }
            
            public async Task InvokeAsync([ThingParameter(IsNullable = false)]string value, CancellationToken cancellationToken = default)
            {
                Value = value;
                await Task.Delay(1_000, cancellationToken)
                    .ConfigureAwait(false);
            }
            
            public async Task InvokeNullableAsync(string value, CancellationToken cancellationToken = default)
            {
                NullableValue = value;
                await Task.Delay(1_000, cancellationToken)
                    .ConfigureAwait(false);
            }

            public void Pattern([ThingParameter(Pattern = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$")] string value)
            {
                Value = value;
            }
            
            public void Enum([ThingParameter(Enum = new object[] { "Lorem", "ipsum etiam", "nec litora placerat" })] string value)
            {
                Value = value;
            }  
            
            public void EnumNull([ThingParameter(Enum = new object[] { null, "elit velit", "erat eleifend integer enim", "porta praesent dictum" })] string value)
            {
                NullableValue = value;
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
            ""type"": ""string""
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
            ""type"": ""string""
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
            ""type"": ""string""
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
            ""type"": ""string""
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
            ""type"": ""string""
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
            ""type"": ""string""
          }}
        }}
      }}
    }},
    ""pattern"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/pattern"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""string"",
            ""pattern"": ""^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{{2,3}}$""
          }}
        }}
      }}
    }},
    ""enum"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/enum"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""string"",
            ""enum"": [
              ""Lorem"",
              ""ipsum etiam"",
              ""nec litora placerat""
            ]
          }}
        }}
      }}
    }},
    ""enumNull"": {{
      ""links"": [
        {{
          ""href"": ""/things/action-thing/actions/enumNull"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""string"",
            ""enum"": [
              null,
              ""elit velit"",
              ""erat eleifend integer enim"",
              ""porta praesent dictum""
            ]
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
