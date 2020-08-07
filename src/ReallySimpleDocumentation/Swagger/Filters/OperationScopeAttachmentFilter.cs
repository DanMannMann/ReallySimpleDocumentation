using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class OperationScopeAttachmentFilter : IOperationFilter
    {
        private readonly OperationScopeOptions options;

        public OperationScopeAttachmentFilter(IOptions<OperationScopeOptions> options)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        options.Scopes.Keys.ToList()
                    }
                }
            };
        }
    }
}
