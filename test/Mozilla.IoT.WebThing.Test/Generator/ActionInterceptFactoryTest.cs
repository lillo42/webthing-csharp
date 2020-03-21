using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
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
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().NotContainKey(nameof(ActionThing.Ignore));
        }

        [Fact]
        public void Different()
        {
            var thing = new ActionThing();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
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

        #region Sync

        [Fact]
        public void CallActionSyncNoNullableValid()
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableNotAttribute));

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
            var dateTimeOffset = _fixture.Create<DateTimeOffset>();
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
                        ""dateTimeOffset"": ""{@dateTimeOffset:O}"",
                        ""guid"": ""{@guid}"",
                        ""timeSpan"": ""{@timeSpan}""
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableNotAttribute)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(17);
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
                [nameof(@dateTimeOffset)] = @dateTimeOffset,
                [nameof(@timeSpan)] = @timeSpan,
                [nameof(@guid)] = @guid
            });
        }

        [Fact]
        public void CallActionSyncNoNullableWithValidationValid()
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableAttribute));
            
            var @minMax = 2;
            var @multipleOf =  10;
            var @exclusive = 2;
            var @string = _fixture.Create<string>();
            var mail = "test@test.com";
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
                        ""multipleOf"": {@multipleOf},
                        ""minMax"": {@minMax},
                        ""exclusive"": {@exclusive},
                        ""string"": ""{@string}"",
                        ""mail"": ""{mail}""
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableAttribute)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(5);
            thing.Values.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                [nameof(@multipleOf)] = @multipleOf,
                [nameof(@minMax)] = @minMax,
                [nameof(@exclusive)] = @exclusive,
                [nameof(@string)] = @string,
                [nameof(@mail)] = @mail
            });

            minMax = 100;
            exclusive = 99;
            thing.Values.Clear();
            
            json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
                        ""multipleOf"": {@multipleOf},
                        ""minMax"": {@minMax},
                        ""exclusive"": {@exclusive},
                        ""string"": ""{@string}"",
                        ""mail"": ""{mail}""
                }} 
            }}");
            
            _factory.Actions[nameof(SyncAction.NoNullableAttribute)].TryAdd(json, out var action2).Should().BeTrue();
            action2.Should().NotBeNull();
            result = action2.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(5);
            thing.Values.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                [nameof(@multipleOf)] = @multipleOf,
                [nameof(@minMax)] = @minMax,
                [nameof(@exclusive)] = @exclusive,
                [nameof(@string)] = @string,
                [nameof(@mail)] = @mail
            });
        }

        [Theory]
        [InlineData(9, 2, 2, "tes", "test@test.com")]
        [InlineData(10, 1, 2, "tes", "test@test.com")]
        [InlineData(10, 2, 1, "tes", "test@test.com")]
        [InlineData(10, 2, 2, "", "test@test.com")]
        [InlineData(10, 2, 2, null, "test@test.com")]
        [InlineData(10, 2, 2, "tes", "test")]
        [InlineData(10, 2, 2, "tes", "")]
        [InlineData(10, 2, 2, "tes", null)]
        public void CallActionSyncNoNullableWithValidationInvalid(int @multipleOf, int @minMax, int @exclusive, string @string, string @mail)
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableAttribute));

            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
                        ""multipleOf"": {@multipleOf},
                        ""minMax"": {@minMax},
                        ""exclusive"": {@exclusive},
                        ""string"": ""{@string}"",
                        ""mail"": ""{mail}""
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableAttribute)].TryAdd(json, out var action).Should().BeFalse();
            action.Should().BeNull();
        }
        
        [Fact]
        public void CallActionSyncNullableValid()
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NullableWithNotAttribute));

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
            var dateTimeOffset = _fixture.Create<DateTimeOffset>();
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
                        ""dateTimeOffset"": ""{@dateTimeOffset:O}"",
                        ""guid"": ""{@guid}"",
                        ""timeSpan"": ""{@timeSpan}""
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NullableWithNotAttribute)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(17);
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
                [nameof(@dateTimeOffset)] = @dateTimeOffset,
                [nameof(@timeSpan)] = @timeSpan,
                [nameof(@guid)] = @guid
            });
        }
        
        [Fact]
        public void CallActionSyncNullableValidWithNullValue()
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NullableWithNotAttribute));


            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
                        ""bool"": null,
                        ""byte"": null,
                        ""sbyte"": null,
                        ""short"": null,
                        ""ushort"": null,
                        ""int"": null,
                        ""uint"": null,
                        ""long"": null,
                        ""ulong"": null,
                        ""float"": null,
                        ""double"": null,
                        ""decimal"": null,
                        ""string"": null,
                        ""dateTime"": null,
                        ""dateTimeOffset"": null,
                        ""guid"": null,
                        ""timeSpan"": null
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NullableWithNotAttribute)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(17);
            thing.Values.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["bool"] = null,
                ["byte"] = null,
                ["sbyte"] = null,
                ["short"] = null,
                ["ushort"] = null,
                ["int"] = null,
                ["uint"] = null,
                ["long"] = null,
                ["ulong"] = null,
                ["float"] = null,
                ["double"] = null,
                ["decimal"] = null,
                ["string"] = null,
                ["dateTime"] = null,
                ["dateTimeOffset"] = null,
                ["timeSpan"] = null,
                ["guid"] = null
            });
        }

        [Fact]
        public void FromService()
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.FromService));

            var json = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": { } }");

            var foo = Substitute.For<IFoo>();
            var fooText = _fixture.Create<string>();
            foo.Text.Returns(fooText);

            _provider.GetService(typeof(IFoo))
                .Returns(foo);

            _factory.Actions[nameof(SyncAction.FromService)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            action.Status.Should().Be(ActionStatus.Completed);
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(1);
            thing.Values.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                [nameof(foo)] = fooText
            });
        }
        #endregion

        #region Async

        [Fact]
        public async Task Execute()
        {
            var thing = new AsyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(AsyncAction.Execute));
            var json = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": {} }");
            _factory.Actions[nameof(AsyncAction.Execute)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeFalse();
            action.Status.Should().Be(ActionStatus.Executing);
            await result;
            action.Status.Should().Be(ActionStatus.Completed);
            
            thing.Values.Should().HaveCount(1);
            thing.Values.Should().HaveElementAt(0, nameof(AsyncAction.Execute));
        }
        
        [Fact]
        public async Task ExecuteWithCancellationToken()
        {
            var thing = new AsyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(AsyncAction.ExecuteWithCancellationToken));
            var json = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": {} }");
            _factory.Actions[nameof(AsyncAction.ExecuteWithCancellationToken)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            action.Status.Should().Be(ActionStatus.Executing);
            result.IsCompleted.Should().BeFalse();
            await result;
            action.Status.Should().Be(ActionStatus.Completed);
            
            thing.Values.Should().HaveCount(1);
            thing.Values.Should().HaveElementAt(0, nameof(AsyncAction.ExecuteWithCancellationToken));
        }
        
        [Fact]
        public async Task ExecuteToCancel()
        {
            var thing = new AsyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(AsyncAction.ExecuteToCancel));
            var json = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": {} }");
            _factory.Actions[nameof(AsyncAction.ExecuteToCancel)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            action.Status.Should().Be(ActionStatus.Executing);
            result.IsCompleted.Should().BeFalse();
            action.Cancel();
            await result;
            action.Status.Should().Be(ActionStatus.Completed);
            
            thing.Values.Should().HaveCount(1);
            thing.Values.Should().HaveElementAt(0, nameof(AsyncAction.ExecuteToCancel));
        }

        #endregion

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

            public void NoNullableNotAttribute(
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
                DateTimeOffset @dateTimeOffset,
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
                Values.Add(nameof(@dateTimeOffset), @dateTimeOffset);
                Values.Add(nameof(@timeSpan), @timeSpan);
                Values.Add(nameof(@guid), @guid);
            }

            public void NoNullableAttribute(
                [ThingParameter(Minimum = 2, Maximum = 100)]int @minMax,
                [ThingParameter(MultipleOf = 2)]int @multipleOf,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]int @exclusive,
                [ThingParameter(MinimumLength = 1, MaximumLength = 40)]string @string,
                [ThingParameter(Pattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]string mail)
            {
                Values.Add(nameof(@minMax), @minMax);
                Values.Add(nameof(@multipleOf), @multipleOf);
                Values.Add(nameof(@exclusive), @exclusive);
                Values.Add(nameof(@string), @string);
                Values.Add(nameof(mail), @mail);
            }
            

            public void NullableWithNotAttribute(
                bool? @bool,
                byte? @byte,
                sbyte? @sbyte,
                short? @short,
                ushort? @ushort,
                int? @int,
                uint? @uint,
                long? @long,
                ulong? @ulong,
                float? @float,
                double? @double,
                decimal? @decimal,
                string? @string,
                DateTime? @dateTime,
                DateTimeOffset? @dateTimeOffset,
                TimeSpan? @timeSpan,
                Guid? @guid)
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
                Values.Add(nameof(@dateTimeOffset), @dateTimeOffset);
                Values.Add(nameof(@timeSpan), @timeSpan);
                Values.Add(nameof(@guid), @guid);
            }

            public void FromService([FromServices] IFoo foo)
            {
                Values.Add(nameof(foo), foo.Text);
            }
        }
        
        public class AsyncAction : Thing
        {
            public override string Name => "async-action";
            
            public List<string> Values { get; } = new List<string>();

            public async Task Execute()
            {
                await Task.Delay(1_000);
                Values.Add(nameof(Execute));
            }
            
            public async Task ExecuteWithCancellationToken(CancellationToken cancellation)
            {
                await Task.Delay(1_000, cancellation);
                Values.Add(nameof(ExecuteWithCancellationToken));
            }
            
            public async Task ExecuteToCancel(CancellationToken cancellation)
            {
                try
                {
                    await Task.Delay(3_000, cancellation).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Values.Add(nameof(ExecuteToCancel));
                }
            }
        }
        
        public interface IFoo
        {
            string Text { get; set; }
        }

        #endregion
    }
}
