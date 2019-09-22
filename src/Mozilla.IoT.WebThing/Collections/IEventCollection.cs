using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Collections
{
    public interface IEventCollection : ICollection<Event>
    {
        event EventHandler<EventAddedEventArgs> EventAdded;
    }
}
