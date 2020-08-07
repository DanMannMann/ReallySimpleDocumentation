using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class UniqueOperationNamesFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var descriptor = context.ApiDescription.ActionDescriptor;
            if (descriptor is ControllerActionDescriptor cad)
            {
                operation.OperationId = $"{cad.ControllerName}.{cad.ActionName}";
                if (cad.ControllerTypeInfo.GetMethods().Count(x => x.Name == cad.MethodInfo.Name) > 1 && cad.MethodInfo.GetParameters().Length > 0)
                {
                    operation.OperationId += $".{string.Join(".", cad.MethodInfo.GetParameters().Select(x => x.Name))}";
                }
            }
        }
    }
}
