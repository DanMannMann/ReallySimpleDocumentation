using Markdig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerUIWikiFactory : ISwaggerUIWikiFactory
    {
        private Lazy<string> wikiHtmlSection;

        private SwaggerUIWikiOptions options;
        private readonly IWikiMarkdownHandler markdownHandler;

        public SwaggerUIWikiFactory(IWikiMarkdownHandler markdownHandler, IOptions<SwaggerUIWikiOptions> options)
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
            var template = options.MainTemplate;

            if (markdown.Folders.Count + markdown.Files.Count <= 1)
            {
                template = template.Replace("{{wiki_nav_class}}", " hide-nav");
            }
            else
            {
                template = template.Replace("{{wiki_nav_class}}", string.Empty);
            }

            foreach (var folder in markdown.Folders)
            {
                swaggerNav.AppendLine(options.NavBarHeadingTemplate.Replace("{{name}}", folder.Name).Replace("{{nav-bar-item-wrapper-class}}", " nav-level-0"));
                swaggerContent.AppendLine($"<span id='wiki-section-{folder.Name}'></span>");

                foreach (var file in folder.Files.Where(x => x.Name == folder.Name))
                {
                    // No nav item for the same-name file, and always put it at the top.
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", Markdown.ToHtml(file.Content)));
                }
                foreach (var file in folder.Files.Where(x => x.Name != folder.Name))
                {
                    swaggerNav.AppendLine(options.NavBarItemTemplate.Replace("{{name}}", file.Name).Replace("{{nav-bar-item-wrapper-class}}", " nav-level-1"));
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", Markdown.ToHtml(file.Content)));
                }
            }

            if (markdown.Files.Any())
            {
                swaggerContent.AppendLine($"<span id='wiki-section-Misc'></span>");
                foreach (var file in markdown.Files)
                {
                    swaggerNav.AppendLine(options.NavBarItemTemplate.Replace("{{name}}", file.Name).Replace("{{nav-bar-item-wrapper-class}}", " nav-level-0"));
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", Markdown.ToHtml(file.Content)));
                }
            }

            return template.Replace("{{wiki_nav}}", swaggerNav.ToString()).Replace("{{wiki_content}}", swaggerContent.ToString());
        }
    }
}
