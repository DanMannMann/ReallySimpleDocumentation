using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUiDocFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor hcx;
        private readonly ISwaggerUiWikiFactory wikiFactory;

        public SwaggerUiDocFilter(IHttpContextAccessor hcx, ISwaggerUiWikiFactory wikiFactory)
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
                foreach (var tag in swaggerDoc.Tags)
                {
                    tag.Description = SwaggerUiOpFilter.Fold(tag.Description);
                }
            }
        }
    }
}
