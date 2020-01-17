using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing.Generator
{
    public interface IGenerateThingConvert
    {
        IThingConverter Generate<T>(T thing)
            where T : Thing;
    }
}
