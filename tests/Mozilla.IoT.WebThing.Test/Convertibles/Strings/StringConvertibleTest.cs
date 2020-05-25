using Mozilla.IoT.WebThing.Convertibles.Strings;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Strings
{
    public class StringConvertibleTest : BaseConvertibleTest<string>
    {
        protected override IConvertible CreateConvertible()
        {
            return StringConvertible.Instance;
        }
    }
}
