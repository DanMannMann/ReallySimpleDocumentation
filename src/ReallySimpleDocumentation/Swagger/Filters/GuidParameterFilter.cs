using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class GuidParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (context.PropertyInfo == null &&
                context.ApiParameterDescription.RouteInfo?.Constraints != null &&
                context.ApiParameterDescription.RouteInfo.Constraints.Any(c => c is GuidRouteConstraint))
            {
                parameter.Schema.Format = "uuid";
            }
        }
    }
}
