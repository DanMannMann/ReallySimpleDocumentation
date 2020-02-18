using System;
using System.Xml.XPath;

namespace Marsman.ReallySimpleDocumentation
{
    public interface IXmlDocProvider
    {
        XPathNavigator GetDocumentForType(Type type);
        XPathNavigator GetNodeForType(Type type);
    }
}
