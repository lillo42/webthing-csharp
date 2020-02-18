using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class DateTimeOffsetJsonMapper : IJsonMapper
    {
        private static DateTimeOffsetJsonMapper? s_instance;
        public static DateTimeOffsetJsonMapper Instance => s_instance ??= new DateTimeOffsetJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetDateTimeOffset();
    }
}
