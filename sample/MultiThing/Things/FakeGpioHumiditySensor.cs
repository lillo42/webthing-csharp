using System;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Attributes;

namespace MultiThing.Things
{
    public class FakeGpioHumiditySensor : Thing
    {
        private readonly Random _random;
        public FakeGpioHumiditySensor()
        {
            _random = new Random();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(3_000).GetAwaiter().GetResult();
                    var newLevel =  ReadFromGPIO();
                    Console.WriteLine("setting new humidity level: {0}", newLevel);
                    Level = newLevel;
                }
            }, TaskCreationOptions.LongRunning);
        }
        public override string Name => "my-humidity-sensor-1234";

        public override string? Title => "My Humidity Sensor";

        public override string[]? Type { get; } = new[] {"MultiLevelSensor"};

        public override string? Description => "A web connected humidity sensor";
        
        
        [ThingProperty(Type = new []{"LevelProperty"}, Title = "Humidity", Description = "The current humidity in %",
            Minimum = 0, Maximum = 100, Unit = "percent")]
        public double Level { get; private set; }
        
        /// <summary>
        /// Mimic an actual sensor updating its reading every couple seconds.
        /// </summary>
        /// <returns></returns>
        private double ReadFromGPIO() {
            return Math.Abs(70.0d * _random.Next() * (-0.5 + _random.Next()));
        }
        
    }
}
