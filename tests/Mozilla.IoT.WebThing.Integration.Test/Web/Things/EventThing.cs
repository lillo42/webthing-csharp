using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Hosting;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Integration.Test.Web.Things
{
    public class EventThing : Thing
    {
        public override string Name => "event-thing";

        public event EventHandler<int> Level;

        [ThingEvent(Name = "info")] 
        public event EventHandler<string> Foo;

        internal void Invoke(int level)
        {
            Level?.Invoke(this, level);
        }
        
        internal void Invoke(string foo)
        {
            Foo?.Invoke(this, foo);
        }
    }


    public class WebSocketEventThing : EventThing
    {
        public override string Name => "web-socket-event-thing";
    }


    public class FireEventService : BackgroundService
    {
        private readonly Fixture _fixture;
        private readonly EventThing _http;
        private readonly WebSocketEventThing _socket;
        public FireEventService(EventThing thing, WebSocketEventThing socket)
        {
            _http = thing ?? throw new ArgumentNullException(nameof(thing));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _fixture = new Fixture();
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                _http.Invoke(_fixture.Create<int>());
                _socket.Invoke(_fixture.Create<int>());
                
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                _http.Invoke(_fixture.Create<string>());
                _socket.Invoke(_fixture.Create<string>());
            }
        }
    }

}
