using System;
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing
{
    public partial class Thing
    {
        private class AvailableAction
        {
            private const string INPUT = "input";
            public IDictionary<string, object> Metadata { get; }
            public Type Type { get; }
            public IJsonSchema Schema { get; }

            public AvailableAction(IDictionary<string, object>  metadata, Type type)
            {
                Metadata = metadata;
                Type = type;

                if (metadata.TryGetValue(INPUT, out var token))
                {
                    //Schema = JSchema.Load(token.CreateReader());
                }
                else
                {
                    Schema = null;
                }
            }

            public bool ValidateActionInput(IDictionary<string, object> jObject)
            {
                if (Schema == null)
                {
                    return true;
                }

                return true;
            }
        }
    }
}
