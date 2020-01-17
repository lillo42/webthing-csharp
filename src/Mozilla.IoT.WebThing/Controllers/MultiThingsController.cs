using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Controllers
{
    [Route("{thingName}")]
    public class MultiThingsController : Controller
    {
        private readonly IEnumerable<Thing> _things;
        private readonly ILogger<MultiThingsController> _logger;
        public MultiThingsController(IEnumerable<Thing> things, ILogger<MultiThingsController> logger)
        {
            _things = things ?? throw new ArgumentNullException(nameof(things));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Get([FromRoute] string thingName)
        {
            var thing = _things.FirstOrDefault(x => x.Name == thingName);
            if (thing == null)
            {
                return NotFound();
            }
            
            return Ok();
        }
    }
}
