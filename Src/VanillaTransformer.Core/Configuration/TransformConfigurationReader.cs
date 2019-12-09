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

                var valuesGroups = ReadValuesGroups(path, doc.Root);

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
                                var valuesProvider = CreateValuesProvider(y, rootPath, valuesGroups, path);
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

        private static Dictionary<string, XmlInlineConfigurationValuesProvider> ReadValuesGroups(string path, XElement rootElement)
        {
            return rootElement.Elements()
                .Where(x => x.IsElementWithName(ValuesGroupElementName))
                .Select(x =>
                {
                    var groupName = x.Attribute("name")?.Value;
                    if (string.IsNullOrWhiteSpace(groupName))
                    {
                        throw InvalidConfigurationFile.BecauseMissingGroupName(path);
                    }

                    var valuesProvider = new XmlInlineConfigurationValuesProvider(x);
                    return new
                    {
                        groupName,
                        valuesProvider
                    };
                }).ToDictionary(el => el.groupName, el => el.valuesProvider);
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

        private IValuesProvider CreateValuesProvider(XElement node, string rootPath, Dictionary<string, XmlInlineConfigurationValuesProvider> valuesGroups, string configPath)
        {
            var underlyingProviders = GetValuesProviders(node, rootPath, valuesGroups, configPath).ToList();
            if (underlyingProviders.Count > 0)
            {
                return new CompositeValuesProvider(underlyingProviders);
            }
            return null;
        }  
        
        private IEnumerable<IValuesProvider> GetValuesProviders(XElement node, string rootPath, Dictionary<string, XmlInlineConfigurationValuesProvider> valuesGroups, string configPath)
        {
            if (node.Element(ValuesSourceElementName) != null)
            {
                yield return new XmlInlineConfigurationValuesProvider(node.Element(ValuesSourceElementName));
            }

            var externalValuesFile = node.Attribute(ValuesSourceElementName)?.Value;
            if (string.IsNullOrWhiteSpace(externalValuesFile) == false)
            {
                var fileFullPath = UpdatePathWithRootPath(externalValuesFile,rootPath);
                if (FileReader.FileExists(fileFullPath) == false)
                {
                    throw InvalidConfigurationFile.BecauseIncludedFileDoesNotExist(configPath, fileFullPath);
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
                    throw InvalidConfigurationFile.BecauseUnknowValuesGroup(configPath, valuesGroupName);
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