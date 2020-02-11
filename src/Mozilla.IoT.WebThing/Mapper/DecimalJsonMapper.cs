using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class DecimalJsonMapper : IJsonMapper
    {
        private static DecimalJsonMapper? s_instance;
        public static DecimalJsonMapper Instance => s_instance ??= new DecimalJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetDecimal();
    }
}
