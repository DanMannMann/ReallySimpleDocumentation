using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Web.Http.Routing.Constraints;

namespace Marsman.ReallySimpleDocumentation
{
    public class GuidParameterFilter : IParameterFilter
    {
        public void Apply(IParameter parameter, ParameterFilterContext context)
        {
            if (context.PropertyInfo == null &&
                parameter is NonBodyParameter nonBodyParam &&
                context.ApiParameterDescription.RouteInfo?.Constraints != null &&
                context.ApiParameterDescription.RouteInfo.Constraints.Any(c => c is GuidRouteConstraint))
            {
                nonBodyParam.Format = "uuid";
            }
        }
    }
}
