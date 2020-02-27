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
        
        public void RunWithValidation(
             [ThingParameter(Minimum = 1, Maximum = 100)]byte @byte,
             [ThingParameter(Minimum = 1, Maximum = 100)]sbyte @sbyte,
             [ThingParameter(Minimum = 1, Maximum = 100)]short @short,
             [ThingParameter(Minimum = 1, Maximum = 100)]ushort @ushort,
             [ThingParameter(Minimum = 1, Maximum = 100)]int @int,
             [ThingParameter(Minimum = 1, Maximum = 100)]uint @uint,
             [ThingParameter(Minimum = 1, Maximum = 100)]long @long,
             [ThingParameter(Minimum = 1, Maximum = 100)]ulong @ulong,
             [ThingParameter(Minimum = 1, Maximum = 100)]float @float, 
             [ThingParameter(Minimum = 1, Maximum = 100)]double @double,
             //[ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]decimal @decimal,
             [FromServices]ILogger<ActionTypeThing> logger
        )
        {
            logger.LogInformation("Execution action....");
        }
        
        public void RunWithValidationExclusive(
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]byte @byte,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]sbyte @sbyte,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]short @short,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]ushort @ushort,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]int @int,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]uint @uint,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]long @long,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]ulong @ulong,
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]float @float, 
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]double @double,
            //[ThingParameter(Minimum = 1, Maximum = 100, MultipleOf = 2)]decimal @decimal,
            [FromServices]ILogger<ActionTypeThing> logger
        )
        {
            logger.LogInformation("Execution action....");
        }
        
        public void RunWithStringValidation(
            [ThingParameter(MinimumLength = 1, MaximumLength = 100)]string @minAnMax,
            [ThingParameter(Pattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]string @patter,
            [FromServices]ILogger<ActionTypeThing> logger
        )
        {
            logger.LogInformation("Execution action....");
        }
    }
}
