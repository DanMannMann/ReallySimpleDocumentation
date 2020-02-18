using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Marsman.ReallySimpleDocumentation
{
    public class WikiMarkdownHandler : IWikiMarkdownHandler
    {
        private readonly WikiMarkdownOptions options;
        private readonly Lazy<WikiMarkdown> value;

        public WikiMarkdownHandler(IOptions<WikiMarkdownOptions> options)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.value = new Lazy<WikiMarkdown>(BuildMarkdownFiles);
        }

        public WikiMarkdown GetMarkdownDocuments()
        {
            return value.Value;
        }

        private WikiMarkdown BuildMarkdownFiles()
        {
            var result = new WikiMarkdown();
            var files = Directory.EnumerateFiles(this.options.MarkdownFilesPath).Where(x => x.ToLowerInvariant().EndsWith(".md")).OrderBy(x => x);
            var folders = Directory.EnumerateDirectories(this.options.MarkdownFilesPath).OrderBy(x => x);
            string[] rootOrder;
            if (File.Exists(@$"{this.options.MarkdownFilesPath}\.order"))
            {
                rootOrder = File.ReadAllText(@$"{this.options.MarkdownFilesPath}\.order").Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                rootOrder = new string[0];
            }

            var orderedFolders = new List<string>();
            foreach (var orderName in rootOrder)
            {
                var match = folders.FirstOrDefault(x => string.Equals(Path.GetFileNameWithoutExtension(x), orderName, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    orderedFolders.Add(match);
                }
            }
            foreach (var folder in folders)
            {
                if (!orderedFolders.Contains(folder))
                {
                    orderedFolders.Add(folder);
                }
            }
            foreach (var folder in orderedFolders)
            {
                var markdownFolder = new WikiMarkdownFolder(Path.GetFileNameWithoutExtension(folder).Replace("-", " "));
                result.Folders.Add(markdownFolder);
                var subFiles = Directory.EnumerateFiles(folder).Where(x => x.ToLowerInvariant().EndsWith(".md")).OrderBy(x => x);
                var orderedSubFiles = new List<string>();
                string[] subFileOrder;
                string orderPath = Path.Combine(folder, ".order");
                if (File.Exists(orderPath))
                {
                    subFileOrder = File.ReadAllText(orderPath).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    subFileOrder = new string[0];
                }

                foreach (var orderName in subFileOrder)
                {
                    var match = subFiles.FirstOrDefault(x => string.Equals(Path.GetFileNameWithoutExtension(x), orderName, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        orderedSubFiles.Add(match);
                    }
                }
                foreach (var subFile in subFiles)
                {
                    if (!orderedSubFiles.Contains(subFile))
                    {
                        orderedSubFiles.Add(subFile);
                    }
                }

                foreach (var file in orderedSubFiles)
                {
                    var name = Path.GetFileNameWithoutExtension(file).Replace("-", " ");
                    markdownFolder.Files.Add((name, ReplaceTemplateVariables(File.ReadAllText(file))));
                }
            }


            if (files.Any())
            {
                var orderedFiles = new List<string>();
                foreach (var orderName in rootOrder)
                {
                    var match = files.FirstOrDefault(x => string.Equals(Path.GetFileNameWithoutExtension(x), orderName, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        orderedFiles.Add(match);
                    }
                }
                foreach (var file in files)
                {
                    if (!orderedFiles.Contains(file))
                    {
                        orderedFiles.Add(file);
                    }
                }

                foreach (var file in orderedFiles)
                {
                    var name = Path.GetFileNameWithoutExtension(file).Replace("-", " ");
                    result.Files.Add((name, ReplaceTemplateVariables(File.ReadAllText(file))));
                }
            }

            return result;
        }

        private string ReplaceTemplateVariables(string markdown)
        {
            foreach (var @var in options.MarkdownTemplateVariables)
            {
                markdown = Regex.Replace(
                    markdown,
                    Regex.Escape(var.Key),
                    var.Value.Replace("$", "$$"),
                    RegexOptions.IgnoreCase
                );
            }
            return markdown;
        }
    }
}
