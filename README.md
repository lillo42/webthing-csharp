# webthing

[![Build Status](https://lillo42.visualstudio.com/Moziila%20%20IoT%20-%20Web%20Thing/_apis/build/status/lillo42.webthing-csharp?branchName=master)](https://lillo42.visualstudio.com/Moziila%20%20IoT%20-%20Web%20Thing/_build/latest?definitionId=3&branchName=master)
[![NuGet](http://img.shields.io/nuget/v/Mozilla.IoT.WebThing.svg)](https://www.nuget.org/packages/Mozilla.IoT.WebThing/)



Implementation of an HTTP [Web Thing](https://iot.mozilla.org/wot/).

# Using

## Nuget

Add the following dependency to your project:

```bash
dotnet add package Mozilla.IoT.WebThing
```

# Example

In this example we will set up a dimmable light and a humidity sensor (both using fake data, of course). Both working examples can be found in [here](https://github.com/lillo42/webthing-csharp/tree/master/sample).

## Dimmable Light

Imagine you have a dimmable light that you want to expose via the web of things API. The light can be turned on/off and the brightness can be set from 0% to 100%. Besides the name, description, and type, a [`Light`](https://iot.mozilla.org/schemas/#Light) is required to expose two properties:
* `on`: the state of the light, whether it is turned on or off
    * Setting this property via a `PUT {"on": true/false}` call to the REST API toggles the light.
* `brightness`: the brightness level of the light from 0-100%
    * Setting this property via a PUT call to the REST API sets the brightness level of this light.

First we create a new Thing:

```csharp
public class LampThing : Thing
{
    public override string Name => "my-lamp-123";
    public override string? Title => "My Lamp";
    public override string? Description => "A web connected lamp";
    public override string[]? Type { get; } = new[] { "Light", "OnOffSwitch" };

}
```

Now we can add the required properties.

The **`on`** property reports and sets the on/off state of the light. For this, we need to create a new property in Thing and add ```ThingPropertyAttribute```. For our purposes, we just want to log the new state if the light is switched on/off.

```csharp
public class LampThing : Thing
{
    ...
    private bool _on;

    [ThingProperty(Type = new []{ "OnOffProperty" }, Title = "On/Off", Description = "Whether the lamp is turned on")]
    public bool On 
    { 
        get => _on;
        set 
        {
            _on = value;
            Console.WriteLine($"On is now {value}");
            OnPropertyChanged();
        }
    }
}
```

The **`brightness`** property reports the brightness level of the light and sets the level. Like before, instead of actually setting the level of a light, we just log the level.

```csharp
public class LampThing : Thing
{
    ...
    private int _brightness;
    [ThingProperty(Type = new []{ "BrightnessProperty" },Title = "Brightness",
        Description = "The level of light from 0-100", Minimum = 0, Maximum = 100,
        Unit = "percent")]
    public int Brightness 
    { 
        get => _brightness; 
        set
        { 
            _brightness = value;
            Console.WriteLine($"Brightness is now {value}");
            OnPropertyChanged();
        } 
    }
}
```

Now we can add our newly created thing and add Thing middleware to Asp Net Core:

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddThings()
        .AddThing<LampThing>();

    // If you want use Web Sockets.
    services.AddWebSockets(opt => { });
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // If you want use Web Sockets.
    app.UseWebSockets();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapThings();
    });
}
```

This will start the server, making the light available via the WoT REST API and announcing it as a discoverable resource on your local network via mDNS.

## Sensor

Let's now also connect a humidity sensor to the server we set up for our light.

A [`MultiLevelSensor`](https://iot.mozilla.org/schemas/#MultiLevelSensor) (a sensor that returns a level instead of just on/off) has one required property (besides the name, type, and optional description): **`level`**. We want to monitor this property and get notified if the value changes.

First we create a new Thing:

```csharp
public class Humidity : Thing
{
    public override string? Title => "My Humidity Sensor";

    public override string[]? Type { get; } = new[] {"MultiLevelSensor"};

    public override string? Description => "A web connected humidity sensor";
}
```

Then we create and add the appropriate property:
* `level`: tells us what the sensor is actually reading
    * Contrary to the light, the value cannot be set via an API call, as it wouldn't make much sense, to SET what a sensor is reading. Therefore, we are creating a *readOnly* property.

```csharp
public class Humidity : Thing
{
    ...
    [ThingProperty(Type = new []{"LevelProperty"}, Title = "Humidity", Description = "The current humidity in %",
        Minimum = 0, Maximum = 100, Unit = "percent")]
    public double Level { get; private set; }
}
```

Now we have a sensor that constantly reports 0%. To make it usable, we need a thread or some kind of inAdd when the sensor has a new reading available. For this purpose we start a task that queries the physical sensor every few seconds. For our purposes, it just calls a fake method.

```csharp
// Start a task that polls the sensor reading every 3 seconds

Task.Factory.StartNew(async () => {
   await Task.Delay(3_000);
   await Level = ReadFromGPIO();
});
```