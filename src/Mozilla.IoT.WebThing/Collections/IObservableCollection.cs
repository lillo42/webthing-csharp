using System.Collections.Generic;
using System.Collections.Specialized;

namespace Mozilla.IoT.WebThing.Collections
{
    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {
    }
}
