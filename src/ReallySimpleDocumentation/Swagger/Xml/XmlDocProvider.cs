using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Marsman.ReallySimpleDocumentation
{
    public class XmlDocProvider : IXmlDocProvider
    {
        private readonly IEnumerable<XmlDocSource> sources;

        public XmlDocProvider(IEnumerable<XmlDocSource> sources)
        {
            this.sources = sources;
        }

        public XPathNavigator GetDocumentForType(Type type, out XPathNavigator typeNode, out string typeNodeName)
        {
            foreach (var source in sources)
            {
                if (source.HasType(type, out var docNode, out typeNode, out typeNodeName))
                {
                    return docNode;
                }
            }
            typeNodeName = null;
            typeNode = null;
            return null;
        }

        public XPathNavigator GetDocumentForType(Type type)
        {
            foreach (var source in sources)
            {
                if (source.HasType(type, out var docNode, out var typeNode, out _))
                {
                    return docNode;
                }
            }
            return null;
        }

        public XPathNavigator GetNodeForType(Type type)
        {
            foreach (var source in sources)
            {
                if (source.HasType(type, out var docNode, out var typeNode, out _))
                {
                    return typeNode;
                }
            }
            return null;
        }
    }
}
