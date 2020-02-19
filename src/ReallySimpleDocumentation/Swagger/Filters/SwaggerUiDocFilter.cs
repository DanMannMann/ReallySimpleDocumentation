using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUIDocFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor hcx;
        private readonly ISwaggerUIWikiFactory wikiFactory;

        public SwaggerUIDocFilter(IHttpContextAccessor hcx, ISwaggerUIWikiFactory wikiFactory)
        {
            this.hcx = hcx;
            this.wikiFactory = wikiFactory;
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathString = hcx.HttpContext.Request.Path;
            if (pathString.Value.Contains("/swaggerui/"))
            {
                swaggerDoc.Info.Title = string.Empty;
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
