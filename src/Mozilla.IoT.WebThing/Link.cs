using System;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// Represent link
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Initialize a new instance of <see cref="Link"/>.
        /// </summary>
        /// <param name="href">Representation of a URL.</param>
        /// <param name="rel">Describing a relationship</param>
        public Link(string? rel, string href)
        {
            Href = href ?? throw new ArgumentNullException(nameof(href));
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
