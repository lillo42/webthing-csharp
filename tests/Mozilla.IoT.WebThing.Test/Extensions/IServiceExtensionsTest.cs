using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.WebSockets;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Extensions
{
    public class IServiceExtensionsTest
    {
        [Fact]
        public void CheckIfTheDependencyInjectionIsCorrect()
        {
            var service = new ServiceCollection();

            service.AddThings()
                .AddThing<FakeThing>();

            service.AddLogging();

            service.AddWebSockets(opt => { });

            var provider = service.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

             var scope = provider.CreateScope().ServiceProvider;
             _ = scope.GetRequiredService<FakeThing>();
             
             _ = scope.GetRequiredService<IPropertyFactory>();
             _ = scope.GetRequiredService<IJsonConvert>();
             _ = scope.GetRequiredService<IJsonSchemaValidationFactory>();
             _ = scope.GetRequiredService<IJsonConvertibleFactory>();
             _ = scope.GetRequiredService<IConvertibleFactory>();
             _ = scope.GetRequiredService<IThingContextFactory>();
             _ = scope.GetRequiredService<IThingResponseBuilder>();
             _ = scope.GetRequiredService<IEventBuilder>();
             _ = scope.GetRequiredService<IActionBuilder>();
             _ = scope.GetRequiredService<IPropertyBuilder>();
             _ = scope.GetRequiredService<IEnumerable<IWebSocketAction>>();
        }
        
        public class FakeThing : Thing
        {
            public override string Name => "test";
            
            public int Value { get; set; }

            public event EventHandler<int> SomeThing;

            public void Emit()
            {
                SomeThing?.Invoke(this, 1);
            }
        }
    }
}
