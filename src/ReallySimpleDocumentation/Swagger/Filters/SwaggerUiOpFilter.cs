﻿using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUIOpFilter : IOperationFilter
    {
        private readonly IHttpContextAccessor hcx;

        public SwaggerUIOpFilter(IHttpContextAccessor hcx)
        {
            this.hcx = hcx ?? throw new System.ArgumentNullException(nameof(hcx));
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
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
