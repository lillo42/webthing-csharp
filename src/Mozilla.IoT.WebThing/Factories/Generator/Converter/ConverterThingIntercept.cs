using System;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal class ConverterThingIntercept : IThingIntercept
    {
        private readonly Utf8JsonWriterILGenerator _jsonWriter;

        public ConverterThingIntercept(Utf8JsonWriterILGenerator jsonWriter)
        {
            _jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof(jsonWriter));
        }
        
        public void Before(Thing thing)
        {
            _jsonWriter.PropertyWithNullableValue(nameof(Thing.Title), thing.Title);
            _jsonWriter.PropertyWithNullableValue(nameof(Thing.Description), thing.Description);
            _jsonWriter.PropertyType("@type", thing.Type);
        }

        public void After(Thing thing)
        {
            
        }
    }
}
