using System.Reflection.Emit;

namespace Mozilla.IoT.WebThing.Extensions
{
    internal static class ILGeneratorExtensions
    {
        #region Return
        public static void Return(this ILGenerator generator, string value)
        {
            generator.Emit(OpCodes.Ldstr, value);
            generator.Emit(OpCodes.Ret);
        }
        
        public static void Return(this ILGenerator generator, FieldBuilder field)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
            generator.Emit(OpCodes.Ret);
        }
        #endregion

        #region Set

        public static void Set(this ILGenerator generator, FieldBuilder field)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);
        }

        public static void SetArgToLocal(this ILGenerator generator, LocalBuilder local)
        {
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stloc_S, local.LocalIndex);
        }

        #endregion
    }
}
