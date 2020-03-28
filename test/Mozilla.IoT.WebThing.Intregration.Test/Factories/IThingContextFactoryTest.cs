using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Factories;

namespace Mozilla.IoT.WebThing.Intregration.Test.Factories
{
    public class IThingContextFactoryTest
    {
        protected IThingContextFactory Factory { get; }
        protected Fixture Fixture { get; }

        public IThingContextFactoryTest()
        {
            var collection = new ServiceCollection();
            collection.AddThings();
            var provider = collection.BuildServiceProvider();
            Factory = provider.GetRequiredService<IThingContextFactory>();
            Fixture = new Fixture();
        }
    }
}
