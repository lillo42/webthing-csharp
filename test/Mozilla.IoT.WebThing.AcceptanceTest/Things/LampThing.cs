using System;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class LampThing : Thing
    {
        public override string Name => "lamp";
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
