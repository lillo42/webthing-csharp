using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Mozzila.IoT.WebThing
{
    public partial class Thing
    {
        private class AvailableAction
        {
            private const string INPUT = "input";
            public JObject Metadata { get; }
            public Type Type { get; }
            public JSchema Schema { get; }

            public AvailableAction(JObject metadata, Type type)
            {
                Metadata = metadata;
                Type = type;

                if (metadata.TryGetValue(INPUT, out JToken token))
                {
                    Schema = JSchema.Load(token.CreateReader());
                }
                else
                {
                    Schema = null;
                }
            }

            public bool ValidateActionInput(JObject jObject)
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
