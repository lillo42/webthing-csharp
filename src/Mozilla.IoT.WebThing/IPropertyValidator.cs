namespace Mozilla.IoT.WebThing
{
    public interface IPropertyValidator
    {
        bool IsValid(object value);
    }
}
