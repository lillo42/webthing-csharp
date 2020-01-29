using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Attributes;

namespace SampleThing.Things
{
    public class LampThing : Thing
    {
        public LampThing()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(3_000).GetAwaiter().GetResult();
                    var @event = Overheated;
                    try
                    {
                        @event?.Invoke(this, 10);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }
        public override string Name => "Lamp";
        public override string? Title => "My Lamp";
        public override string? Description => "A web connected lamp";
        public override string[]? Type { get; } = new[] { "Light", "OnOffSwitch" };

        [ThingProperty(Type = new []{ "OnOffProperty" }, Title = "On/Off", Description = "Whether the lamp is turned on")]
        public bool On { get; set; }
        
        [ThingProperty(Type = new []{ "BrightnessProperty" },Title = "Brightness",
            Description = "The level of light from 0-100", Minimum = 0, Maximum = 100)]
        public int Brightness { get; set; }

        [ThingEvent(Title = "Overheated", 
            Type = new [] {"OverheatedEvent"},
            Description = "The lamp has exceeded its safe operating temperature")]
        public event EventHandler<double> Overheated;


        [ThingAction(Title = "Fade", Type = new []{"FadeAction"},
            Description = "Fade the lamp to a given level")]
        public void Fade(
            [ThingParameter(Minimum = 0, Maximum = 100)]int level,
            [ThingParameter(Minimum = 0, Unit = "milliseconds")]int duration)
        {
            Console.WriteLine("Fade executed....");
        }
    }
}
