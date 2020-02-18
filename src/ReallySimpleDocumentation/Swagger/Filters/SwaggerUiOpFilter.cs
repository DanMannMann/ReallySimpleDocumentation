using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUiOpFilter : IOperationFilter
    {
        private readonly IHttpContextAccessor hcx;

        public SwaggerUiOpFilter(IHttpContextAccessor hcx)
        {
            this.hcx = hcx ?? throw new System.ArgumentNullException(nameof(hcx));
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var pathString = hcx.HttpContext.Request.Path;
            if (pathString.Value.Contains("/swaggerui/"))
            {
                operation.Description = Fold(operation.Description);
            }
        }

        public static string Fold(string input)
        {
            if (input == null) return input;
            return input.Replace("<fold>", $"<details><summary>Overview...</summary>{Environment.NewLine}{Environment.NewLine}")
                        .Replace("</fold>", "</details>");
        }
    }
}
