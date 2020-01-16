namespace Mozilla.IoT.WebThing.Hateos
{
    public interface IMapper<TSource, TDestiny>
    {
        TDestiny Map(TSource source);
    }
}
