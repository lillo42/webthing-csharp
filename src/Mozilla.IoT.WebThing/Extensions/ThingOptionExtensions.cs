using System.Text.Json;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ThingOptionExtensions
    {
        private static JsonSerializerOptions? s_options;
        private static readonly object s_locker = new object();

        /// <summary>
        /// Convert the <see cref="ThingOption"/> to <see cref="JsonSerializerOptions"/>.
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/> to be convert.</param>
        /// <returns>The <see cref="JsonSerializerOptions"/>.</returns>
        public static JsonSerializerOptions ToJsonSerializerOptions(this ThingOption option)
        {
            if (s_options == null)
            {
                lock (s_locker)
                {
                    if (s_options == null)
                    {
                        s_options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = option.PropertyNamingPolicy,
                            DictionaryKeyPolicy = option.PropertyNamingPolicy,
                            IgnoreReadOnlyProperties = false,
                            IgnoreNullValues = option.IgnoreNullValues,
                            WriteIndented = option.WriteIndented,
                            Converters =
                            {
                                new ActionStatusConverter()
                            }
                        };
                    }
                }
            }

            return s_options!;
        }
    }
}
