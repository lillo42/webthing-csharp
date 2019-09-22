namespace Mozilla.IoT.WebThing
{
    internal interface IHttpRouteValue
    {
        T GetValue<T>(string key);
    }
}
