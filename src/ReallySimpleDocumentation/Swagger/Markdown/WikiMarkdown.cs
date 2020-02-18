using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public class WikiMarkdown
    {
        public List<WikiMarkdownFolder> Folders { get; } = new List<WikiMarkdownFolder>();
        public List<(string Name, string Content)> Files { get; } = new List<(string Name, string Content)>();
    }
}