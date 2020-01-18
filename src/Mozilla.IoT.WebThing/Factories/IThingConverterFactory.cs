using System.Text.Json;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing.Factories
{
    public interface IThingConverterFactory
    {
        IThingConverter Create(Thing thing, JsonSerializerOptions options);
    }
}
