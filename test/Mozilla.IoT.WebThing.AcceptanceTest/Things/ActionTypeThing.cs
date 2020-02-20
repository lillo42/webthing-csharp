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
            bool? @bool,
            [ThingParameter(Minimum = byte.MinValue, Maximum = byte.MaxValue, MultipleOf = 2)]byte? @byte,
            [ThingParameter(Minimum = sbyte.MinValue, Maximum = sbyte.MaxValue, MultipleOf = 2)]sbyte? @sbyte,
            [ThingParameter(Minimum = short.MinValue, Maximum = short.MaxValue, MultipleOf = 2)]short? @short,
            [ThingParameter(Minimum = ushort.MinValue, Maximum = ushort.MaxValue, MultipleOf = 2)]ushort? @ushort,
            [ThingParameter(Minimum = int.MinValue, Maximum = int.MaxValue, MultipleOf = 2)]int? @int,
            [ThingParameter(Minimum = uint.MinValue, Maximum = uint.MaxValue, MultipleOf = 2)]uint? @uint,
            [ThingParameter(Minimum = long.MinValue, Maximum = long.MaxValue, MultipleOf = 2)]long? @long,
            [ThingParameter(Minimum = ulong.MinValue, Maximum = ulong.MaxValue, MultipleOf = 2)]ulong? @ulong,
            [ThingParameter(Minimum = double.MinValue, Maximum = double.MaxValue, MultipleOf = 2)]double? @double,
            [ThingParameter(Minimum = float.MinValue, Maximum = float.MaxValue, MultipleOf = 2)]float? @float,
            [ThingParameter(Minimum = byte.MinValue, Maximum = byte.MaxValue, MultipleOf = 2)]decimal? @decimal,
            string? @string,
            DateTime? @dateTime,
            DateTimeOffset? @dateTimeOffset,
            [FromServices]ILogger<ActionTypeThing> logger
        )
        {
            logger.LogInformation("Execution action....");
        }
    }
}
