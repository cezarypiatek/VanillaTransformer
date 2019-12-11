using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VanillaTransformer.Core.Configuration.PostTransformations;
using VanillaTransformer.Core.PostTransformations;
using VanillaTransformer.Core.Utility;
using VanillaTransformer.Core.ValuesProviders;

namespace VanillaTransformer.Core.Configuration
{
    public class TransformConfigurationReader
    {
        private const string ValuesSourceElementName = "values";

        private const string TransformationGroupElementName = "transformationGroup";
        
        private const string ValuesGroupElementName = "valuesGroup";

        private const string TransformationElementName = "transformation";

        private const string PatternSourceElementName = "pattern";

        private const string PlaceholderPattern = "placeholderPattern";

        private const string OutputPathElementName = "output";

        private const string OutputArchiveElementName = "archive";

        private const string PostTransformationElementName = "postTransformations";

        private readonly ITextFileReader fileReader;
        private readonly string configFilePath;
        private readonly string rootPath;

        public TransformConfigurationReader(ITextFileReader textFileReader, string path, string rootPath = "")
        {
            this.configFilePath = path;
            this.rootPath = rootPath;
            this.fileReader = textFileReader;
        }

        public List<TransformConfiguration> ReadConfig()
        {
            using (var str = fileReader.ReadFile(configFilePath))
            {
                var doc = XDocument.Load(str);
                if (doc.Root == null)
                {
                    throw InvalidConfigurationFile.BecauseMissingRoot(configFilePath);
                }

                var valuesGroups = ReadValuesGroups(doc.Root);

                var rootPostTransformations = GetPostTransformation(doc.Root, new List<IPostTransformation>());
                var result = doc.Root.Elements()
                    .Where(x => x.IsElementWithName(TransformationGroupElementName))
                    .Select(x =>
                    {
                        var groupPostTransformations = GetPostTransformation(x, rootPostTransformations);
                        var pattern = x.Attribute(PatternSourceElementName)?.Value;

                        if (string.IsNullOrWhiteSpace(pattern))
                        {
                            throw InvalidConfigurationFile.BecauseMissingPattern(configFilePath);
                        }

                        var placeholderPattern = x.Attribute(PlaceholderPattern)?.Value;

                        var transformations = x.Elements()
                            .Where(y => y.IsElementWithName(TransformationElementName))
                            .Select(y =>
                            {
                                var valuesProvider = CreateValuesProvider(y, valuesGroups);
                                if (valuesProvider == null)
                                {
                                    throw InvalidConfigurationFile.BecauseMissingValuesSource(configFilePath, pattern);
                                }

                                var outputFilePath = y.Attribute(OutputPathElementName)?.Value;

                                if (string.IsNullOrWhiteSpace(outputFilePath))
                                {
                                    throw InvalidConfigurationFile.BecauseMissingOutput(configFilePath, pattern);
                                }

                                return new TransformConfiguration
                                {
                                    PlaceholderPattern = placeholderPattern,
                                    PatternFilePath = pattern,
                                    OutputFilePath = outputFilePath,
                                    OutputArchive = y.Attribute(OutputArchiveElementName)?.Value,
                                    ValuesProvider = valuesProvider,
                                    PostTransformations = GetPostTransformation(y, groupPostTransformations)
                                };
                            }).ToList();
                        return transformations;
                    });
                return result.SelectMany(x => x).ToList();
            }
        }

        private  Dictionary<string, XmlInlineConfigurationValuesProvider> ReadValuesGroups(XElement rootElement)
        {
            return rootElement.Elements()
                .Where(x => x.IsElementWithName(ValuesGroupElementName))
                .Select(x =>
                {
                    var groupName = x.Attribute("name")?.Value;
                    if (string.IsNullOrWhiteSpace(groupName))
                    {
                        throw InvalidConfigurationFile.BecauseMissingGroupName(configFilePath);
                    }

                    var valuesProvider = new XmlInlineConfigurationValuesProvider(x);
                    return new
                    {
                        groupName,
                        valuesProvider
                    };
                }).ToDictionary(el => el.groupName, el => el.valuesProvider);
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
                throw InvalidConfigurationFile.BecauseDuplicatedPostTransformations(configFilePath);
            }
        }

        private IValuesProvider CreateValuesProvider(XElement node, Dictionary<string, XmlInlineConfigurationValuesProvider> valuesGroups)
        {
            var underlyingProviders = GetValuesProviders(node, valuesGroups).ToList();
            if (underlyingProviders.Count > 0)
            {
                return new CompositeValuesProvider(underlyingProviders);
            }
            return null;
        }  
        
        private IEnumerable<IValuesProvider> GetValuesProviders(XElement node, Dictionary<string, XmlInlineConfigurationValuesProvider> valuesGroups)
        {
            if (node.Element(ValuesSourceElementName) != null)
            {
                yield return new XmlInlineConfigurationValuesProvider(node.Element(ValuesSourceElementName));
            }

            var externalValuesFile = node.Attribute(ValuesSourceElementName)?.Value;
            if (string.IsNullOrWhiteSpace(externalValuesFile) == false)
            {
                var fileFullPath = UpdatePathWithRootPath(externalValuesFile, rootPath);
                if (fileReader.FileExists(fileFullPath) == false)
                {
                    throw InvalidConfigurationFile.BecauseIncludedFileDoesNotExist(configFilePath, fileFullPath);
                }
                yield return new XmlFileConfigurationValuesProvider(fileFullPath);
            }

            var valuesGroupName = node.Attribute("valuesGroup")?.Value;
            if (string.IsNullOrWhiteSpace(valuesGroupName) == false)
            {
                if (valuesGroups.TryGetValue(valuesGroupName, out var group))
                {
                    yield return group;
                }
                else
                {
                    throw InvalidConfigurationFile.BecauseUnknowValuesGroup(configFilePath, valuesGroupName);
                }
            }
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