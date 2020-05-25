using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Integration.Test.Action.Array.String
{
    public class CharArrayActionTest : AbstractArrayActionTest<char>
    {
        protected override List<char> CreateValue(int arrayLength)
        {
            var values = new List<char>(arrayLength);

            for (var i = 0; i < arrayLength; i++)
            {
                values.Add(Fixture.Create<string>()[0]);
            }

            return values;
        }

        protected override JsonElement CreateJson(IEnumerable<char> values)
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

                sb.Append(@"""").Append(value).Append(@"""");
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

            #region Int
            sb.Clear();
            
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
}
