using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
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

        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Security = new List<IDictionary<string, IEnumerable<string>>> {
                    new Dictionary<string, IEnumerable<string>> {{"oauth2", options.Scopes.Keys.ToArray()} }
            };
        }
    }
}
