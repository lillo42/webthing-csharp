using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class UIntJsonMapper : IJsonMapper
    {
        private static UIntJsonMapper s_instance;
        public static UIntJsonMapper Instance => s_instance ??= new UIntJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetUInt32();
    }
}
