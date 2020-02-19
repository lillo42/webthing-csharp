using System;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class EventThing : Thing
    {
        public EventThing()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(3_000).GetAwaiter().GetResult();
                    var @event = Overheated;
                    @event?.Invoke(this, 0);
                }
            });
            
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(4_000).GetAwaiter().GetResult();
                    var @event = OtherEvent;
                    @event?.Invoke(this, 1.ToString());
                }
            });
        }
        
        public override string Name => "event";
        
        [ThingEvent(Title = "Overheated", 
            Type = new [] {"OverheatedEvent"},
            Description = "The lamp has exceeded its safe operating temperature")]
        public event EventHandler<int> Overheated;
        
        [ThingEvent(Title = "OtherEvent")]
        public event EventHandler<string> OtherEvent;
    }
}