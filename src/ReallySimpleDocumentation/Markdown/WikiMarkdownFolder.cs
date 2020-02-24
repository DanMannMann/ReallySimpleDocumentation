using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class WikiMarkdownFolder : WikiMarkdownNode, IList<WikiMarkdownFile>
    {
        private List<WikiMarkdownFile> files = new List<WikiMarkdownFile>();

        public WikiMarkdownFolder(string name)
            : base(name)
        {
        }

        public void Sort()
        {
            files = files.OrderBy(x => x.Order).ToList();
        }

        public WikiMarkdownFile this[int index] { get => ((IList<WikiMarkdownFile>)files)[index]; set => ((IList<WikiMarkdownFile>)files)[index] = value; }

        public int Count { get => ((IList<WikiMarkdownFile>)files).Count; }
        public bool IsReadOnly { get => ((IList<WikiMarkdownFile>)files).IsReadOnly; }

        public void Add(WikiMarkdownFile item)
        {
            ((IList<WikiMarkdownFile>)files).Add(item);
        }

        public void Clear()
        {
            ((IList<WikiMarkdownFile>)files).Clear();
        }

        public bool Contains(WikiMarkdownFile item)
        {
            return ((IList<WikiMarkdownFile>)files).Contains(item);
        }

        public void CopyTo(WikiMarkdownFile[] array, int arrayIndex)
        {
            ((IList<WikiMarkdownFile>)files).CopyTo(array, arrayIndex);
        }

        public IEnumerator<WikiMarkdownFile> GetEnumerator()
        {
            return ((IList<WikiMarkdownFile>)files).GetEnumerator();
        }

        public int IndexOf(WikiMarkdownFile item)
        {
            return ((IList<WikiMarkdownFile>)files).IndexOf(item);
        }

        public void Insert(int index, WikiMarkdownFile item)
        {
            ((IList<WikiMarkdownFile>)files).Insert(index, item);
        }

        public bool Remove(WikiMarkdownFile item)
        {
            return ((IList<WikiMarkdownFile>)files).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<WikiMarkdownFile>)files).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<WikiMarkdownFile>)files).GetEnumerator();
        }
    }

    public class WikiMarkdownFile : WikiMarkdownNode
    {
        public WikiMarkdownFile(string name, string content)
            : base(name)
        {
            Content = content;
        }

        public string Content { get; set; }
    }

    public class WikiMarkdownNode
    {
        public WikiMarkdownNode(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public int Order { get; set; }
    }
}
