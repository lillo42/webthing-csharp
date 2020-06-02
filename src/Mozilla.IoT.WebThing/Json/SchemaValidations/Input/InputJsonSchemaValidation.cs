using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.Input
{ 
    /// <summary>
    /// 
    /// </summary>
    public class InputJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly Dictionary<string, IJsonSchemaValidation> _schemaValidations;

        /// <summary>
        /// Initialize a new instance of <see cref="InputJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="schemaValidations">The <see cref="IJsonSchemaValidation"/>>.</param>
        public InputJsonSchemaValidation(Dictionary<string, IJsonSchemaValidation> schemaValidations)
        {
            _schemaValidations = schemaValidations ?? throw new ArgumentNullException(nameof(schemaValidations));
        }

        /// <inheritdoc/>
        public bool IsValid(object? value)
        {
            if (!(value is Dictionary<string, object?> input))
            {
                return false;
            }
            
            var properties = new HashSet<string>();

            foreach (var (propertyName, propertyValue) in input)
            {
                if (!_schemaValidations.TryGetValue(propertyName, out var schemaValidation))
                {
                    continue;
                }

                if (!schemaValidation.IsValid(propertyValue))
                {
                    return false;
                }
                
                properties.Add(propertyName);
            }
            
            foreach (var (propertyName, schemaValidation) in _schemaValidations)
            {
                if (properties.Contains(propertyName))
                {
                    continue;
                }

                if (!schemaValidation.IsValid(null))
                {
                    return false;
                }
                    
                input.Add(propertyName, null!);
            }
            
            
            return true;
        }
    }
}
