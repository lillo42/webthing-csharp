using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class FloatJsonMapper : IJsonMapper
    {
        private static FloatJsonMapper s_instance;
        public static FloatJsonMapper Instance => s_instance ??= new FloatJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetSingle();
    }
}
