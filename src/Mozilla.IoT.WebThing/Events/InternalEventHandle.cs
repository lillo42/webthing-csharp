namespace Mozilla.IoT.WebThing.Events
{
    /// <summary>
    /// Handler all event
    /// </summary>
    public static class InternalEventHandle
    {
        /// <summary>
        /// Handle event and enqueue the event
        /// </summary>
        /// <param name="sender">The sender, it must be <see cref="Thing"/>.</param>
        /// <param name="args">The received args.</param>
        /// <param name="name">The event name</param>
        public static void Handler(object sender, object args, string name)
        {
            ((Thing)sender).ThingContext.Events[name].Enqueue(new Event(args), name, (Thing)sender);
        }
    }
}
