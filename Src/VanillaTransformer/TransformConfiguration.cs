using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VanillaTransformer.Utility;
using VanillaTransformer.ValuesProviders;

namespace VanillaTransformer
{
    public class TransformConfiguration
    {
        public string PatternFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public IValuesProvider ValuesProvider { get; set; }
    }

    public class TransformConfigurationReader
    {
        private const string ValuesSourceElementName = "values";

        private const string TransformationGroupElementName = "transformationGroup";

        private const string TransformationElementName = "transformation";

        private const string PatternSourceElementName = "pattern";

        private const string OutputPathElementName = "output";


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
                    .Where(x => x.IsElementWithName(TransformationGroupElementName))
                    .Select(x =>
                    {
                        var pattern = x.Attribute(PatternSourceElementName);
                        var transformations = x.Elements()
                            .Where(y => y.IsElementWithName(TransformationElementName))
                            .Select(y => new TransformConfiguration
                            {
                                PatternFilePath = pattern.Value,
                                OutputFilePath = y.Attribute(OutputPathElementName).Value,
                                ValuesProvider = CreateValuesProvider(y)
                            }).ToList();
                        return transformations;
                    });
                return result.SelectMany(x => x).ToList();
            }
        }

        private static IValuesProvider CreateValuesProvider(XElement y)
        {
            if (y.Attribute(ValuesSourceElementName) != null)
            {
                return new XmlFileConfigurationValuesProvider(y.Attribute(ValuesSourceElementName).Value);
            }

            if (y.Element(ValuesSourceElementName) != null)
            {
                return new XmlInlineConfigurationValuesProvider(y.Element(ValuesSourceElementName));
            }

            throw new InvalidFileStructure("There is no values source defined for transformation");
        }
    }
}