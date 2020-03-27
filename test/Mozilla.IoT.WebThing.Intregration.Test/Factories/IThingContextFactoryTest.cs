using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Xunit;

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
