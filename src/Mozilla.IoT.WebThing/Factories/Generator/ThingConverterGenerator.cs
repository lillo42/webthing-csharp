using System;
using System.Reflection.Emit;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal sealed partial class ThingConverterGenerator
    {
        private readonly ILGenerator _ilGenerator;

        public ThingConverterGenerator(ILGenerator ilGenerator)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
        }

        public void Generated(Thing thing)
        {
            var type = thing.GetType();
            GenerateProperties(thing, type, true);
        }
    }
}
