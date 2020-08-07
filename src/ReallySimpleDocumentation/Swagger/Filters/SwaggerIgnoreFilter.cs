using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var excludedProperties = context.Type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(t => t.GetCustomAttribute<SwaggerIgnoreAttribute>() != null);
            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name))
                {
                    schema.Properties.Remove(excludedProperty.Name);
                }
                else
                {
                    var camelCaseName = char.ToLowerInvariant(excludedProperty.Name[0]) + excludedProperty.Name.Substring(1);
                    if (schema.Properties.ContainsKey(camelCaseName))
                    {
                        schema.Properties.Remove(camelCaseName);
                    }
                }
            }
        }
    }
}
