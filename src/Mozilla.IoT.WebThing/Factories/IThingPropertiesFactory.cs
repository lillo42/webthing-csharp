namespace Mozilla.IoT.WebThing.Factories
{
    public interface IThingPropertiesFactory
    {
        IProperties Create(Thing thing);
    }
}
