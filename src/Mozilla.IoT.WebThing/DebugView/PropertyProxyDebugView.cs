using System.Diagnostics;

namespace Mozilla.IoT.WebThing.DebugView
{
    internal sealed class PropertyProxyDebugView
    {
        private readonly PropertyProxy _propertyProxy;
        public PropertyProxyDebugView(PropertyProxy property)
        {
            _propertyProxy = property;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Property Property => _propertyProxy.Property;
    }
}
