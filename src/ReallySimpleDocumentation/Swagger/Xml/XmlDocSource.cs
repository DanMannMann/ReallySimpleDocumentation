using System;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace Marsman.ReallySimpleDocumentation
{
    public class XmlDocSource
    {
        private readonly Lazy<XPathDocument> document;
        private const string NodeXPath = "/doc/members/member[@name='{0}']";

        public XmlDocSource(string filePath)
        {
            FilePath = filePath;
            document = new Lazy<XPathDocument>(LoadDocument);
        }

        public string FilePath { get; }
        public XPathDocument Document { get => document.Value; }

        private XPathDocument LoadDocument()
        {
            return new XPathDocument(FilePath);
        }

        public bool HasType(Type type, out XPathNavigator docNode, out XPathNavigator typeNode, out string typeNodeName)
        {
            var name = GetMemberNameForType(type);
            docNode = Document.CreateNavigator();
            typeNode = docNode?.SelectSingleNode(string.Format(NodeXPath, name));
            if (typeNode != null)
            {
                typeNodeName = name;
                return true;
            }
            typeNodeName = null;
            return false;
        }

        private static string GetMemberNameForType(Type type)
        {
            var builder = new StringBuilder("T:");
            builder.Append(QualifiedNameFor(type));

            return builder.ToString();
        }

        private static string QualifiedNameFor(Type type, bool expandGenericArgs = false)
        {
            if (type.IsArray)
                return $"{QualifiedNameFor(type.GetElementType(), expandGenericArgs)}[]";

            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(type.Namespace))
                builder.Append($"{type.Namespace}.");

            if (type.IsNested)
                builder.Append($"{type.DeclaringType.Name}.");

            if (type.IsConstructedGenericType && expandGenericArgs)
            {
                var nameSansGenericArgs = type.Name.Split('`').First();
                builder.Append(nameSansGenericArgs);

                var genericArgsNames = type.GetGenericArguments().Select(t =>
                {
                    return t.IsGenericParameter
                        ? $"`{t.GenericParameterPosition}"
                        : QualifiedNameFor(t, true);
                });

                builder.Append($"{{{string.Join(",", genericArgsNames)}}}");
            }
            else
            {
                builder.Append(type.Name);
            }

            return builder.ToString();
        }
    }
}
