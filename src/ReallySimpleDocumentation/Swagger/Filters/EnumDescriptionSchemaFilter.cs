using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class EnumDescriptionSchemaFilter : ISchemaFilter
    {
        private const string NodeXPath = "/doc/members/member[@name='{0}']";
        private const string SummaryTag = "summary";
        private readonly IXmlDocProvider xmlDocProvider;

        public EnumDescriptionSchemaFilter(IXmlDocProvider xmlDocProvider)
        {
            this.xmlDocProvider = xmlDocProvider;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Enum?.Any() == true && context.Type.IsEnum)
            {
                TryApplyTypeComments(schema, context.Type);
            }
        }

        private void TryApplyTypeComments(OpenApiSchema schema, Type type)
        {
            var typeNode = xmlDocProvider.GetNodeForType(type);
            if (typeNode == null) return;
            var summaryNode = typeNode.SelectSingleNode(SummaryTag);
            if (summaryNode != null)
                schema.Description = XmlCommentsTextHelper.Humanize(summaryNode.InnerXml);
        }
    }
}
