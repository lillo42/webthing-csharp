using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class ActionThing : Thing
    {
        
        public override string Name => "action";
        
        [ThingAction(Name = "fade", Title = "Fade", Type = new []{"FadeAction"},
            Description = "Fade the lamp to a given level")]
        public void Fade(
            [ThingParameter(Minimum = 0, Maximum = 100)]int level,
            [ThingParameter(Minimum = 0, Unit = "milliseconds")]int duration)
        {
            
        }
        
        public Task LongRun(CancellationToken cancellationToken)
        {
            return Task.Delay(3_000, cancellationToken);
        }

    }
}
