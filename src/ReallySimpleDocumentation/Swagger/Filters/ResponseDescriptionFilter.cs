using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class ResponseDescriptionFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var descriptor = context.ApiDescription.ActionDescriptor;
            if (descriptor is ControllerActionDescriptor cad)
            {
                var responseAttribtes = cad.MethodInfo.GetCustomAttributes(typeof(SwaggerResponseAttribute), true).Cast<SwaggerResponseAttribute>().ToList();
                responseAttribtes.AddRange(cad.ControllerTypeInfo.GetCustomAttributes(typeof(SwaggerResponseAttribute), true).Cast<SwaggerResponseAttribute>());
                foreach (var response in operation.Responses)
                {
                    var attr = responseAttribtes.FirstOrDefault(x => x.StatusCode.ToString() == response.Key);
                    if (attr != null)
                    {
                        response.Value.Description = attr.Description;
                    }
                }
            }
        }
    }
}
