using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Mozilla.IoT.WebThing.Controllers
{
    [Route("{thingName}")]
    public class MultiThingsController : Controller
    {
        private readonly IEnumerable<Thing> _things;

        public MultiThingsController(IEnumerable<Thing> things)
        {
            _things = things;
        }

        [HttpGet]
        public IActionResult Get([FromRoute] string thingName)
        {
            
        }
    }
}
