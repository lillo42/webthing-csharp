namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// 
    /// </summary>
    public class Link
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        public Link(string href)
        {
            Href = href;
            Rel = null;
        }
        
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
