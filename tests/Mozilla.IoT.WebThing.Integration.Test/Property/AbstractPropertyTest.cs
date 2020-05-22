using System.Text.Json;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Integration.Test.Property
{
    public abstract class AbstractPropertyTest<T>
    {
        protected IThingContextFactory Factory { get; }
        protected Fixture Fixture { get; }

        protected AbstractPropertyTest()
        {
            var collection = new ServiceCollection();
            collection.AddThings();
            var provider = collection.BuildServiceProvider();
            Factory = provider.GetRequiredService<IThingContextFactory>();
            Fixture = new Fixture();
        }

        protected abstract JsonElement CreateJson(T value);
        protected abstract JsonElement[] CreateInvalidJson();

        protected void TestResponseProperty<TThing>(string response)
            where TThing : Thing, new()
        {
            var thing = new TThing();
            var context = Factory.Create(thing, new ThingOption());

            var message = JsonSerializer.Serialize(context.Response,
                new ThingOption().ToJsonSerializerOptions());

            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(response));
        }
    }
}
