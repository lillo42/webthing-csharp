using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Actions
{
    public interface IActionInfoFactory
    {
        ActionInfo CreateActionInfo(Dictionary<string, object?> values);
    }
}
