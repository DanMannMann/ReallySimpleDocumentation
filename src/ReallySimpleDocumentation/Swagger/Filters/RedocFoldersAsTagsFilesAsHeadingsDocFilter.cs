using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marsman.ReallySimpleDocumentation
{
    public class RedocFoldersAsTagsFilesAsHeadingsDocFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor hcx;
        private readonly IWikiMarkdownHandler markdownHandler;
        private readonly RedocUiOptions options;
        private readonly SwaggerDocOptions docOptions;

        public RedocFoldersAsTagsFilesAsHeadingsDocFilter(IHttpContextAccessor hcx, IWikiMarkdownHandler markdownHandler, IOptions<RedocUiOptions> options, IOptions<SwaggerDocOptions> docOptions)
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
                var tagGroup = new TagGroup
                {
                    Name = options.WikiGroupName,
                    Tags = new OpenApiArray()
                };
                foreach (var node in markdown)
                {
                    var content = new StringBuilder();
                    switch (node)
                    {
                        case WikiMarkdownFolder folder:
                            tagGroup.Tags.Add(new OpenApiString(folder.Name));
                            foreach (var file in folder.Where(x => x.Name == folder.Name))
                            {
                                content.AppendLine(file.Content);
                                content.AppendLine();
                                content.AppendLine();
                            }

                            foreach (var file in folder.Where(x => x.Name != folder.Name))
                            {
                                content.AppendLine($"# {file.Name}");
                                content.AppendLine(file.Content);
                                content.AppendLine();
                                content.AppendLine();
                            }
                            swaggerDoc.Tags.Add(new OpenApiTag
                            {
                                Name = folder.Name,
                                Description = content.ToString()
                            });
                            break;

                        case WikiMarkdownFile rootFile:
                            tagGroup.Tags.Add(new OpenApiString(rootFile.Name));
                            content.AppendLine(rootFile.Content);
                            content.AppendLine();
                            content.AppendLine();
                            swaggerDoc.Tags.Add(new OpenApiTag
                            {
                                Name = rootFile.Name,
                                Description = content.ToString()
                            });
                            break;
                    }
                }

                var additions = options.AdditionalControllersToInclude.Except(existingTags, System.StringComparer.OrdinalIgnoreCase).ToList();
                existingTags.AddRange(additions);

                swaggerDoc.Extensions.Add("x-tagGroups", new OpenApiArray
                {
                    tagGroup,
                    new TagGroup
                    {
                        Name = options.ApiReferenceGroupName,
                        Tags = existingTags.ToOpenApiArray()
                    }
                });

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
