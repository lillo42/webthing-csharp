using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Json;
using Action = Mozilla.IoT.WebThing.Action;

namespace Multi.Things
{
    public class ExampleDimmableLight : Thing
    {
        public ExampleDimmableLight()
        {
            Name = "My Lamp";
            Type = new[] {"OnOffSwitch", "Light"};
            Description = "A web connected lamp"; 
            
            
            var onDescription = new Dictionary<string, object>
            {
                ["@type"] = "OnOffProperty",
                ["title"] = "On/Off",
                ["type"] = "boolean",
                ["description"] = "Whether the lamp is turned on"
            };

            var on = new Property(this, "on", true, onDescription);
            on.ValuedChanged += (sender, args) =>
            {
                Console.WriteLine($"On-State is now {args.Value}");
            };
    
            Properties.Add(on);

            var brightnessDescription = new Dictionary<string, object>
            {
                ["@type"] = "BrightnessProperty",
                ["title"] = "Brightness",
                ["type"] = "integer",
                ["description"] = "The level of light from 0-100",
                ["minimum"] = 0,
                ["maximum"] = 100,
                ["unit"] = "percent"
            };
            
            var brightness  = new Property<double>(this, "brightness", 50, brightnessDescription);
            brightness.ValuedChanged += (sender, args) =>
            {
                Console.WriteLine($"Brightness is now {args.Value}");
            };
            
            Properties.Add(brightness);


            var fadeMetadata = new Dictionary<string, object>
            {
                ["title"] = "Fade", 
                ["description"] = "Fade the lamp to a given level"
            };
            
            var fadeInAdd = new  Dictionary<string, object>
            {
                ["type"] = "object", 
                ["required"] = new []{"brightness", "duration"}
            };

            var fadeBrightness = new  Dictionary<string, object>
            {
                ["type"] = "integer", 
                ["minimum"] = 0, 
                ["maximum"] = 100, 
                ["unit"] = "percent"
            };

            var fadeDuration = new  Dictionary<string, object>
            {
                ["type"] = "integer", 
                ["minimum"] = 1, 
                ["unit"] = "milliseconds"
            };

            var fadeProperties = new  Dictionary<string, object>
            {
                ["brightness"] = fadeBrightness, 
                ["duration"] = fadeDuration
            };
            
            fadeInAdd.Add("properties", fadeProperties);
            fadeMetadata.Add("inAdd", fadeInAdd);
            
            AddAction<FadeAction>("fade", fadeMetadata);
        }
        
        public class OverheatedEvent : Event
        {
            public OverheatedEvent(Thing thing, int data) 
                : base(thing, "overheated", data)
            {
                
            }
        }
        
        public class FadeAction : Action
        {
            public override string Name => "fade";
            
            protected override async Task ExecuteAsync(CancellationToken cancellation)
            {
                int value = Input["input"] as int? ?? 0;
                await Task.Delay(value, cancellation);
                
                var property = Thing.Properties.FirstOrDefault(x => x.Name == "brightness");
                if (property != null)
                {
                    property.Value = value;
                }
                
                Thing.Events.Add(new OverheatedEvent(Thing, 102));
            }
        }
    }
}
