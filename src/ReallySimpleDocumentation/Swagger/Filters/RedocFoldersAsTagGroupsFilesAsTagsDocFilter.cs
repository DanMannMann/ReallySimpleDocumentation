using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{

    public class RedocFoldersAsTagGroupsFilesAsTagsDocFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor hcx;
        private readonly IWikiMarkdownHandler markdownHandler;
        private readonly RedocUiOptions options;
        private readonly SwaggerDocOptions docOptions;

        public RedocFoldersAsTagGroupsFilesAsTagsDocFilter(IHttpContextAccessor hcx, IWikiMarkdownHandler markdownHandler, IOptions<RedocUiOptions> options, IOptions<SwaggerDocOptions> docOptions)
        {
            this.hcx = hcx ?? throw new System.ArgumentNullException(nameof(hcx));
            this.markdownHandler = markdownHandler ?? throw new System.ArgumentNullException(nameof(markdownHandler));
            this.options = options?.Value ?? throw new System.ArgumentNullException(nameof(options));
            this.docOptions = docOptions?.Value ?? throw new System.ArgumentNullException(nameof(docOptions));
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathString = hcx.HttpContext.Request.Path;
            if (pathString.Value.Contains("/redoc/"))
            {
                swaggerDoc.Info.Title = docOptions.Title;
                swaggerDoc.Info.Version = docOptions.Version;
                swaggerDoc.Tags = swaggerDoc.Tags ?? new List<OpenApiTag>();
                var existingTags = new List<string>(swaggerDoc.Tags.Select(x => x.Name));
                swaggerDoc.Info.Description = null;
                var markdown = markdownHandler.GetMarkdownDocuments();
                var tagGroups = new List<TagGroup>();
                foreach (var folder in markdown.OfType<WikiMarkdownFolder>())
                {
                    tagGroups.Add(new TagGroup
                    {
                        Name = folder.Name,
                        Tags = folder.Select(x => x.Name).ToOpenApiArray()
                    });

                    foreach (var file in folder)
                    {
                        var tag = new OpenApiTag
                        {
                            Name = file.Name,
                            Description = file.Content
                        };
                        swaggerDoc.Tags.Add(tag);
                    }
                }

                if (markdown.OfType<WikiMarkdownFile>().Any())
                {
                    var miscTagGroup = new TagGroup { Name = options.WikiRootFilesFolderName, Tags = new OpenApiArray() };
                    tagGroups.Add(miscTagGroup);
                    foreach (var file in markdown.OfType<WikiMarkdownFile>())
                    {
                        var tag = new OpenApiTag
                        {
                            Name = file.Name,
                            Description = file.Content
                        };
                        miscTagGroup.Tags.Add(new OpenApiString(file.Name));
                        swaggerDoc.Tags.Add(tag);
                    }
                }

                var additions = options.AdditionalControllersToInclude.Except(existingTags, System.StringComparer.OrdinalIgnoreCase).ToList();
                existingTags.AddRange(additions);
                tagGroups.Add(
                    new TagGroup 
                    { 
                        Name = options.ApiReferenceGroupName, 
                        Tags = existingTags.ToOpenApiArray()
                    });

                swaggerDoc.Extensions.Add("x-tagGroups", tagGroups.ToOpenApiArray());
                
                if (!swaggerDoc.Info.Extensions.ContainsKey("x-logo") && !string.IsNullOrWhiteSpace(options.LogoUrl))
                {
                    swaggerDoc.Info.Extensions.Add("x-logo", new Logo
                    {
                        AltText = options.LogoAltText,
                        BackgroundColor = options.LogoBackgroundColor,
                        Url = options.LogoUrl
                    });
                }
            }
        }
    }
}
