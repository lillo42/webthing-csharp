using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Integration.Test.Action.String
{
    public class EnumActionTest : AbstractStructActionTest<Foo>
    {
        protected override JsonElement CreateJson(String.Foo value)
        {
            return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{value}"" }} }} }}")
                .GetProperty("action");
        }

        protected override IEnumerable<JsonElement> CreateInvalidJson()
        {
            yield return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": ""{Fixture.Create<string>()}"" }} }} }}")
                .GetProperty("action");
            
            yield return JsonSerializer
                .Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<bool>().ToString().ToLower()} }} }} }}")
                .GetProperty("action");
            
            yield return JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": {Fixture.Create<int>()} }} }} }}")
                .GetProperty("action");
        }
    }
    
    public enum Foo
    {
        A,
        B,
        C
    }
}
