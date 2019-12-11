using System.Xml.Linq;
using VanillaTransformer.Core.Utility;

namespace VanillaTransformer.Core.Configuration
{
    public class XmlTextFileReader : IXmlTextFileReader
    {
        private readonly ITextFileReader textFileReader;

        public XmlTextFileReader(ITextFileReader textFileReader)
        {
            this.textFileReader = textFileReader;
        }

        public XElement Read(string path)
        {
            using (var str = textFileReader.ReadFile(path))
            {
                var doc = XDocument.Load(str, LoadOptions.PreserveWhitespace);
                if (doc.Root == null)
                {
                    throw InvalidConfigurationException.BecauseMissingRoot();
                }

                return doc.Root;
            }
        }
    }
}