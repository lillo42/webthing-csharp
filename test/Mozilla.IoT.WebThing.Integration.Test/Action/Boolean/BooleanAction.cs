using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Integration.Test.Action.Boolean
{
    public class BooleanAction : AbstractStructActionTest<bool>
    {
        protected override JsonElement CreateJson(bool value)
        {
            return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {value.ToString().ToLower()} }} }} }}").GetProperty("action");
        }

        protected override IEnumerable<JsonElement> CreateInvalidJson()
        {
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": ""{Fixture.Create<string>()}"" }} }} }}").GetProperty("action");
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<int>()} }} }} }}").GetProperty("action");
            
            yield return JsonSerializer.Deserialize<JsonElement>(
                $@"{{ ""action"": {{ ""input"": {{ ""value"": [ {Fixture.Create<bool>().ToString().ToLower()} ] }} }} }}").GetProperty("action");
        }
    }
}
