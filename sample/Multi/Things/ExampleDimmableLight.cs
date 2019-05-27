using Mozilla.IoT.WebThing;
using Newtonsoft.Json.Linq;

namespace Multi.Things
{
    public class ExampleDimmableLight : Thing
    {
        public ExampleDimmableLight() 
            : base("My Lamp", new JArray("OnOffSwitch", "Light"),
                "A web connected lamp")
        {
            
        }
    }
}
