using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class WikiMarkdown : IList<WikiMarkdownNode>
    {
        private List<WikiMarkdownNode> backingList = new List<WikiMarkdownNode>();

        public WikiMarkdownNode this[int index] { get => backingList[index]; set => backingList[index] = value; }

        public void Sort()
        {
            backingList = backingList.OrderBy(x => x.Order).ToList();
        }

        public int Count { get => backingList.Count; }

        public bool IsReadOnly { get => ((IList<WikiMarkdownNode>)backingList).IsReadOnly; }

        public void Add(WikiMarkdownNode item)
        {
            backingList.Add(item);
        }

        public void Clear()
        {
            backingList.Clear();
        }

        public bool Contains(WikiMarkdownNode item)
        {
            return backingList.Contains(item);
        }

        public void CopyTo(WikiMarkdownNode[] array, int arrayIndex)
        {
            backingList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WikiMarkdownNode> GetEnumerator()
        {
            return ((IList<WikiMarkdownNode>)backingList).GetEnumerator();
        }

        public int IndexOf(WikiMarkdownNode item)
        {
            return backingList.IndexOf(item);
        }

        public void Insert(int index, WikiMarkdownNode item)
        {
            backingList.Insert(index, item);
        }

        public bool Remove(WikiMarkdownNode item)
        {
            return backingList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            backingList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<WikiMarkdownNode>)backingList).GetEnumerator();
        }
    }
}