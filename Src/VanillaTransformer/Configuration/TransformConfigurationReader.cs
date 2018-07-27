using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VanillaTransformer.Configuration.PostTransformations;
using VanillaTransformer.PostTransformations;
using VanillaTransformer.Utility;
using VanillaTransformer.ValuesProviders;

namespace VanillaTransformer.Configuration
{
    public class TransformConfigurationReader
    {
        private const string ValuesSourceElementName = "values";

        private const string TransformationGroupElementName = "transformationGroup";

        private const string TransformationElementName = "transformation";

        private const string PatternSourceElementName = "pattern";

        private const string PlaceholderPattern = "placeholderPattern";

        private const string OutputPathElementName = "output";

        private const string OutputArchiveElementName = "archive";

        private const string PostTransformationElementName = "postTransformations";

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

                var rootPostTransformations = GetPostTransformation(doc.Root, new List<IPostTransformation>());
                var result = doc.Root.Elements()
                    .Where(x => x.IsElementWithName(TransformationGroupElementName))
                    .Select(x =>
                    {
                        var groupPostTransformations = GetPostTransformation(x, rootPostTransformations);
                        var pattern = x.Attribute(PatternSourceElementName);
                        var placeholderPattern = x.Attribute(PlaceholderPattern);

                        var transformations = x.Elements()
                            .Where(y => y.IsElementWithName(TransformationElementName))
                            .Select(y => new TransformConfiguration
                            {
                                PatternFilePath = pattern.Value,
                                PlaceholderPattern = placeholderPattern?.Value,
                                OutputFilePath = y.Attribute(OutputPathElementName).Value,
                                OutputArchive = y.Attribute(OutputArchiveElementName)?.Value,
                                ValuesProvider = CreateValuesProvider(y),
                                PostTransformations = GetPostTransformation(y, groupPostTransformations)
                            }).ToList();
                        return transformations;
                    });
                return result.SelectMany(x => x).ToList();
            }
        }

        private List<IPostTransformation> GetPostTransformation(XElement node, List<IPostTransformation> inheritedPostTransformations)
        {
            var configuration = GetPostTransformationsConfiguration(node);
            var result = inheritedPostTransformations.ToList();
            foreach (var operation in configuration)
            {
                operation.Execute(result);
            }
            return result;
        }

        private List<IPostTransformationsConfigurationOperation> GetPostTransformationsConfiguration(XElement node)
        {
            try
            {
                var postTransformationsNode = node.Elements()
                    .SingleOrDefault(x => x.IsElementWithName(PostTransformationElementName));

                if (postTransformationsNode == null)
                {
                    return new List<IPostTransformationsConfigurationOperation>();
                }

                return postTransformationsNode.Elements()
                    .Select(PostTransformationsConfigurationOperationFactory.Create)
                    .ToList();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidFileStructure("Only one 'PostTransformations' element is allowed on given level");
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