using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class UShortJsonMapper : IJsonMapper
    {
        private static UShortJsonMapper? s_instance;
        public static UShortJsonMapper Instance => s_instance ??= new UShortJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetUInt16();
    }
}
