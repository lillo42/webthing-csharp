using System.Collections.Generic;
using AutoFixture;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.Number
{
    public abstract class AbstractNumberPropertyTest<T> : AbstractStructPropertyTest<T>
        where T : struct
    {
        protected override JsonElement CreateJson(T value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value} }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
        
    }
    
    public class ByteProperty : AbstractNumberPropertyTest<byte> { }
    public class SByteProperty : AbstractNumberPropertyTest<sbyte> { }
    
    public class ShortProperty : AbstractNumberPropertyTest<short> { }
    public class UShortProperty : AbstractNumberPropertyTest<ushort> { }
    
    public class IntProperty : AbstractNumberPropertyTest<int> { }
    public class UIntProperty : AbstractNumberPropertyTest<uint> { }
    
    public class LongProperty : AbstractNumberPropertyTest<long> { }
    public class ULongProperty : AbstractNumberPropertyTest<ulong> { }
    
    public class FloatProperty : AbstractNumberPropertyTest<float> { }
    public class DoubleProperty : AbstractNumberPropertyTest<double> { }
    public class DecimalProperty : AbstractNumberPropertyTest<decimal> { }
}
