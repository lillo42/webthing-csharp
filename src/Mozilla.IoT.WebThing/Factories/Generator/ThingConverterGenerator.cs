using System;
using System.Reflection.Emit;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal sealed partial class ThingConverterGenerator
    {
        private readonly ILGenerator _ilGenerator;
        private readonly JsonSerializerOptions _options;

        public ThingConverterGenerator(ILGenerator ilGenerator, JsonSerializerOptions options)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Generated(Thing thing)
        {
            var type = thing.GetType();
            
            PropertyWithNullableValue(nameof(Thing.Title), thing.Title);
            PropertyWithNullableValue(nameof(Thing.Description), thing.Description);
            PropertyType("@type", thing.Type);
            
            GenerateProperties(thing, type);
            GenerateEvents(thing, type);
            GenerateActions(thing, type);
            
           /* StartArray("Links");
            
            StartObject();
            PropertyWithValue("rel", "properties");
            PropertyWithValue("href", $"/things/{thing.Name}/properties");
            EndObject();
            
            StartObject();
            PropertyWithValue("rel", "actions");
            PropertyWithValue("href", $"/things/{thing.Name}/actions");
            EndObject();
            
            StartObject();
            PropertyWithValue("rel", "events");
            PropertyWithValue("href", $"/things/{thing.Name}/events");
            EndObject();
            
            
            EndArray();*/
        }
    }
}
