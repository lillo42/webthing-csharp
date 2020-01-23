namespace System.Text.Json
{
    internal static class JsonSerializerOptionsExtensions
    {
        public static string GetPropertyName(this JsonSerializerOptions options, string propertyName)
        {
            if (options.PropertyNamingPolicy != null)
            {
                return options.PropertyNamingPolicy.ConvertName(propertyName);
            }

            return propertyName;
        }
    }
}
