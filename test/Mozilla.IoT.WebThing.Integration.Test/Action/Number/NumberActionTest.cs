using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;

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
