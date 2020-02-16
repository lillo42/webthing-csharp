using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Attributes;

namespace MultiThing.Things
{
    public class ExampleDimmableLight : Thing
    {
        private readonly ILogger<ExampleDimmableLight> _logger;
        
        public ExampleDimmableLight(ILogger<ExampleDimmableLight> logger)
        {
            _logger = logger;
        }
        
        public override string Name => "my-lamp-1234";

        public override string Title => "My Lamp";

        public override string[] Type { get; } = new[] {"OnOffSwitch", "Light"};

        public override string Description => "A web connected lamp";

        private bool _on = true;

        [ThingProperty(Title = "On/Off", Type = new []{ "OnOffProperty" }, Description = "Whether the lamp is turned on")]
        public bool On
        {
            get => _on;
            set
            {
                _on = value;
                
                _logger.LogInformation("On-State is now {on}", _on);
                OnPropertyChanged();
            }
        }

        private int _brightness = 50;


        [ThingProperty(Type = new []{ "BrightnessProperty" }, Title = "Brightness", Description = "The level of light from 0-100",
            Minimum = 0, Maximum = 100, Unit = "percent")]
        public int Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                _logger.LogInformation("Brightness is now {brightness}", _brightness);
                OnPropertyChanged();
            }
        }


        [ThingAction(Title = "Fade", Description = "Fade the lamp to a given level")]
        public async Task Fade(
            [ThingParameter(Minimum = 0, Maximum = 100, Unit = "percent")]int brightness,
            [ThingParameter(Minimum = 1, Unit = "milliseconds")]int duration,
            [FromServices]ILogger<ExampleDimmableLight> logger)
        {
            await Task.Delay(duration);
            
            logger.LogInformation("Going to set Brightness to {brightness}", brightness);
            Brightness = brightness;
            
            logger.LogInformation("Going to send event OverheatedEvent");
            OverheatedEvent?.Invoke(this, 102);
        }

        [ThingEvent(Description = "The lamp has exceeded its safe operating temperature", Unit = "degree celsius")]
        public event EventHandler<int> OverheatedEvent;
    }
}
