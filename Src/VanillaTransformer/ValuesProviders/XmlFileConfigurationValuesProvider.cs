using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VanillaTransformer.Utility;

namespace VanillaTransformer.ValuesProviders
{
    public class XmlFileConfigurationValuesProvider : IValuesProvider
    {
        private string SourceFilePath { get; set; }

        public XmlFileConfigurationValuesProvider(string sourceFilePath)
        {
            SourceFilePath = sourceFilePath;
        }

        private ITextFileReader fileReader;

        public ITextFileReader FileReader
        {
            get
            {
                if (fileReader == null)
                {
                    fileReader = new SimpleTextFileReader();
                }
                return fileReader;
            }
            set { fileReader = value; }
        }

        public IDictionary<string, string> GetValues()
        {
            using (var str = FileReader.ReadFile(SourceFilePath))
            {
                var doc = XDocument.Load(str, LoadOptions.PreserveWhitespace);
                if (doc.Root == null)
                {
                    throw InvalidValuesFileStructure.BecauseMissingRoot(SourceFilePath);
                }

                var result = new Dictionary<string, string>();
                var valueNodes = doc.Root.Elements()
                    .Where(x => x.NodeType == XmlNodeType.Element);

                foreach (var el in valueNodes)
                {
                    var key = el.Name.LocalName;
                    if (result.ContainsKey(key))
                    {
                        throw InvalidValuesFileStructure.BecauseDuplicatedKey(SourceFilePath, key);
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
}