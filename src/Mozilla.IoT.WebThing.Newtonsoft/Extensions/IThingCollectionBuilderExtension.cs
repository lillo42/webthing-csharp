using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Newtonsoft.Convertibles;
using Mozilla.IoT.WebThing.Newtonsoft.Factories;

namespace Mozilla.IoT.WebThing.Newtonsoft
{
    /// <summary>
    /// Extension class 
    /// </summary>
    public static class IThingCollectionBuilderExtension
    {
        /// <summary>
        /// Add Newtonsoft
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IThingCollectionBuilder AddNewtonsoft(this IThingCollectionBuilder builder)
        {
            builder.ServiceCollection.AddSingleton<IJsonConvertibleFactory, NewtonsoftJsonConvertibleFactory>();
            builder.ServiceCollection.AddSingleton<IJsonConvert, NewtonsoftJsonConvert>();
            return builder;
        }
    }
}
