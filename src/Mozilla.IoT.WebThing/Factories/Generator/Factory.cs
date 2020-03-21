using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal static class Factory
    {
        public static TypeBuilder CreateTypeBuilder(string typeName, string baseName, Type? @interface, TypeAttributes typeAttributes)
        {
            var assemblyName = new AssemblyName($"{baseName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{typeName}Module");
            Type[]? interfaces = null;
            if (@interface != null)
            {
                interfaces = new[] { @interface };
            }
            
            return moduleBuilder.DefineType(typeName, typeAttributes, null, interfaces);
        }
    }
}
