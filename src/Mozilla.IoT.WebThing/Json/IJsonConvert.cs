namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonConvert
    {
        T Deserialize<T>(string value);
        string Serialize<T>(T value, IJsonSerializerSettings settings);
        string Serialize(object value, IJsonSerializerSettings settings);
    }
}
