using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.Array.Number
{
    public abstract class NumberArrayProperty<T> : AbstractArrayPropertyTest<T>
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
            
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {sb} }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>();

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
            
            result.Add(JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":  {sb} }}")
                .GetProperty("input"));

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
            
            result.Add(JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":  {sb} }}")
                .GetProperty("input"));

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
            
            result.Add(JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":  {sb} }}")
                .GetProperty("input"));

            #endregion
            
            return result.ToArray();
        }
    }
    
    public class ByteArrayProperty : NumberArrayProperty<byte> { }
    
    public class SByteArrayProperty : NumberArrayProperty<sbyte> { }
    
    public class ShortArrayProperty : NumberArrayProperty<short> { }
    
    public class UShortArrayProperty : NumberArrayProperty<ushort> { }
    
    public class IntArrayProperty : NumberArrayProperty<int> { }
    
    public class UIntArrayProperty : NumberArrayProperty<uint> { }
    
    public class LongArrayProperty : NumberArrayProperty<long> { }
    
    public class ULongArrayProperty : NumberArrayProperty<ulong> { }
    
    public class FloatArrayProperty : NumberArrayProperty<float> { }
    
    public class DoubleArrayProperty : NumberArrayProperty<double> { }
    
    public class DecimalArrayProperty : NumberArrayProperty<decimal> { }
}
