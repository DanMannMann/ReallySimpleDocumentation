using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public class WikiMarkdownOptions
    {
        public string MarkdownFilesPath { get; set; }
        public Dictionary<string, string> MarkdownTemplateVariables { get; } = new Dictionary<string, string>();
        public bool IncludeErrorDetailsInMarkdownOutput { get; set; }
    }
}
