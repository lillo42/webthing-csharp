using System.Text.Json;

namespace Mozilla.IoT.WebThing
{
    public interface IProperty
    {
        object GetValue();
        SetPropertyResult SetValue(JsonElement element);
    }
    
    public interface IProperty<out T> : IProperty
    {
        new T GetValue();

        object IProperty.GetValue()
            => GetValue();
    }
}
