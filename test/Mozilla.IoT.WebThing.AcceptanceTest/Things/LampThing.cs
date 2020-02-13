using System;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class LampThing : Thing
    {
        private int _counter = 0;
        public LampThing()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(3_000).GetAwaiter().GetResult();
                    var @event = Overheated;
                    @event?.Invoke(this, _counter++);
                }
            });
            
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(4_000).GetAwaiter().GetResult();
                    var @event = OtherEvent;
                    @event?.Invoke(this, _counter.ToString());
                }
            });
        }
        public override string Name => "Lamp";
        public override string Title => "My Lamp";
        public override string Description => "A web connected lamp";
        public override string[] Type { get; } = new[] { "Light", "OnOffSwitch" };

        private bool _on;
        [ThingProperty(Type = new[] {"OnOffProperty"}, Title = "On/Off", Description = "Whether the lamp is turned on")]
        public bool On
        {
            get => _on;
            set
            {
                _on = value;
                OnPropertyChanged();
            }
        }

        private int _brightness;

        [ThingProperty(Type = new[] {"BrightnessProperty"}, Title = "Brightness",
            Description = "The level of light from 0-100", Minimum = 0, Maximum = 100)]
        public int Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged();
            }
        }

        [ThingEvent(Title = "Overheated", 
            Type = new [] {"OverheatedEvent"},
            Description = "The lamp has exceeded its safe operating temperature")]
        public event EventHandler<int> Overheated;
        
        [ThingEvent(Title = "OtherEvent")]
        public event EventHandler<string> OtherEvent;
        
        [ThingAction(Name = "fade", Title = "Fade", Type = new []{"FadeAction"},
            Description = "Fade the lamp to a given level")]
        public void Fade(
            [ThingParameter(Minimum = 0, Maximum = 100)]int level,
            [ThingParameter(Minimum = 0, Unit = "milliseconds")]int duration)
        {
            
        }
        
        public Task LongRun(CancellationToken cancellationToken)
        {
            return Task.Delay(3_000, cancellationToken);
        }
    }
}
