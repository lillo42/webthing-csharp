using System.Collections.Generic;
using System.Linq;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Description
{
    public class ActionDescription : IDescription<Action>
    {
        public IDictionary<string, object> CreateDescription(Action value)
        {
            var result = new Dictionary<string, object>
            {
                [HREF] = value.Href,
                [TIME_REQUESTED] = value.TimeRequested,
                [STATUS] = value.Status.ToString().ToLower()
            };

            if (value.Input != null && value.Input.Any())
            {
                result.Add(INPUT, value.Input);
            }

            if (value.TimeCompleted.HasValue)
            {
                result.Add(TIME_COMPLETED, value.TimeCompleted);
            }

            return result;
        }
    }
}