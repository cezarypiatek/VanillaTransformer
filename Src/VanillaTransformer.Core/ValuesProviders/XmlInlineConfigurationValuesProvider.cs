using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VanillaTransformer.Core.Utility;

namespace VanillaTransformer.Core.ValuesProviders
{
    public class XmlInlineConfigurationValuesProvider:IValuesProvider
    {
        private readonly XElement inlineValues;

        public XmlInlineConfigurationValuesProvider(XElement inlineValues)
        {
            this.inlineValues = inlineValues;
        }

        public IDictionary<string, string> GetValues()
        {
            var result = inlineValues.Elements()
                .Where(x => x.NodeType == XmlNodeType.Element)
                .ToDictionary(el =>
                {
                    var keyAttributeValue = el.Attribute("key")?.Value;
                    if (el.Name.LocalName == "value" && string.IsNullOrWhiteSpace(keyAttributeValue) == false)
                    {
                        return keyAttributeValue;
                    }
                    return el.Name.LocalName;
                }, el => el.GetInnerXmlAsText());
            return result;
        }
    }
}