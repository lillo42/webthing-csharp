using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Attributes;

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
        
        public void RunNull(
            bool? @bool,
            byte? @byte,
            sbyte? @sbyte,
            short? @short,
            ushort? @ushort,
            int? @int,
            uint? @uint,
            long? @long,
            ulong? @ulong,
            double? @double,
            float? @float,
            decimal? @decimal,
            string? @string,
            DateTime? @dateTime,
            DateTimeOffset? @dateTimeOffset,
            [FromServices]ILogger<ActionTypeThing> logger
        )
        {
            logger.LogInformation("Execution action....");
        }
        
        public void RunNullVWithValidation(
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]byte @byte,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]sbyte @sbyte,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]short @short,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]ushort @ushort,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]int @int,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]uint @uint,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]long @long,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]ulong @ulong,
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]float @float, 
             [ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]double @double,
             //[ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]decimal @decimal,
             [FromServices]ILogger<ActionTypeThing> logger
        )
        {
            logger.LogInformation("Execution action....");
        }
    }
}
