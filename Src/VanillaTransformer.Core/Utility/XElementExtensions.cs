using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VanillaTransformer.Core.Configuration;

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

        public static string GetRequiredAttribute(this XElement x, string name)
        {
            var value = x.Attribute(name)?.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidConfigurationException($"Missing '{name}' attribute value for '{x.Name}' element");

            }
            return value;
        }

        public static XElement GetRequiredElement(this XElement x, string name)
        {
            var element = x.Element(name);
            if (element == null)
            {
                throw new InvalidConfigurationException($"Missing '{name}' node for '{x.Name}' element");
            }

            return element;
        }

        public static IReadOnlyList<XElement> GetChildren(this XElement x, string parent, string child, bool isRequired)
        {
            var parentNode = x.Element(parent);
            if (parentNode == null)
            {
                if (isRequired)
                {
                    throw new InvalidConfigurationException($"Missing '{parent}' element");
                }
                return new XElement[0];
            }
            return parentNode.Elements(child).ToList();
        }
    }
}