using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerRequiredFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var excludedProperties = context.SystemType
                                            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                            .Select(t => (Prop: t, Attr: t.GetCustomAttribute<SwaggerRequiredAttribute>()))
                                            .Where(x => x.Item2 != null);

            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.Any(x => x.Key.Equals(excludedProperty.Prop.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    var propName = schema.Properties.Single(x => x.Key.Equals(excludedProperty.Prop.Name, StringComparison.OrdinalIgnoreCase)).Key;

                    if (schema.Required == null)
                    {
                        schema.Required = new List<string>();
                    }
                    schema.Required.Add(propName);
                }
            }
        }
    }
}
