using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Attributes;

namespace SampleThing.Things
{
    public class LampThing : Thing
    {
        public override string Name => "Lamp";
        public override string? Title => "My Lamp";
        public override string? Description => "A web connected lamp";
        public override string[]? Type { get; } = new[] { "Light", "OnOffSwitch" };

        [ThingProperty(Type = new []{ "OnOffProperty" }, Title = "On/Off", Description = "Whether the lamp is turned on")]
        public bool On { get; set; }
        
        [ThingProperty(Type = new []{ "BrightnessProperty" },Title = "Brightness",
            Description = "The level of light from 0-100", Minimum = 0, Maximum = 100)]
        public int Brightness { get; set; }
    }
}
