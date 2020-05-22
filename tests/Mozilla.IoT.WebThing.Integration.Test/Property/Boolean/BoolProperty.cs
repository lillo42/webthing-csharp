using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.Boolean
{
    public class BoolProperty : AbstractStructPropertyTest<bool>
    {
        protected override JsonElement CreateJson(bool value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {value.ToString().ToLower()} }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
    }
}
