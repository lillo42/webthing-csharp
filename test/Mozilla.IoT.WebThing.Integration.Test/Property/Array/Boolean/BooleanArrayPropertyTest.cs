using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.Array.Boolean
{
    public class BooleanArrayPropertyTest : AbstractArrayPropertyTest<bool>
    {
        protected override JsonElement CreateJson(IEnumerable<bool> values)
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

                sb.Append(value.ToString().ToLower());
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


            sb.Clear();
            
            #endregion

            #region Int
            var ints = Fixture.Create<int[]>();
            
            sb.Append("[");
            
            isFirst = true;
            
            foreach (var value in ints)
            {
                if (!isFirst)
                {
                    sb.Append(", ");
                }

                sb.Append(value);
                isFirst = false;
            }

            sb.Append("]");
            
            result.Add(JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"":  {sb} }}")
                .GetProperty("input"));
            #endregion


            #region Multi Value
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
            
            sb.Clear();

            #endregion
            
            return result.ToArray();
        }
    }
}
