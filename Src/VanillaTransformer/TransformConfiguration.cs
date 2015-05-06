using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VanillaTransformer.Utility;
using VanillaTransformer.ValuesProviders;

namespace VanillaTransformer
{
    public class TransformConfiguration
    {
        public string PatternFile { get; set; }
        public string ValuesSource { get; set; }
        public string Output { get; set; }
    }

    public class TransformConfigurationReader
    {

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

        
        public List<TransformConfiguration> ReadFromFile(string path)
        {
            using (var str = FileReader.ReadFile(path))
            {
                var doc = XDocument.Load(str);
                if (doc.Root == null)
                {
                    throw new InvalidFileStructure("There is no root element");
                }
                var result = doc.Root.Elements()
                    .Where(x => IsElementWithName(x, "transformationGroup"))
                    .Select(x =>
                    {
                        var pattern = x.Attribute("pattern");
                        var transformations = x.Elements()
                            .Where(y => IsElementWithName(y, "transformation"))
                            .Select(y => new TransformConfiguration
                            {
                                PatternFile = pattern.Value,
                                ValuesSource = y.Attribute("values").Value,
                                Output = y.Attribute("output").Value
                            }).ToList();
                        return transformations;
                    });
                return result.SelectMany(x => x).ToList();
            }
        }

        private static bool IsElementWithName(XElement x, string name)
        {
            return x.NodeType == XmlNodeType.Element && string.Equals(x.Name.LocalName, name,StringComparison.InvariantCultureIgnoreCase);
        }
    }
}