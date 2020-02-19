using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Marsman.ReallySimpleDocumentation
{

    public class SwaggerDocFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor hcx;
        private readonly SwaggerDocOptions options;

        public SwaggerDocFilter(IHttpContextAccessor hcx, IOptions<SwaggerDocOptions> options)
        {
            this.hcx = hcx ?? throw new System.ArgumentNullException(nameof(hcx));
            this.options = options?.Value ?? throw new System.ArgumentNullException(nameof(options));
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathString = hcx.HttpContext.Request.Path;

            if (!pathString.Value.Contains("/swaggerui/") &&
                !pathString.Value.Contains("/redoc/"))
            {
                swaggerDoc.Info.Title = options.Title;
                swaggerDoc.Info.Version = options.Version;
                swaggerDoc.Info.Description = options.DefaultDescription;
            }
        }
    }
}
