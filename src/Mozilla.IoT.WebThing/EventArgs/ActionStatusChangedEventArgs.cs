using System;

namespace Mozilla.IoT.WebThing.EventArgs
{
    public class ActionStatusChangedEventArgs : System.EventArgs
    {
        public ActionStatusChangedEventArgs(Action action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Action Action { get; }
    }
}
