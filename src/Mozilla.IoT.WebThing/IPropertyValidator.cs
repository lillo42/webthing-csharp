namespace Mozilla.IoT.WebThing
{
    public interface IPropertyValidator
    {
        bool IsReadOnly { get; }
        bool IsValid(object value);
    }
}
