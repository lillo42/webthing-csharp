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
        public ActionCollection Actions { get; } = new ActionCollection();
    }
}
