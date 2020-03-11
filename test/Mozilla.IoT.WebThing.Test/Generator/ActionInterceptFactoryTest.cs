using System;
using System.Collections;
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
        public void CallActionSyncNoNullableInvalidType(object[] values)
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
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]byte @byte,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]sbyte @sbyte,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]short @short,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]ushort @ushort,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]int @int,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]uint @uint,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]long @long,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]ulong @ulong,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]float @float,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]double @double,
                [ThingParameter(Minimum = 0, Maximum = 100, MultipleOf = 2)]decimal @decimal,
                [ThingParameter(MinimumLength = 1, MaximumLength = 36)]string @string,
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

                    yield return new object[]
                    {
                        result
                    };
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
