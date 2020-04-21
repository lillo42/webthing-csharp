using AutoFixture;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.String
{
    public class DateTimeProperty : AbstractStructPropertyTest<DateTime>
    {
        protected override JsonElement CreateJson(DateTime value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{value:O}"" }}")
                .GetProperty("input");
        }

        protected override JsonElement[] CreateInvalidJson()
        {
            var result = new List<JsonElement>
            {
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": ""{Fixture.Create<string>()}"" }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<bool>().ToString().ToLower()} }}")
                    .GetProperty("input"),
                JsonSerializer.Deserialize<JsonElement>($@"{{ ""input"": {Fixture.Create<int>()} }}")
                    .GetProperty("input")
            };
            
            return result.ToArray();
        }
    }
}
