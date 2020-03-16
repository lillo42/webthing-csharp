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
        Pending,
        
        /// <summary>
        /// Executing action.
        /// </summary>
        Executing,
        
        /// <summary>
        /// Action completed.
        /// </summary>
        Completed
    }
}
