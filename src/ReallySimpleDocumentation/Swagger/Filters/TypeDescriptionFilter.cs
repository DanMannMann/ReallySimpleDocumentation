using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class TypeDescriptionFilter : ISchemaFilter
    {
        private readonly IEnumerable<TypeDescription> types;

        public TypeDescriptionFilter(IEnumerable<TypeDescription> types)
        {
            this.types = types;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = types.FirstOrDefault(x => x.Type == context.Type);
            if (type != null)
            {
                schema.Description = type.Description ?? schema.Description;

                if (schema.Properties != null)
                {
                    var propertyIntersect = type.MembersToExclude.Intersect(schema.Properties.Select(x => x.Key)).ToList();
                    schema.Properties = schema.Properties.Where(x => !propertyIntersect.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                    foreach (var property in type.PropertyDescriptions)
                    {
                        if (schema.Properties.ContainsKey(property.Key))
                        {
                            var target = schema.Properties[property.Key];
                            target.Description = property.Value;
                        }
                    }
                }

                if (schema.Enum != null)
                {
                    schema.Enum = schema.Enum.Where(x => !(x is OpenApiString s) || !type.MembersToExclude.Contains(s.Value, System.StringComparer.OrdinalIgnoreCase)).ToList();
                }
            }
        }
    }
}
