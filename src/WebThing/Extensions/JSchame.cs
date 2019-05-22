using System.Collections;

namespace Newtonsoft.Json.Schema
{
    internal static class JSchemaExtensions
    {
        public static bool IsValid(this JSchema schema, object value)
        {
            if (schema.Type.HasValue)
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

            return true;
        }
    }
}
