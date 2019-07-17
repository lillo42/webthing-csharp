namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonConvert
    {
        T Deserialize<T>(string value);
        T Deserialize<T>(string value, IJsonSerializerSettings settings);
        
        string Serialize<T>(T value);
        string Serialize(object value);
        string Serialize<T>(T value, IJsonSerializerSettings settings);
        string Serialize(object value, IJsonSerializerSettings settings);
    }
}
