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
                var doc = XDocument.Load(str);
                if (doc.Root == null)
                {
                    throw new InvalidFileStructure("There is no root element");
                }
                var result = doc.Root.Elements()
                    .Where(x => x.NodeType == XmlNodeType.Element)
                    .ToDictionary(el => el.Name.LocalName, el => el.GetInnerXmlAsText());
                return result;
            }
        }
    }
}