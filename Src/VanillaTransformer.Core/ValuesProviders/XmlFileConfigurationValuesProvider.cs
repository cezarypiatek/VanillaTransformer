using System.Collections.Generic;
using System.Linq;
using System.Xml;
using VanillaTransformer.Core.Configuration;
using VanillaTransformer.Core.Utility;

namespace VanillaTransformer.Core.ValuesProviders
{
    public class XmlFileConfigurationValuesProvider : IValuesProvider
    {
        private readonly string sourceFilePath;
        private readonly IXmlTextFileReader xmlTextFileReader;

        public XmlFileConfigurationValuesProvider(string sourceFilePath, IXmlTextFileReader xmlTextFileReader)
        {
            this.sourceFilePath = sourceFilePath;
            this.xmlTextFileReader = xmlTextFileReader;
        }

        public IDictionary<string, string> GetValues()
        {
            var doc = xmlTextFileReader.Read(sourceFilePath);
            var result = new Dictionary<string, string>();
            var valueNodes = doc.Elements()
                .Where(x => x.NodeType == XmlNodeType.Element);

            foreach (var el in valueNodes)
            {
                var key = el.Name.LocalName;
                if (result.ContainsKey(key))
                {
                    throw InvalidValuesFileStructure.BecauseDuplicatedKey(sourceFilePath, key);
                }
                var value = el.NodeType == XmlNodeType.Element 
                    ? el.GetInnerXmlAsText().Replace("\n", "\r\n") 
                    : el.Value;
                result.Add(key, value);
            }
            return result;
        }
    }
}