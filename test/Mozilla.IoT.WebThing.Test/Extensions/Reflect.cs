using System.Linq;
using System.Reflection;

namespace Mozilla.IoT.WebThing.Test
{
    internal static class Reflect
    {
        public static object Protected(this object target, string name, params object[] args) {
            var type = target.GetType();
            var method = type
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.Name == name);
            return method.Invoke(target, args);
        }
    }
}
