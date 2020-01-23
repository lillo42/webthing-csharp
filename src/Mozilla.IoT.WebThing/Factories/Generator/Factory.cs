using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal class Factory
    {
        public static TypeBuilder CreateTypeBuilder(string typeName, string baseName, Type @interface)
        {
            var assemblyName = new AssemblyName($"{baseName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{typeName}Module");
            const TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public;
            var interfaces = new[] { @interface };
            return moduleBuilder.DefineType(typeName, typeAttributes, null, interfaces);
        }
    }
}
