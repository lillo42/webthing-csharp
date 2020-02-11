using System;
using System.Collections.Concurrent;
using Mozilla.IoT.WebThing.Actions;

namespace Mozilla.IoT.WebThing
{
    public class ActionContext
    {
        public ActionContext(Type actionType)
        {
            ActionType = actionType ?? throw new ArgumentNullException(nameof(actionType));
        }

        public Type ActionType { get; }
        public ConcurrentDictionary<Guid, ActionInfo> Actions { get; } = new ConcurrentDictionary<Guid, ActionInfo>();
    }
}
