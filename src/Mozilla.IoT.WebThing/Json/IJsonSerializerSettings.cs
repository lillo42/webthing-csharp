namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonSerializerSettings
    {
        bool IgnoreNullValues { get; }
        bool WriteIndented { get; }
    }
}
