using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing;

namespace Multi.Things
{
    public class FakeGpioHumiditySensor : Thing
    {
        private readonly Property<double> _level;  
        private static Random s_random = new Random();
        public FakeGpioHumiditySensor()
        {
            Name = "My Humidity Sensor";
            Type = new[] {"MultiLevelSensor"};
            Description = "A web connected humidity sensor";
                
            var levelDescription = new Dictionary<string, object>
            {
                ["@type"] = "LevelProperty",
                ["title"] = "Humidity",
                ["type"] = "number",
                ["description"] = "The current humidity in %",
                ["minimum"] = 0,
                ["maximum"] = 100,
                ["unit"] = "percent",
                ["readOnly"] = true
            };

            _level = new Property<double>(this, "level", 0, levelDescription);

            Properties.Add(_level);
            
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(3_000);
                double value = ReadFromGPIO();
                Console.WriteLine($"setting new humidity level: {value}");
                //_level.NotifyOfExternalUpdate(value);
            });
        }
        
        /**
         * Mimic an actual sensor updating its reading every couple seconds.
         */
        private static double ReadFromGPIO()
            => Math.Abs(70.0d * s_random.Next() * (-0.5 + s_random.Next()));
    }
}
