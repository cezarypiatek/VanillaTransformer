using System;
using System.Collections.Generic;
using System.IO;
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

        
        public List<TransformConfiguration> ReadFromFile(string path, string rootPath = "")
        {
            using (var str = FileReader.ReadFile(path))
            {
                var doc = XDocument.Load(str);
                if (doc.Root == null)
                {
                    throw InvalidConfigurationFile.BecauseMissingRoot(path);
                }

                var rootPostTransformations = GetPostTransformation(doc.Root, new List<IPostTransformation>(), path);
                var result = doc.Root.Elements()
                    .Where(x => x.IsElementWithName(TransformationGroupElementName))
                    .Select(x =>
                    {
                        var groupPostTransformations = GetPostTransformation(x, rootPostTransformations, path);
                        var pattern = x.Attribute(PatternSourceElementName)?.Value;

                        if (string.IsNullOrWhiteSpace(pattern))
                        {
                            throw InvalidConfigurationFile.BecauseMissingPattern(path);
                        }

                        var placeholderPattern = x.Attribute(PlaceholderPattern)?.Value;

                        var transformations = x.Elements()
                            .Where(y => y.IsElementWithName(TransformationElementName))
                            .Select(y =>
                            {
                                var valuesProvider = CreateValuesProvider(y, rootPath);
                                if (valuesProvider == null)
                                {
                                    throw InvalidConfigurationFile.BecauseMissingValuesSource(path, pattern);
                                }

                                var outputFilePath = y.Attribute(OutputPathElementName)?.Value;

                                if (string.IsNullOrWhiteSpace(outputFilePath))
                                {
                                    throw InvalidConfigurationFile.BecauseMissingOutput(path, pattern);
                                }

                                return new TransformConfiguration
                                {
                                    PlaceholderPattern = placeholderPattern,
                                    PatternFilePath = pattern,
                                    OutputFilePath = outputFilePath,
                                    OutputArchive = y.Attribute(OutputArchiveElementName)?.Value,
                                    ValuesProvider = valuesProvider,
                                    PostTransformations = GetPostTransformation(y, groupPostTransformations, path)
                                };
                            }).ToList();
                        return transformations;
                    });
                return result.SelectMany(x => x).ToList();
            }
        }

        private List<IPostTransformation> GetPostTransformation(XElement node,
            List<IPostTransformation> inheritedPostTransformations, string path)
        {
            var configuration = GetPostTransformationsConfiguration(node, path);
            var result = inheritedPostTransformations.ToList();
            foreach (var operation in configuration)
            {
                operation.Execute(result);
            }
            return result;
        }

        private List<IPostTransformationsConfigurationOperation> GetPostTransformationsConfiguration(XElement node, string path)
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
                throw InvalidConfigurationFile.BecauseDuplicatedPostTransformations(path);
            }
        }

        private static IValuesProvider CreateValuesProvider(XElement y, string rootPath)
        {
            if (y.Attribute(ValuesSourceElementName) != null)
            {
                return new XmlFileConfigurationValuesProvider(UpdatePathWithRootPath(y.Attribute(ValuesSourceElementName).Value,rootPath));
            }

            if (y.Element(ValuesSourceElementName) != null)
            {
                return new XmlInlineConfigurationValuesProvider(y.Element(ValuesSourceElementName));
            }

            return null;
        }

        private static string UpdatePathWithRootPath(string path, string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath) == false)
            {
                return Path.Combine(rootPath, path);
            }
            return path;
        }
    }
}