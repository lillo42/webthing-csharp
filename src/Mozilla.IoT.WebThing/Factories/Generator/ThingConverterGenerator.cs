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
            GenerateProperties(thing, type);
            GenerateEvents(thing, type);
            GenerateActions(thing, type);
        }
    }
}
