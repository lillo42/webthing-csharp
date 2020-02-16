using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class PropertyThing : Thing
    {
        public override string Name => "Property";
        
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
        
        public int Reader => _brightness;
    }
}
