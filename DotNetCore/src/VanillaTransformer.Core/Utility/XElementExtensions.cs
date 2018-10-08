using System;
using System.Xml;
using System.Xml.Linq;

namespace VanillaTransformer.Core.Utility
{
    public static class XElementExtensions
    {
        public static string GetInnerXmlAsText(this XElement el)
        {
            var reader = el.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        public static bool IsElementWithName(this XElement x, string name)
        {
            return x.NodeType == XmlNodeType.Element && string.Equals(x.Name.LocalName, name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}