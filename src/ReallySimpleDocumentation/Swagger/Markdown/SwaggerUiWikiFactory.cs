using Markdig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUiWikiFactory : ISwaggerUiWikiFactory
    {
        private Lazy<string> wikiHtmlSection;

        private SwaggerUiWikiOptions options;
        private readonly IWikiMarkdownHandler markdownHandler;

        public SwaggerUiWikiFactory(IWikiMarkdownHandler markdownHandler, IOptions<SwaggerUiWikiOptions> options)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.wikiHtmlSection = new Lazy<string>(BuildWikiHtmlSection);
            this.markdownHandler = markdownHandler ?? throw new ArgumentNullException(nameof(markdownHandler));
        }

        public string GetWikiHtmlSection()
        {
            return wikiHtmlSection.Value;
        }
        private string BuildWikiHtmlSection()
        {
            var swaggerNav = new StringBuilder();
            var swaggerContent = new StringBuilder();
            var markdown = markdownHandler.GetMarkdownDocuments();
            foreach (var folder in markdown.Folders)
            {
                swaggerNav.AppendLine(options.NavBarHeadingTemplate.Replace("{{name}}", folder.Name));
                swaggerContent.AppendLine($"<span id='wiki-section-{folder.Name}'></span>");
                foreach (var file in folder.Files)
                {
                    swaggerNav.AppendLine(options.NavBarItemTemplate.Replace("{{name}}", file.Name));
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", Markdown.ToHtml(file.Content)));
                }
            }

            if (markdown.Files.Any())
            {
                swaggerContent.AppendLine(options.NavBarHeadingTemplate.Replace("{{name}}", "Misc."));
                foreach (var file in markdown.Files)
                {
                    swaggerNav.AppendLine(options.NavBarItemTemplate.Replace("{{name}}", file.Name));
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", Markdown.ToHtml(file.Content)));
                }
            }

            return options.MainTemplate.Replace("{{wiki_nav}}", swaggerNav.ToString()).Replace("{{wiki_content}}", swaggerContent.ToString());
        }
    }
}
