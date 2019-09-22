using System;

namespace Mozilla.IoT.WebThing
{
    public sealed class ActionStatusChangedEventArgs : EventArgs
    {
        public ActionStatusChangedEventArgs(Action action)
        {
            Action = action;
        }

        public Action Action { get; }
    }
}
