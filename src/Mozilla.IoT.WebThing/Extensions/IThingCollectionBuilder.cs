namespace Mozilla.IoT.WebThing.Extensions
{
    public interface IThingCollectionBuilder
    {
        IThingCollectionBuilder AddThing<T>()
            where T : Thing;
        
        IThingCollectionBuilder AddThing<T>(T thing)
            where T : Thing;
        
    }
}
