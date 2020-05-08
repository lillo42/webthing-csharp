using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Integration.Test.Action.Array.Number
{
    public abstract class NumberArrayActionTest<T> : AbstractArrayActionTest<T>
    {
        protected override JsonElement CreateJson(IEnumerable<T> values)
        {
            var sb = new StringBuilder();

            sb.Append("[");
            
            var isFirst = true;
            
            foreach (var value in values)
            {
                if (!isFirst)
                {
                    sb.Append(", ");
                }

                sb.Append(value);
                isFirst = false;
            }

            sb.Append("]");
            
            return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {sb} }} }} }}").GetProperty("action");
        }

        protected override IEnumerable<JsonElement> CreateInvalidJson()
        {
            #region String
            
            var strings = Fixture.Create<string[]>();
            
            var sb = new StringBuilder();

            sb.Append("[");
            
            var isFirst = true;
            
            foreach (var value in strings)
            {
                if (!isFirst)
                {
                    sb.Append(", ");
                }

                sb.Append("\"").Append(value).Append("\"");
                isFirst = false;
            }

            sb.Append("]");
            
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {sb} }} }} }}").GetProperty("action");
            

            #endregion

            #region bool
            sb.Clear();
            var bools = Fixture.Create<bool[]>();
            
            sb.Append("[");
            
            isFirst = true;
            
            foreach (var value in bools)
            {
                if (!isFirst)
                {
                    sb.Append(", ");
                }

                sb.Append(value.ToString().ToLower());
                isFirst = false;
            }

            sb.Append("]");
            
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {sb} }} }} }}").GetProperty("action");

            #endregion
            
            #region Multi Value
            sb.Clear();
            sb = new StringBuilder();

            sb.Append("[")
                .Append(Fixture.Create<bool>().ToString().ToLower())
                .Append(", ")
                .Append(Fixture.Create<int>())
                .Append(", ")
                .Append("\"").Append(Fixture.Create<string>()).Append("\"");
            
            sb.Append("]");
            
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {sb} }} }} }}").GetProperty("action");

            #endregion

        }
    }
    
    public class ByteArrayProperty : NumberArrayActionTest<byte> { }
    
    public class SByteArrayProperty : NumberArrayActionTest<sbyte> { }
    
    public class ShortArrayProperty : NumberArrayActionTest<short> { }
    
    public class UShortArrayProperty : NumberArrayActionTest<ushort> { }
    
    public class IntArrayProperty : NumberArrayActionTest<int> { }
    
    public class UIntArrayProperty : NumberArrayActionTest<uint> { }
    
    public class LongArrayProperty : NumberArrayActionTest<long> { }
    
    public class ULongArrayProperty : NumberArrayActionTest<ulong> { }
    
    public class FloatArrayProperty : NumberArrayActionTest<float> { }
    
    public class DoubleArrayProperty : NumberArrayActionTest<double> { }
    
    public class DecimalArrayProperty : NumberArrayActionTest<decimal> { }
}
