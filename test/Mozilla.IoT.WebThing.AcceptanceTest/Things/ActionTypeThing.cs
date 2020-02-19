using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class ActionTypeThing : Thing
    {
        public override string Name => "action-type";

        public void Run(
            bool @bool,
            byte @byte,
            sbyte @sbyte,
            short @short,
            ushort @ushort,
            int @int,
            uint @uint,
            long @long,
            ulong @ulong,
            double @double,
            float @float,
            decimal @decimal,
            string @string,
            DateTime @dateTime,
            DateTimeOffset @dateTimeOffset,
            [FromServices]ILogger<ActionTypeThing> logger
            )
        {
            logger.LogInformation("Execution action....");
        }
    }
}
