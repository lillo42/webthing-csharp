namespace Mozilla.IoT.WebThing.Http
{
    public interface IHttpRouteValue
    {
        T GetValue<T>(string key);
    }
}
