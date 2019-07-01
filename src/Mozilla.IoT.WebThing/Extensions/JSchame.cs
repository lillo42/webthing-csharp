using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Mozilla.IoT.WebThing.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class JSchemaExtensions
    {
        public static bool IsValid(this JSchema schema, object value)
        {
            if (schema.Type.HasValue)
            {
                if (value is JValue jValue)
                {
                    switch (schema.Type.Value)
                    {
                        case JSchemaType.Number:
                            return jValue.Type == JTokenType.Float
                                   || jValue.Type == JTokenType.Integer;
                        case JSchemaType.Integer:
                            return jValue.Type == JTokenType.Integer;
                        case JSchemaType.Boolean:
                            return jValue.Type == JTokenType.Boolean;
                        case JSchemaType.Array:
                            return jValue.Type == JTokenType.Array;
                    }
                }
                else
                {
                    switch (schema.Type.Value)
                    {
                        case JSchemaType.String:
                            return value is string;
                        case JSchemaType.Number:
                            return value is int
                                   || value is short
                                   || value is double
                                   || value is long
                                   || value is decimal
                                   || value is float
                                   || value is uint
                                   || value is ulong;
                        case JSchemaType.Integer:
                            return value is int;
                        case JSchemaType.Boolean:
                            return value is bool;
                        case JSchemaType.Array:
                            return value is IEnumerable;
                    }
                }
            }

            return true;
        }
    }
}
