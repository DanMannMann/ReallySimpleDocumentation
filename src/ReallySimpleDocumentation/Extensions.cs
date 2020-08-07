using Microsoft.OpenApi.Any;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    internal static class Extensions
    {
        internal static OpenApiArray ToOpenApiArray(this IEnumerable<IOpenApiAny> input)
        {
            var result = new OpenApiArray();
            result.AddRange(input);
            return result;
        }
        internal static OpenApiArray ToOpenApiArray(this IEnumerable<string> input)
        {
            var result = new OpenApiArray();
            result.AddRange(input.Select(x => new OpenApiString(x)));
            return result;
        }
    }
}
