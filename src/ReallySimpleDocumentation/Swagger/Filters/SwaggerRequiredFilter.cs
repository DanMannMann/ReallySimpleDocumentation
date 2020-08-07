using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerRequiredFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var excludedProperties = context.Type
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
                        schema.Required = new HashSet<string>();
                    }
                    schema.Required.Add(propName);
                }
            }
        }
    }
}
