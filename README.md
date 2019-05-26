# webthing

Implementation of an HTTP [Web Thing](https://iot.mozilla.org/wot/).

# Using

## Nuget

Add the following dependency to your project:

```bash
dotnet add package XXXXX
```

# Example

In this example we will set up a dimmable light and a humidity sensor (both using fake data, of course). Both working examples can be found in [here](https://github.com/mozilla-iot/webthing-java/tree/master/src/main/java/org/mozilla/iot/webthing/example).

## Dimmable Light

Imagine you have a dimmable light that you want to expose via the web of things API. The light can be turned on/off and the brightness can be set from 0% to 100%. Besides the name, description, and type, a [`Light`](https://iot.mozilla.org/schemas/#Light) is required to expose two properties:
* `on`: the state of the light, whether it is turned on or off
    * Setting this property via a `PUT {"on": true/false}` call to the REST API toggles the light.
* `brightness`: the brightness level of the light from 0-100%
    * Setting this property via a PUT call to the REST API sets the brightness level of this light.

First we create a new Thing:

```csharp
var light = new Thing("My Lamp",
                        new JArray(Arrays.AsList("OnOffSwitch", "Light")),
                        "A web connected lamp");
```

Now we can add the required properties.

The **`on`** property reports and sets the on/off state of the light. For this, we need to have a `Value` object which holds the actual state and also a method to turn the light on/off. For our purposes, we just want to log the new state if the light is switched on/off.

```csharp
var onDescription = new JObject();
onDescription.Add("@type", "OnOffProperty");
onDescription.Add("title", "On/Off");
onDescription.Add("type", "boolean");
onDescription.Add("description", "Whether the lamp is turned on");

var property = new Property<bool>(light, "on", true, onDescription);
property.ValuedChanged += (sender, value) => 
{
   Console.WriteLine($"On-State is now {value}");
};

light.AddProperty(property);
```

The **`brightness`** property reports the brightness level of the light and sets the level. Like before, instead of actually setting the level of a light, we just log the level.

```csharp
var brightnessDescription = new JObject();
brightnessDescription.Add("@type", "BrightnessProperty");
brightnessDescription.Add("title", "Brightness");
brightnessDescription.Add("type", "number");
brightnessDescription.Add("description",
                          "The level of light from 0-100");
brightnessDescription.Add("minimum", 0);
brightnessDescription.Add("maximum", 100);
brightnessDescription.Add("unit", "percent");

var level = new Property<double>(light, "level", true, onDescription);
level.ValuedChanged += (sender, value) => 
{
   Console.WriteLine($"Brightness is now {value}");
};

light.AddProperty(level);
```

Now we can add our newly created thing to the server and start it:

```csharp
try 
{
    // If adding more than one thing, use MultipleThings() with a name.
    // In the single thing case, the thing's name will be broadcast.
    WebThingServer server = new WebThingServer(new SingleThing(light), 8888);

    Runtime.getRuntime().addShutdownHook(new Thread() {
        public void run() {
            server.stop();
        }
    });

    server.start(false);
} catch (IOException e) {
    System.out.println(e);
    System.exit(1);
}
```

This will start the server, making the light available via the WoT REST API and announcing it as a discoverable resource on your local network via mDNS.

## Sensor

Let's now also connect a humidity sensor to the server we set up for our light.

A [`MultiLevelSensor`](https://iot.mozilla.org/schemas/#MultiLevelSensor) (a sensor that returns a level instead of just on/off) has one required property (besides the name, type, and optional description): **`level`**. We want to monitor this property and get notified if the value changes.

First we create a new Thing:

```csharp
var sensor = new Thing("My Humidity Sensor",
                         new JSONArray(Arrays.asList("MultiLevelSensor")),
                         "A web connected humidity sensor");
```

Then we create and add the appropriate property:
* `level`: tells us what the sensor is actually reading
    * Contrary to the light, the value cannot be set via an API call, as it wouldn't make much sense, to SET what a sensor is reading. Therefore, we are creating a *readOnly* property.

    ```csharp
    var levelDescription = new JObject();
    levelDescription.Add("@type", "LevelProperty");
    levelDescription.Add("title", "Humidity");
    levelDescription.Add("type", "number");
    levelDescription.Add("description", "The current humidity in %");
    levelDescription.Add("minimum", 0);
    levelDescription.Add("maximum", 100);
    levelDescription.Add("unit", "percent");
    levelDescription.Add("readOnly", true);

    sensor.AddProperty(new Property<double>(sensor, "level", 0, levelDescription));
    ```

Now we have a sensor that constantly reports 0%. To make it usable, we need a thread or some kind of inAdd when the sensor has a new reading available. For this purpose we start a thread that queries the physical sensor every few seconds. For our purposes, it just calls a fake method.

```csharp
// Start a thread that polls the sensor reading every 3 seconds

await Task.Factory.StartNew(async () => {
   await Task.Delay(3_000);
   await level.NotifyOfExternalUpdate(ReadFromGPIO());
});
```

This will update our `Value` object with the sensor readings via the `this.level.NotifyOfExternalUpdate(ReadFromGPIO());` call. The `Value` object now notifies the property and the thing that the value has changed, which in turn notifies all websocket listeners.
