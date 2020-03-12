using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

        [Theory]
        [ClassData(typeof(SyncNonNullableInvalidType))]
        public void CallActionSyncNoNullableInvalidType(object @bool, object @byte, object @sbyte, object @short,
            object @ushort, object @int, object @uint, object @long, object @ulong, object @float, object @double,
            object @decimal, object @string, object dateTime, object dateTimeOffset, object guid, object timeSpan)
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableNotAttribute));


            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
                        ""bool"": {@bool},
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
                        ""string"": {@string},
                        ""dateTime"": {@dateTime},
                        ""dateTimeOffset"": {@dateTimeOffset},
                        ""guid"": {@guid},
                        ""timeSpan"": {@timeSpan}
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableNotAttribute)].TryAdd(json, out var action).Should().BeFalse();
            action.Should().BeNull();
            thing.Values.Should().BeEmpty();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CallActionSyncNoNullableWithValidationValid(bool isMin)
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableAttribute));

            var @byte = isMin ? (byte)2 : (byte)100;
            var @sbyte =  isMin ? (sbyte)2 : (sbyte)100;
            var @short = isMin ? (short)2 : (short)100;
            var @ushort = isMin ? (ushort)2 : (ushort)100;
            var @int = isMin ? 2 : 100;
            var @uint = isMin ? 2 : (uint)100;
            var @long = isMin ? 2 : (long)100;
            var @ulong = isMin ? 2 : (ulong)100;
            var @float = isMin ? 2 : (float)100;
            var @double = isMin ? 2 : (double)100;
            var @decimal = isMin ? 2 : (decimal)100;

            var @string = _fixture.Create<string>();
            var mail = "test@test.com";


            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
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
                        ""mail"": ""{mail}""
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableAttribute)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(13);
            thing.Values.Should().BeEquivalentTo(new Dictionary<string, object>
            {
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
                [nameof(@mail)] = @mail
            });
        }

        [Theory]
        [ClassData(typeof(SyncNonNullableAttributeInvalidType))]
        public void CallActionSyncNoNullableWithValidationInvalid(byte @byte, sbyte @sbyte, short @short, ushort @ushort, 
            int @int, uint @uint, long @long, ulong @ulong, float @float, double @double, decimal @decimal,
            string @string, string @mail)
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableAttribute));
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
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

        [Theory]
        [ClassData(typeof(SyncNullableInvalidType))]
        public void CallActionSyncNullableInvalidType(object[] values)
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableNotAttribute));


            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
                        ""bool"": {values[0]},
                        ""byte"": {values[1]},
                        ""sbyte"": {values[2]},
                        ""short"": {values[3]},
                        ""ushort"": {values[4]},
                        ""int"": {values[5]},
                        ""uint"": {values[6]},
                        ""long"": {values[7]},
                        ""ulong"": {values[8]},
                        ""float"": {values[9]},
                        ""double"": {values[10]},
                        ""decimal"": {values[11]},
                        ""string"": {values[12]},
                        ""dateTime"": {values[13]},
                        ""dateTimeOffset"": {values[14]},
                        ""guid"": {values[15]},
                        ""timeSpan"": {values[16]}
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableNotAttribute)].TryAdd(json, out var action).Should().BeFalse();
            action.Should().BeNull();
            thing.Values.Should().BeEmpty();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CallActionSyncNoNullableExclusiveWithValidationValid(bool isMin)
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableAttributeExclusive));

            var @byte = isMin ? (byte)2 : (byte)99;
            var @sbyte =  isMin ? (sbyte)2 : (sbyte)99;
            var @short = isMin ? (short)2 : (short)99;
            var @ushort = isMin ? (ushort)2 : (ushort)99;
            var @int = isMin ? 2 : 99;
            var @uint = isMin ? 2 : (uint)99;
            var @long = isMin ? 2 : (long)99;
            var @ulong = isMin ? 2 : (ulong)99;
            var @float = isMin ? 2 : (float)99;
            var @double = isMin ? 2 : (double)99;
            var @decimal = isMin ? 2 : (decimal)99;

            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
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
                        ""decimal"": {@decimal}
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableAttributeExclusive)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            thing.Values.Should().NotBeEmpty();
            thing.Values.Should().HaveCount(11);
            thing.Values.Should().BeEquivalentTo(new Dictionary<string, object>
            {
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
            });
        }

        [Theory]
        [ClassData(typeof(SyncNonNullableAttributeExclusiveInvalidType))]
        public void CallActionSyncNoNullableExclusiveWithValidationInvalid(byte @byte, sbyte @sbyte, short @short,
            ushort @ushort, int @int, uint @uint, long @long, ulong @ulong, float @float, double @double, decimal @decimal)
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.NoNullableAttributeExclusive));

            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                    ""input"": {{
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
                        ""decimal"": {@decimal}
                }} 
            }}");

            _factory.Actions[nameof(SyncAction.NoNullableAttributeExclusive)].TryAdd(json, out var action).Should().BeFalse();
            action.Should().BeNull();
        }

        [Fact]
        public void FromService()
        {
            var thing = new SyncAction();
            CodeGeneratorFactory.Generate(thing, new[] { _factory });
            _factory.Actions.Should().ContainKey(nameof(SyncAction.FromService));

            var json = JsonSerializer.Deserialize<JsonElement>(@"{{ ""input"": {{ }} }}");

            var foo = Substitute.For<IFoo>();
            var fooText = _fixture.Create<string>();
            foo.Text.Returns(fooText);

            _provider.GetService(typeof(IFoo))
                .Returns(foo);

            _factory.Actions[nameof(SyncAction.FromService)].TryAdd(json, out var action).Should().BeTrue();
            action.Should().NotBeNull();
            var result = action.ExecuteAsync(thing, _provider);
            result.IsCompleted.Should().BeTrue();
            action.Status.Should().Be(Status.Completed);
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
            action.Status.Should().Be(Status.Executing);
            await result;
            action.Status.Should().Be(Status.Completed);
            
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
            action.Status.Should().Be(Status.Executing);
            result.IsCompleted.Should().BeFalse();
            await result;
            action.Status.Should().Be(Status.Completed);
            
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
            action.Status.Should().Be(Status.Executing);
            result.IsCompleted.Should().BeFalse();
            action.Cancel();
            await result;
            action.Status.Should().Be(Status.Completed);
            
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
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]byte @byte,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]sbyte @sbyte,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]short @short,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]ushort @ushort,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]int @int,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]uint @uint,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]long @long,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]ulong @ulong,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]float @float,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]double @double,
                [ThingParameter(Minimum = 2, Maximum = 100, MultipleOf = 2)]decimal @decimal,
                [ThingParameter(MinimumLength = 1, MaximumLength = 40)]string @string,
                [ThingParameter(Pattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]string mail)
            {
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
                Values.Add(nameof(mail), @mail);
            }
            
            public void NoNullableAttributeExclusive(
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]byte @byte,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]sbyte @sbyte,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]short @short,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]ushort @ushort,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]int @int,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]uint @uint,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]long @long,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]ulong @ulong,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]float @float,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]double @double,
                [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]decimal @decimal)
            {
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

        #region Data Generator

        public class SyncNonNullableInvalidType : IEnumerable<object[]>
        {
            private readonly Fixture _fixture = new Fixture();
            public IEnumerator<object[]> GetEnumerator()
            {
                var right = new object[]
                {
                    _fixture.Create<bool>().ToString().ToLower(), _fixture.Create<byte>(), _fixture.Create<sbyte>(),
                    _fixture.Create<short>(), _fixture.Create<ushort>(), _fixture.Create<int>(),
                    _fixture.Create<uint>(), _fixture.Create<long>(), _fixture.Create<ulong>(),
                    _fixture.Create<float>(), _fixture.Create<double>(), _fixture.Create<decimal>(),
                    $@"""{_fixture.Create<string>()}""", $@"""{_fixture.Create<DateTime>():O}""",
                    $@"""{_fixture.Create<DateTimeOffset>():O}""", $@"""{_fixture.Create<Guid>()}""",
                    $@"""{_fixture.Create<TimeSpan>()}"""
                };
                
                for (var i = 0; i < 17; i++)
                {
                    var result = new object[right.Length];
                    Array.Copy(right, 0, result, 0, right.Length);

                    if (i >= 12)
                    {
                        result[i] = _fixture.Create<int>();
                    }
                    else
                    {
                        result[i] = $@"""{_fixture.Create<string>()}""";
                    }

                    yield return result;
                }

                for (var i = 0; i < 17; i++)
                {
                    var result = new object[right.Length];
                    Array.Copy(right, 0, result, 0, right.Length);

                    if (i == 12)
                    {
                        continue;
                    }

                    result[i] = "null";

                    yield return result;
                }

                for (var i = 13; i < 17; i++)
                {
                    var result = new object[right.Length];
                    Array.Copy(right, 0, result, 0, right.Length);

                    result[i] = $@"""{_fixture.Create<bool>()}""";
                    
                    yield return result;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
        
        public class SyncNonNullableAttributeInvalidType : IEnumerable<object[]>
        {
            private readonly Fixture _fixture = new Fixture();
            public IEnumerator<object[]> GetEnumerator()
            {
                var right = new object[]
                {
                    (byte)10, (sbyte)10, (short)10, (ushort)10, (int)10, (uint)10, (long)10, (ulong)10,
                    (float)10, (double)10, (decimal)10,
                    _fixture.Create<string>(), "test@test.com"
                };

                object[] result = null;
                for (var i = 0; i < 11; i++)
                {
                    result = new object[right.Length];
                    Array.Copy(right, 0, result, 0, right.Length);
                    result[i] = 0;
                    yield return result;
                    
                    result[i] = 101 + i;
                    yield return result;
                    
                    result[i] = i;
                    if (i % 2 == 0)
                    {
                        result[i] = i + 1;
                    }
                    yield return result;
                }


                result = new object[right.Length];
                Array.Copy(right, 0, result, 0, right.Length);
                result[11] = string.Empty;
                yield return result;
                result[11] = _fixture.Create<string>() + _fixture.Create<string>();
                yield return result;
                result = new object[right.Length];
                Array.Copy(right, 0, result, 0, right.Length);
                result[12] = string.Empty;
                yield return result;
                result[12] = _fixture.Create<string>();
                yield return result;
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
        
        public class SyncNonNullableAttributeExclusiveInvalidType : IEnumerable<object[]>
        {
            private readonly Fixture _fixture = new Fixture();
            public IEnumerator<object[]> GetEnumerator()
            {
                var right = new object[]
                {
                    (byte)2, (sbyte)3, (short)4, (ushort)5, (int)6, (uint)7, (long)8, (ulong)9,
                    (float)10, (double)11, (decimal)12
                };

                for (var i = 0; i < right.Length; i++)
                {
                    var result = new object[right.Length];
                    Array.Copy(right, 0, result, 0, right.Length);
                    result[i] = 1;
                    yield return result;
                    
                    result[i] = 100;
                    yield return result;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }

        public class SyncNullableInvalidType : IEnumerable<object[]>
        {
            private readonly Fixture _fixture = new Fixture();
            public IEnumerator<object[]> GetEnumerator()
            {
                var right = new object[]
                {
                    _fixture.Create<bool>().ToString().ToLower(), _fixture.Create<byte>(), _fixture.Create<sbyte>(),
                    _fixture.Create<short>(), _fixture.Create<ushort>(), _fixture.Create<int>(),
                    _fixture.Create<uint>(), _fixture.Create<long>(), _fixture.Create<ulong>(),
                    _fixture.Create<float>(), _fixture.Create<double>(), _fixture.Create<decimal>(),
                    $@"""{_fixture.Create<string>()}""", $@"""{_fixture.Create<DateTime>():O}""",
                    $@"""{_fixture.Create<DateTimeOffset>():O}""", $@"""{_fixture.Create<Guid>()}""",
                    $@"""{_fixture.Create<TimeSpan>()}"""
                };


                for (var i = 0; i < 17; i++)
                {
                    var result = new object[right.Length];
                    Array.Copy(right, 0, result, 0, right.Length);

                    if (i >= 12)
                    {
                        result[i] = _fixture.Create<int>();
                    }
                    else
                    {
                        result[i] = $@"""{_fixture.Create<string>()}""";
                    }

                    yield return new object[]
                    {
                        result
                    };
                }

                for (var i = 13; i < 17; i++)
                {
                    var result = new object[right.Length];
                    Array.Copy(right, 0, result, 0, right.Length);

                    result[i] = $@"""{_fixture.Create<bool>()}""";

                    yield return new object[]
                    {
                        result
                    };
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }

        #endregion
    }
}
