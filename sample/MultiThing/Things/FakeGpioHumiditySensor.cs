using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Attributes;

namespace MultiThing.Things
{
    public class FakeGpioHumiditySensor : Thing
    {
        private readonly Random _random;
        private readonly ILogger<FakeGpioHumiditySensor> _logger;
        public FakeGpioHumiditySensor(ILogger<FakeGpioHumiditySensor> logger)
        {
            _logger = logger;
            _random = new Random();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(3_000).GetAwaiter().GetResult();
                    Level = ReadFromGPIO();
                }
            }, TaskCreationOptions.LongRunning);
        }
        public override string Name => "my-humidity-sensor-1234";

        public override string Title => "My Humidity Sensor";

        public override string[] Type { get; } = new[] {"MultiLevelSensor"};

        public override string Description => "A web connected humidity sensor";

        private double _level;

        [ThingProperty(Type = new[] {"LevelProperty"}, Title = "Humidity", Description = "The current humidity in %",
            Minimum = 0, Maximum = 100, Unit = "percent")]
        public double Level
        {
            get => _level;
            private set
            {
                _level = value;
                _logger.LogInformation("setting new humidity level: {level}", value);
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Mimic an actual sensor updating its reading every couple seconds.
        /// </summary>
        /// <returns></returns>
        private double ReadFromGPIO()
        {
            var value = Math.Abs(70.0d * _random.NextDouble() * (-0.5 + _random.NextDouble()));
            return value;
        }
        
    }
}
