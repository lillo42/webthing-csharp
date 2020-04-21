using System.Collections.Generic;
using AutoFixture;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.String
{
    public class StringProperty : AbstractClassPropertyTest<string>
    {
        protected override JsonElement CreateJson(string value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value}"" }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
    }
}
