using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Model
{
    public class ThingModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Actions { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Events { get; set; } = new Dictionary<string, object>();
        public LinkedList<LinkModel> Links { get; set; }
    }
}
