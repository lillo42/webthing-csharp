using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// The <see cref="Thing"/> Response.
    /// </summary>
    public abstract class ThingResponse
    {
        private readonly Thing _thing;

        /// <summary>
        /// Initialize a new instance of <see cref="ThingResponse"/>.
        /// </summary>
        /// <param name="thing"></param>
        protected ThingResponse(Thing thing)
        {
            _thing = thing;
        }
        
        
        /// <summary>
        /// The id member provides an identifier of the device in the form of a URI [RFC3986] (e.g. a URL or a URN). 
        /// </summary>
        public virtual string? Id { get; set; }

        /// <summary>
        /// The @context member is an optional annotation which can be used to provide a URI for a schema repository which defines standard schemas for common "types" of device capabilities. 
        /// </summary>
        [JsonPropertyName("@context")] 
        public string Context => _thing.Context;

        /// <summary>
        /// The title member is a human friendly string which describes the device. 
        /// </summary>
        public string? Title => _thing.Title;
        
        /// <summary>
        /// The description member is a human friendly string which describes the device and its functions.
        /// </summary>
        public string? Description => _thing.Description;
        
        /// <summary>
        /// Links
        /// </summary>
        public List<Link> Links { get; } = new List<Link>();
    }
}
