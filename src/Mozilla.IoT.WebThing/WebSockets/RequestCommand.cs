namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Represent the Web Socket request command.
    /// </summary>
    public class RequestCommand
    {
        /// <summary>
        /// The message type.
        /// </summary>
        public string? MessageType { get; set; }
        
        /// <summary>
        /// The object data.
        /// </summary>
        public object? Data { get; set; }
    }
}
