using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

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
            var navCount = 0;

            foreach (var folder in markdown.Folders)
            {
                navCount++;
                swaggerNav.AppendLine(options.NavBarHeadingTemplate.Replace("{{name}}", folder.Name).Replace("{{nav-bar-item-wrapper-class}}", " nav-level-0"));
                swaggerContent.AppendLine($"<span id='wiki-section-{folder.Name}'></span>");

                foreach (var file in folder.Files.Where(x => x.Name == folder.Name))
                {
                    // No nav item for the same-name file, and always put the file at the top of the folder section.
                    var html = Markdown.ToHtml(file.Content);
                    html = InsertHeadingTagIds(swaggerNav, file, html, 2);
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", html));
                }
                foreach (var file in folder.Files.Where(x => x.Name != folder.Name))
                {
                    var html = Markdown.ToHtml(file.Content);
                    navCount++;
                    swaggerNav.AppendLine(options.NavBarItemTemplate
                                                 .Replace("{{name}}", file.Name)
                                                 .Replace("{{ref-type}}", "tag")
                                                 .Replace("{{nav-bar-item-wrapper-class}}", " nav-level-1"));

                    html = InsertHeadingTagIds(swaggerNav, file, html, 2);
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", html));
                }
            }

            if (markdown.Files.Any())
            {
                swaggerContent.AppendLine($"<span id='wiki-section-Misc'></span>");
                foreach (var file in markdown.Files)
                {
                    var html = Markdown.ToHtml(file.Content);
                    navCount++;
                    swaggerNav.AppendLine(options.NavBarItemTemplate
                                                 .Replace("{{name}}", file.Name)
                                                 .Replace("{{ref-type}}", "tag")
                                                 .Replace("{{nav-bar-item-wrapper-class}}", " nav-level-0"));
                    html = InsertHeadingTagIds(swaggerNav, file, html, 1);
                    swaggerContent.AppendLine(options.ContentTemplate.Replace("{{name}}", file.Name).Replace("{{content}}", html));
                }
            }

            if (navCount  > 1)
            {
                template = template.Replace("{{wiki_nav_class}}", string.Empty);
            }
            else
            {
                template = template.Replace("{{wiki_nav_class}}", " hide-nav");
            }

            return template.Replace("{{wiki_nav}}", swaggerNav.ToString()).Replace("{{wiki_content}}", swaggerContent.ToString());
        }

        private string InsertHeadingTagIds(StringBuilder swaggerNav, (string Name, string Content) file, string html, int navLevel)
        {
            var headings = Markdown.Parse(file.Content)
                                                       .Where(x => x is HeadingBlock)
                                                       .Cast<HeadingBlock>()
                                                       .ToList();
            foreach (var heading in headings)
            {
                var text = heading.Inline.FirstChild.ToString();
                var section = $"section/{text}";
                html = Regex.Replace(html,
                                     @$"<(h\d)>{Regex.Escape(text)}</\1>",
                                     $@"<$1 id='{section}'>{text}</$1>",
                                     RegexOptions.IgnoreCase);

                swaggerNav.AppendLine(options.NavBarItemTemplate
                                             .Replace("{{name}}", text)
                                             .Replace("{{ref-type}}", "section")
                                             .Replace("{{nav-bar-item-wrapper-class}}", $" nav-level-{navLevel}"));
            }

            return html;
        }
    }
}
