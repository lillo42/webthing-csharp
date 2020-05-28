namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Action status
    /// </summary>
    public enum ActionStatus
    {
        /// <summary>
        /// Waiting to be execute.
        /// </summary>
        Created,
        
        /// <summary>
        /// Executing action.
        /// </summary>
        Pending,
        
        /// <summary>
        /// Action completed.
        /// </summary>
        Completed
    }
}
