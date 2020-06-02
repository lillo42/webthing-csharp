using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Attributes;

namespace SingleThing.Things
{
    public class LampThing : Thing
    {
        public override string Name => "my-lamp-1234";
        public override string? Title => "My Lamp";
        public override string? Description => "A web connected lamp";
        public override string[]? Type { get; } = new[] { "Light", "OnOffSwitch" };

        private bool _on = true;
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

        private int _brightness = 50;
        [ThingProperty(Type = new[] {"BrightnessProperty"}, Title = "Brightness", Description = "The level of light from 0-100", Unit = "percent", Minimum = 0, Maximum = 100)]
        public int Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged();
            }
        }

        [ThingEvent(Title = "Overheated", Unit = "degree celsius", Type = new [] {"OverheatedEvent"}, Description = "The lamp has exceeded its safe operating temperature")]
        public event EventHandler<double>? Overheated;


        [ThingAction(Title = "Fade", Type = new []{"FadeAction"}, Description = "Fade the lamp to a given level")]
        public async Task Fade(
            [ThingParameter(Minimum = 0, Maximum = 100, Unit = "percent")]int brightness,
            [ThingParameter(Minimum = 1, Unit = "milliseconds")]int duration,
            [FromServices]ILogger<LampThing> logger)
        {
            await Task.Delay(duration);
            
            logger.LogInformation("Going to set Brightness to {brightness}", brightness);
            Brightness = brightness;
            
            logger.LogInformation("Going to send event Overheated");
            Overheated?.Invoke(this, 102);
        }
    }
}
