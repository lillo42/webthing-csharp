using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Visitor
{
    internal static class PropertiesVisitor
    {
        public static void Visit(IEnumerable<IPropertyIntercept> intercepts, Thing thing)
        {
            var thingType = thing.GetType();
            var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name));
            
            foreach (var intercept in intercepts)
            {
                intercept.Before(thing);
            }
            
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (!IsAcceptedType(propertyType))
                {
                    continue;
                }
                
                var information = propertyType.GetCustomAttribute<ThingPropertyAttribute>();

                if (information != null && information.Ignore)
                {
                    continue;
                }

                foreach (var intercept in intercepts)
                {
                    intercept.Intercept(thing, property, information);
                }
            }
            
            
            foreach (var intercept in intercepts)
            {
                intercept.After(thing);
            }
        }
        
        private static bool IsAcceptedType(Type? type)
        {
            if (type == null)
            {
                return false;
            }
            
            return type == typeof(string)
                   || type == typeof(bool)
                   || type == typeof(int)
                   || type == typeof(byte)
                   || type == typeof(short)
                   || type == typeof(long)
                   || type == typeof(uint)
                   || type == typeof(ulong)
                   || type == typeof(ushort)
                   ||type == typeof(double)
                   || type == typeof(float);
        }
        
        private static bool IsThingProperty(string name)
            => name == nameof(Thing.Context)
               || name == nameof(Thing.Name)
               || name == nameof(Thing.Description)
               || name == nameof(Thing.Title)
               || name == nameof(Thing.Type);
    }
}
