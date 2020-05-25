using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Integration.Test.Action
{
    public abstract class AbstractActionTest<T>
    {
        protected IServiceProvider Provider { get; }
        protected IThingContextFactory Factory { get; }
        protected Fixture Fixture { get; }
        
        protected virtual void ConfigureServiceCollection(IServiceCollection collection) { }

        protected AbstractActionTest()
        {
            Fixture = new Fixture();
            
            var collection = new ServiceCollection();
            collection.AddThings();
            collection.AddLogging();
            
            ConfigureServiceCollection(collection);
            
            Provider = collection.BuildServiceProvider().CreateScope().ServiceProvider;
            Factory = Provider.GetRequiredService<IThingContextFactory>();
        }
        
        protected abstract JsonElement CreateJson(T value);
        
        protected abstract IEnumerable<JsonElement> CreateInvalidJson();
        
        protected virtual T CreateValue() => Fixture.Create<T>();
        
        protected void TestResponse<TThing>(string response)
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
