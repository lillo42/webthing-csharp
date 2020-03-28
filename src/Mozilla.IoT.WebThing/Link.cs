namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Link
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        public Link(string href, string? rel)
        {
            Href = href;
            Rel = rel;
        }

        /// <summary>
        /// Representation of a URL.
        /// </summary>
        public string Href { get; }
        
        /// <summary>
        /// Describing a relationship
        /// </summary>
        public string? Rel { get; }
    }
}
