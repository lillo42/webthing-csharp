using Mozilla.IoT.WebThing;
using Newtonsoft.Json.Linq;

namespace Multi.Things
{
    public class FakeGpioHumiditySensor : Thing
    {
        public FakeGpioHumiditySensor()
            : base("My Humidity Sensor",
                new JArray("MultiLevelSensor"),
                "A web connected humidity sensor")
        {
            
        }
        
    }
}
