using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Extensions;
using Xunit;

namespace Mozilla.IoT.WebThing.Intregration.Test.Factories
{
    public class PropertyTest : IThingContextFactoryTest
    {
        

        #region Valid Property

        [Theory]
        [InlineData(typeof(bool[]))]
        [InlineData(typeof(Guid[]))]
        [InlineData(typeof(TimeSpan[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(DateTimeOffset[]))]
        [InlineData(typeof(Foo[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(sbyte[]))]
        [InlineData(typeof(short[]))]
        [InlineData(typeof(ushort[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(uint[]))]
        [InlineData(typeof(long[]))]
        [InlineData(typeof(ulong[]))]
        [InlineData(typeof(float[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(decimal[]))]
        public void ValidArray(Type type)
        {
            if (type == typeof(bool[]))
            {
                TestValidNullableProperty<bool[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString().ToLower());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #region String

            if (type == typeof(Guid[]))
            {
                TestValidNullableProperty<Guid[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(x[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(DateTime[]))
            {
                TestValidNullableProperty<DateTime[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(x[i].ToString("O")).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(DateTimeOffset[]))
            {
                TestValidNullableProperty<DateTimeOffset[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(x[i].ToString("O")).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(TimeSpan[]))
            {
                TestValidNullableProperty<TimeSpan[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(x[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(char[]))
            {
                TestValidNullableProperty<char[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(x[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(Foo[]))
            {
                TestValidNullableProperty<Foo[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(x[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(string[]))
            {
                TestValidNullableProperty<string[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(x[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #endregion

            #region Integer

            if (type == typeof(byte[]))
            {
                TestValidNullableProperty<byte[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(sbyte[]))
            {
                TestValidNullableProperty<sbyte[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(short[]))
            {
                TestValidNullableProperty<short[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(ushort[]))
            {
                TestValidNullableProperty<ushort[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(int))
            {
                TestValidNullableProperty<int[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(uint[]))
            {
                TestValidNullableProperty<uint[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(long[]))
            {
                TestValidNullableProperty<long[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(ulong[]))
            {
                TestValidNullableProperty<ulong[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #endregion

            #region Number

            if (type == typeof(float[]))
            {
                TestValidNullableProperty<float[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString(CultureInfo.InvariantCulture));
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(double[]))
            {
                TestValidNullableProperty<double[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString(CultureInfo.InvariantCulture));
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(decimal[]))
            {
                TestValidNullableProperty<decimal[]>(x =>
                {
                    var value = new StringBuilder();
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(x[i].ToString(CultureInfo.InvariantCulture));
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #endregion
        }
        
        [Theory]
        [InlineData(typeof(IEnumerable<bool>))]
        [InlineData(typeof(IEnumerable<Guid>))]
        [InlineData(typeof(IEnumerable<TimeSpan>))]
        [InlineData(typeof(IEnumerable<DateTime>))]
        [InlineData(typeof(IEnumerable<DateTimeOffset>))]
        [InlineData(typeof(IEnumerable<Foo>))]
        [InlineData(typeof(IEnumerable<char>))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(IEnumerable<byte>))]
        [InlineData(typeof(IEnumerable<sbyte>))]
        [InlineData(typeof(IEnumerable<short>))]
        [InlineData(typeof(IEnumerable<ushort>))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(IEnumerable<uint>))]
        [InlineData(typeof(IEnumerable<long>))]
        [InlineData(typeof(IEnumerable<ulong>))]
        [InlineData(typeof(IEnumerable<float>))]
        [InlineData(typeof(IEnumerable<double>))]
        [InlineData(typeof(IEnumerable<decimal>))]
        public void ValidIEnumerable(Type type)
        {
            type = type.GetCollectionType();
            if (type == typeof(bool))
            {
                TestValidNullableProperty<IEnumerable<bool>>(x =>
                {
                    var value = new StringBuilder();
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString().ToLower());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #region String

            if (type == typeof(Guid))
            {
                TestValidNullableProperty<IEnumerable<Guid>>(x =>
                {
                    var value = new StringBuilder();
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(values[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(DateTime))
            {
                TestValidNullableProperty<IEnumerable<DateTime>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(values[i].ToString("O")).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(DateTimeOffset))
            {
                TestValidNullableProperty<IEnumerable<DateTimeOffset>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(values[i].ToString("O")).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(TimeSpan[]))
            {
                TestValidNullableProperty<IEnumerable<TimeSpan>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(values[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(char[]))
            {
                TestValidNullableProperty<IEnumerable<char>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(values[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(Foo))
            {
                TestValidNullableProperty<IEnumerable<Foo>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(values[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(string))
            {
                TestValidNullableProperty<IEnumerable<string>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append("\"").Append(values[i].ToString()).Append("\"");
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #endregion

            #region Integer

            if (type == typeof(byte[]))
            {
                TestValidNullableProperty<IEnumerable<byte>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(sbyte[]))
            {
                TestValidNullableProperty<IEnumerable<sbyte>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(short[]))
            {
                TestValidNullableProperty<IEnumerable<short>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(ushort[]))
            {
                TestValidNullableProperty<IEnumerable<ushort>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(int))
            {
                TestValidNullableProperty<IEnumerable<int>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(uint[]))
            {
                TestValidNullableProperty<IEnumerable<uint>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(long[]))
            {
                TestValidNullableProperty<IEnumerable<long>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(ulong[]))
            {
                TestValidNullableProperty<IEnumerable<ulong>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString());
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #endregion

            #region Number

            if (type == typeof(float[]))
            {
                TestValidNullableProperty<IEnumerable<float>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString(CultureInfo.InvariantCulture));
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(double[]))
            {
                TestValidNullableProperty<IEnumerable<double>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString(CultureInfo.InvariantCulture));
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            if (type == typeof(decimal[]))
            {
                TestValidNullableProperty<IEnumerable<decimal>>(x =>
                {
                    var value = new StringBuilder();
                    
                    var values = x.ToArray();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (i != 0)
                        {
                            value.Append(", ");
                        }
                        
                        value.Append(values[i].ToString(CultureInfo.InvariantCulture));
                    }
                    
                    return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":[{value}] }}")
                        .GetProperty("input");
                });
                return;
            }

            #endregion
        }

        private void TestValidProperty<T>(Func<T, JsonElement> createJsonElement)
            where T : struct
        {
            var thing = new StructPropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(2);
            context.Properties.Should().ContainKey(nameof(StructPropertyThing<T>.Value));
            context.Properties.Should().ContainKey(nameof(StructPropertyThing<T>.NullableValue));

            var value = Fixture.Create<T>();
            var jsonElement = createJsonElement(value);
            
            context.Properties[nameof(StructPropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().Be(value);
            context.Properties[nameof(StructPropertyThing<T>.Value)].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().Be(value);
            
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.NullableValue.Should().Be(value);
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().Be(value);
            
            jsonElement =  JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.NullableValue.Should().BeNull();
            context.Properties[nameof(StructPropertyThing<T>.NullableValue)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().BeNull();
        }
        
        private void TestValidNullableProperty<T>(Func<T, JsonElement> createJsonElement)
        {
            var thing = new NullablePropertyThing<T>();
            var context = Factory.Create(thing, new ThingOption());
            
            thing.ThingContext = context;
            
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().HaveCount(1);
            context.Properties.Should().ContainKey(nameof(NullablePropertyThing<T>.Value));

            var value = Fixture.Create<T>();
            var jsonElement = createJsonElement(value);
            
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().BeEquivalentTo(value);
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TryGetValue(out var getValue).Should().BeTrue();
            getValue.Should().BeEquivalentTo(value);

            jsonElement =  JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            thing.Value.Should().BeNull();
            context.Properties[nameof(NullablePropertyThing<T>.Value)].TryGetValue(out getValue).Should().BeTrue();
            getValue.Should().BeNull();
        }

        #endregion

        public class StructPropertyThing<T> : Thing
            where T : struct
        {
            public override string Name => "property-thing";
            
            public T Value { get; set; }
            public T? NullableValue { get; set; }
        }
        
        public class NullablePropertyThing<T> : Thing
        {
            public override string Name => "property-thing";
            
            public T Value { get; set; }
        }

        public enum Foo
        {
            A,
            Bar,
            C
        }

        private const string RESPONSE_WITH_NULLABLE = @"{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""properties"": {{
    ""value"": {{
      ""type"": ""{0}"",
      ""isReadOnly"": false,
      {1}
      ""link"": [
        {{
          ""href"": ""/things/property-thing/properties/value"",
          ""rel"": ""property""
        }}
      ]
    }},
    ""nullableValue"": {{
      ""type"": ""{0}"",
      ""isReadOnly"": false,
      {1}
      ""link"": [
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
        
        private const string RESPONSE_WITHOUT_NULLABLE = @"{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""properties"": {{
    ""value"": {{
      ""type"": ""{0}"",
      ""isReadOnly"": false,
      {1}
      ""link"": [
        {{
          ""href"": ""/things/property-thing/properties/value"",
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
