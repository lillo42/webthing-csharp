namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    public interface IIntercept
    {
        void Before(Thing thing);
        void After(Thing thing);
    }
}
