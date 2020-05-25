using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Integration.Test.Web.Things
{
    public class ActionThing : Thing
    {
        public override string Name => "action-thing";

        public async Task LongRunning(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken)
                .ConfigureAwait(false);
        }

        public ValueTask MayLongRunning(int? value, [FromServices]ILogger<ActionThing> logger, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Going to execute {nameof(MayLongRunning)}");

            if (!value.HasValue || value.Value == 0)
            {
                return new ValueTask();
            }
            
            return new ValueTask(Task.Delay(value.Value, cancellationToken));
        }
        
        [ThingAction(Ignore = true)]
        public void Nothing() {}

        public void NoRestriction(string value, int level, bool active, Guid id, [FromServices]ILogger<ActionThing> logger)
        {
            logger.LogInformation("Received: [Value: {value}][Level: {level}][Active: {active}][Id: {id}]",
                value, level, active, id);
        }
        
        public void WithRestriction([ThingParameter(IsNullable = false)]string value, 
            [ThingParameter(ExclusiveMinimum = 1, ExclusiveMaximum = 100)]int level, 
            bool active, 
            [ThingParameter(Enum = new object[]{"4f12dbd1-ea24-40e6-ac26-620a1b787a25", "a8e3202d-7eaa-4889-a5cf-ec44275414eb"})]Guid id, 
            [FromServices]ILogger<ActionThing> logger)
        {
            logger.LogInformation("Received: [Value: {value}][Level: {level}][Active: {active}][Id: {id}]",
                value, level, active, id);
        }
    }

    public class WebSocketActionThing : ActionThing
    {
        public override string Name => "web-socket-action-thing";
    }
}
