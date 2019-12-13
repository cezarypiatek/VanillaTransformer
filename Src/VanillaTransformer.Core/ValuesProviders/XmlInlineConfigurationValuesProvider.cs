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
            if (inlineValues == null)
            {
                return new Dictionary<string, string>();
            }

            var result = inlineValues.Elements()
                .Where(x => x.NodeType == XmlNodeType.Element)
                .ToDictionary(el =>
                {
                    var keyAttributeValue = el.Attribute("key")?.Value;
                    var valueName = el.Name.LocalName;
                    if ((valueName == "add" || valueName == "value") && string.IsNullOrWhiteSpace(keyAttributeValue) == false)
                    {
                        return keyAttributeValue;
                    }
                    return valueName;
                }, el => el.GetInnerXmlAsText());
            return result;
        }
    }
}