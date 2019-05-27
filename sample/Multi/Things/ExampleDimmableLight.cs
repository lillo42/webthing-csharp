using System;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing;
using Newtonsoft.Json.Linq;
using Action = Mozilla.IoT.WebThing.Action;

namespace Multi.Things
{
    public class ExampleDimmableLight : Thing
    {
        public ExampleDimmableLight() 
            : base("My Lamp", new JArray("OnOffSwitch", "Light"),
                "A web connected lamp")
        
        {
            var onDescription = new JObject
            {
                {"@type", "OnOffProperty"},
                {"title", "On/Off"},
                {"type", "boolean"},
                {"description", "Whether the lamp is turned on"}
            };

            var on = new Property(this, "on", true, onDescription);
            on.ValuedChanged += (sender, args) =>
            {
                Console.WriteLine($"On-State is now {args.Value}");
            };
    
            AddProperty(on);

            var brightnessDescription = new JObject
            {
                {"@type", "BrightnessProperty"},
                {"title", "Brightness"},
                {"type", "integer"},
                {"description", "The level of light from 0-100"},
                {"minimum", 0},
                {"maximum", 100},
                {"unit", "percent"}
            };
            
            var brightness  = new Property<double>(this, "brightness", 50, brightnessDescription);
            brightness.ValuedChanged += (sender, args) =>
            {
                Console.WriteLine($"Brightness is now {args.Value}");
            };
            
            AddProperty(brightness);


            var fadeMetadata = new JObject
            {
                {"title", "Fade"}, 
                {"description", "Fade the lamp to a given level"}
            };
            
            var fadeInAdd = new JObject
            {
                {"type", "object"}, 
                {"required", new JArray("brightness", "duration")}
            };

            var fadeBrightness = new JObject
            {
                {"type", "integer"}, 
                {"minimum", 0}, 
                {"maximum", 100}, 
                {"unit", "percent"}
            };

            var fadeDuration = new JObject
            {
                {"type", "integer"}, 
                {"minimum", 1}, 
                {"unit", "milliseconds"}
            };

            var fadeProperties = new JObject
            {
                {"brightness", fadeBrightness}, 
                {"duration", fadeDuration}
            };
            
            fadeInAdd.Add("properties", fadeProperties);
            fadeMetadata.Add("inAdd", fadeInAdd);
            
            AddAvailableAction<FadeAction>("fade", fadeMetadata);
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
            public FadeAction(Thing thing, JObject input) 
                : base(thing, input)
            {
            }

            public override string Id { get; } = Guid.NewGuid().ToString();
            public override string Name => "fade";

            protected override async Task PerformActionAsync(CancellationToken cancellation)
            {
                await Task.Delay(Input.Value<int>("input"), cancellation);
                Thing.SetProperty("brightness", Input.Value<int>("brightness"));
                await Thing.AddEventAsync(new OverheatedEvent(Thing, 102), cancellation);
            }
        }
    }
}
