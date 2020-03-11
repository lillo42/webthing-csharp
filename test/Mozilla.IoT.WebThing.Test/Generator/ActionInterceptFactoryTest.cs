using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Actions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator
{
    public class ActionInterceptFactoryTest
    {
        private readonly Fixture _fixture;
        private readonly ActionInterceptFactory _factory;
        private readonly IServiceProvider _provider;
        private readonly ILogger<ActionInfo> _logger;

        public ActionInterceptFactoryTest()
        {
            _fixture = new Fixture();
            _provider = Substitute.For<IServiceProvider>();
            _logger = Substitute.For<ILogger<ActionInfo>>();

            _provider.GetService(typeof(ILogger<ActionInfo>))
                .Returns(_logger);
            
            _factory = new ActionInterceptFactory(new ThingOption
            {
                IgnoreCase = true
            });
        }
        
        [Fact]
        public void Ignore()
        {
            var thing = new ActionThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
            _factory.Actions.Should().NotContainKey(nameof(ActionThing.Ignore));
        }
        
        [Fact]
        public void Different()
        {
            var thing = new ActionThing(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
            _factory.Actions.Should().NotContainKey(nameof(ActionThing.DifferentName));
            _factory.Actions.Should().ContainKey("test");

            var json = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": {} }");
            _factory.Actions["test"].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Logger.Should().HaveCount(1);
            thing.Logger.Should().HaveElementAt(0, nameof(ActionThing.DifferentName));
        }
        
        [Fact]
        public void SyncAction_NotAttribute_NoNullable()
        {
            var thing = new SyncAction(); 
            CodeGeneratorFactory.Generate(thing, new []{ _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NotAttribute));

            var @bool = _fixture.Create<bool>();
            var @byte = _fixture.Create<byte>();
            var @sbyte = _fixture.Create<sbyte>();
            var @short = _fixture.Create<short>();
            var @ushort = _fixture.Create<ushort>();
            var @int = _fixture.Create<int>();
            var @uint = _fixture.Create<uint>();
            var @long = _fixture.Create<long>();
            var @ulong = _fixture.Create<ulong>();
            var @float = _fixture.Create<float>();
            var @double = _fixture.Create<double>();
            var @decimal = _fixture.Create<decimal>();
            
            var @string = _fixture.Create<string>();
            var dateTime = _fixture.Create<DateTime>();
            var guid = _fixture.Create<Guid>();
            var timeSpan = _fixture.Create<TimeSpan>();
            
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
                        ""bool"": {@bool.ToString().ToLower()},
                        ""byte"": {@byte},
                        ""sbyte"": {@sbyte},
                        ""short"": {@short},
                        ""ushort"": {@ushort},
                        ""int"": {@int},
                        ""uint"": {@uint},
                        ""long"": {@long},
                        ""ulong"": {@ulong},
                        ""float"": {@float},
                        ""double"": {@double},
                        ""decimal"": {@decimal},
                        ""string"": ""{@string}"",
                        ""dateTime"": ""{@dateTime:O}"",
                        ""guid"": ""{@guid}"",
                        ""timeSpan"": ""{@timeSpan}""
                }} 
            }}");
            
            _factory.Actions[nameof(SyncAction.NotAttribute)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(16);
            thing.Values.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                [nameof(@bool)] = @bool,
                [nameof(@byte)] = @byte,
                [nameof(@sbyte)] = @sbyte,
                [nameof(@short)] = @short,
                [nameof(@ushort)] = @ushort,
                [nameof(@int)] = @int,
                [nameof(@uint)] = @uint,
                [nameof(@long)] = @long,
                [nameof(@ulong)] = @ulong,
                [nameof(@float)] = @float,
                [nameof(@double)] = @double,
                [nameof(@decimal)] = @decimal,
                [nameof(@string)] = @string,
                [nameof(@dateTime)] = @dateTime,
                [nameof(@timeSpan)] = @timeSpan,
                [nameof(@guid)] = @guid
            });
        }


        #region Thing

        public class ActionThing : Thing
        {
            public List<string> Logger { get; } = new List<string>();
            public override string Name => "action";

            [ThingAction(Ignore = true)]
            public void Ignore()
            {
                
            }
            
            [ThingAction(Name = "test")]
            public void DifferentName()
            {
                Logger.Add(nameof(DifferentName));
            }
        }
        
        public class SyncAction : Thing
        {
            public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();
            public override string Name => "sync-action";

            public void NotAttribute(
                bool @bool,
                byte @byte,
                sbyte @sbyte,
                short @short,
                ushort @ushort,
                int @int,
                uint @uint,
                long @long,
                ulong @ulong,
                float @float,
                double @double,
                decimal @decimal,
                string @string,
                DateTime @dateTime,
                TimeSpan @timeSpan,
                Guid @guid)
            {
                Values.Add(nameof(@bool), @bool);
                Values.Add(nameof(@byte), @byte);
                Values.Add(nameof(@sbyte), @sbyte);
                Values.Add(nameof(@short), @short);
                Values.Add(nameof(@ushort), @ushort);
                Values.Add(nameof(@int), @int);
                Values.Add(nameof(@uint), @uint);
                Values.Add(nameof(@long), @long);
                Values.Add(nameof(@ulong), @ulong);
                Values.Add(nameof(@float), @float);
                Values.Add(nameof(@double), @double);
                Values.Add(nameof(@decimal), @decimal);
                Values.Add(nameof(@string), @string);
                Values.Add(nameof(@dateTime), @dateTime);
                Values.Add(nameof(@timeSpan), @timeSpan);
                Values.Add(nameof(@guid), @guid);
            }
        }

        #endregion
    }
}
