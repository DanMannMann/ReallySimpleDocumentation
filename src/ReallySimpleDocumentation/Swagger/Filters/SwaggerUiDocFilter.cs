using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUIDocFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor hcx;
        private readonly ISwaggerUIWikiFactory wikiFactory;
        private readonly SwaggerDocOptions docOptions;

        public SwaggerUIDocFilter(IHttpContextAccessor hcx, ISwaggerUIWikiFactory wikiFactory, IOptions<SwaggerDocOptions> docOptions)
        {
            this.hcx = hcx;
            this.wikiFactory = wikiFactory;
            this.docOptions = docOptions?.Value;
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathString = hcx.HttpContext.Request.Path;
            if (pathString.Value.Contains("/swaggerui/"))
            {
                swaggerDoc.Info.Title = string.Empty;
                swaggerDoc.Info.Version = docOptions.Version;
                swaggerDoc.Info.Description = wikiFactory.GetWikiHtmlSection();
                if (swaggerDoc.Tags != null)
                {
                    foreach (var tag in swaggerDoc.Tags)
                    {
                        tag.Description = SwaggerUIOpFilter.Fold(tag.Description);
                    }
                }
            }
        }
    }
}
