using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public class WikiMarkdownFolder
    {
        public WikiMarkdownFolder(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public List<(string Name, string Content)> Files { get; } = new List<(string Name, string Content)>();
    }
}
