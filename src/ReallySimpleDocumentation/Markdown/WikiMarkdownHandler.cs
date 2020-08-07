using Microsoft.Extensions.Options;
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
            try
            {
                if (!Directory.Exists(this.options.MarkdownFilesPath))
                {

                    var r = new WikiMarkdown();
                    if (options.IncludeErrorDetailsInMarkdownOutput)
                    {
                        r.Add(new WikiMarkdownFile("Error", $"<div style='color: red; font-size: large;'>Markdown files path {this.options.MarkdownFilesPath} not found</div>"));
                        return r;
                    }
                    r.Add(new WikiMarkdownFile("Error", $"<div style='color: red; font-size: large;'>Markdown files path not found</div>"));
                    return r;
                }
                var result = new WikiMarkdown();
                var files = Directory.EnumerateFiles(this.options.MarkdownFilesPath).Where(x => x.ToLowerInvariant().EndsWith(".md")).OrderBy(x => x).ToList();
                var folders = Directory.EnumerateDirectories(this.options.MarkdownFilesPath).OrderBy(x => x).ToList();
                string[] rootOrder;

                if (File.Exists(@$"{this.options.MarkdownFilesPath}\.order"))
                {
                    rootOrder = File.ReadAllText(@$"{this.options.MarkdownFilesPath}\.order").Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    rootOrder = new string[0];
                }

                ProcessMarkdownFolders(result, folders, rootOrder);
                ProcessMarkdownFiles(result, files, rootOrder);
                result.Sort();
                return result;
            }
            catch (Exception ex)
            {
                var r = new WikiMarkdown();
                var msg = ex.Message;
                if (ex is AggregateException aex)
                {
                    ex = aex.Flatten();
                    var msgs = aex.InnerExceptions?.Select(x => x?.Message)?.ToList();
                    if (msgs?.Any() == true) msg = string.Join(", ", msgs); 
                    else msg = aex.InnerException?.Message ?? msg;
                }
                
                if (options.IncludeErrorDetailsInMarkdownOutput)
                {
                    r.Add(new WikiMarkdownFile("Error", $"<div style='color: red; font-size: large;'>Unexpected error during markdown rendering: {ex.ToString()}.</div>"));
                    return r;
                }

                r.Add(new WikiMarkdownFile("Error", $"<div style='color: red; font-size: large;'>Unexpected error during markdown rendering: {msg}. Set the option IncludeErrorDetailsInMarkdownOutput to include full error details in these messages.</div>"));
                r.Sort();
                return r;
            }
        }

        private void ProcessMarkdownFolders(WikiMarkdown result, IList<string> folders, IList<string> rootOrder)
        {
            foreach (var folder in folders)
            {
                var markdownFolder = new WikiMarkdownFolder(Path.GetFileNameWithoutExtension(folder).Replace("-", " "));
                markdownFolder.Order = rootOrder.Contains(markdownFolder.Name) ? rootOrder.IndexOf(markdownFolder.Name) : int.MaxValue;
                result.Add(markdownFolder);

                var subFiles = Directory.EnumerateFiles(folder).Where(x => x.ToLowerInvariant().EndsWith(".md")).OrderBy(x => x).ToList();
                IList<string> subFileOrder;
                string orderPath = Path.Combine(folder, ".order");
                if (File.Exists(orderPath))
                {
                    subFileOrder = File.ReadAllText(orderPath).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    subFileOrder = new List<string>();
                }

                foreach (var file in subFiles)
                {
                    var name = Path.GetFileNameWithoutExtension(file).Replace("-", " ");
                    var markdownFile = new WikiMarkdownFile(name, ReplaceTemplateVariables(File.ReadAllText(file)));
                    markdownFile.Order = subFileOrder.Contains(markdownFile.Name) ? subFileOrder.IndexOf(markdownFile.Name) : int.MaxValue;
                    markdownFolder.Add(markdownFile);
                }
                markdownFolder.Sort();
            }
        }

        private void ProcessMarkdownFiles(WikiMarkdown result, IList<string> files, IList<string> rootOrder)
        {
            if (files.Any())
            {
                foreach (var file in files)
                {
                    var name = Path.GetFileNameWithoutExtension(file).Replace("-", " ");
                    var markdownFile = new WikiMarkdownFile(name, ReplaceTemplateVariables(File.ReadAllText(file)));
                    markdownFile.Order = rootOrder.Contains(markdownFile.Name) ? rootOrder.IndexOf(markdownFile.Name) : int.MaxValue;
                    result.Add(markdownFile);
                }
            }
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
