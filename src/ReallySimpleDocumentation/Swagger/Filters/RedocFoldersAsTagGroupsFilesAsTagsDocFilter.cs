using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;

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

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathString = hcx.HttpContext.Request.Path;
            if (pathString.Value.Contains("/redoc/"))
            {
                swaggerDoc.Info.Title = docOptions.Title;
                swaggerDoc.Info.Version = docOptions.Version;
                swaggerDoc.Tags = swaggerDoc.Tags ?? new List<Tag>();
                var existingTags = new List<Tag>(swaggerDoc.Tags);
                swaggerDoc.Info.Description = null;
                var markdown = markdownHandler.GetMarkdownDocuments();
                var tagGroups = new List<TagGroup>();
                foreach (var folder in markdown.Folders)
                {
                    tagGroups.Add(new TagGroup
                    {
                        Name = folder.Name,
                        Tags = folder.Files.Select(x => x.Name).ToList()
                    });

                    foreach (var file in folder.Files)
                    {
                        var tag = new Tag
                        {
                            Name = file.Name,
                            Description = file.Content
                        };
                        swaggerDoc.Tags.Add(tag);
                    }
                }

                if (markdown.Files.Any())
                {
                    var miscTagGroup = new TagGroup { Name = options.WikiRootFilesFolderName, Tags = new List<string>() };
                    tagGroups.Add(miscTagGroup);
                    foreach (var file in markdown.Files)
                    {
                        var tag = new Tag
                        {
                            Name = file.Name,
                            Description = file.Content
                        };
                        miscTagGroup.Tags.Add(file.Name);
                        swaggerDoc.Tags.Add(tag);
                    }
                }

                tagGroups.Add(
                    new TagGroup 
                    { 
                        Name = options.ApiReferenceGroupName, 
                        Tags = options.AdditionalControllersToInclude.Union(existingTags.Select(x => x.Name)).ToList()
                    });

                swaggerDoc.Extensions.Add("x-tagGroups", tagGroups);
                
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
