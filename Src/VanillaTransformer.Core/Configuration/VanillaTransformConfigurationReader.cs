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
    public class VanillaTransformConfigurationReader : ITransformConfigurationReader
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

        private readonly IFileSystem fileSystem;
        private readonly IXmlTextFileReader xmlReader;
        private readonly string rootPath;

        public VanillaTransformConfigurationReader(IFileSystem fileSystem, IXmlTextFileReader xmlReader, string rootPath = "")
        {
            this.fileSystem = fileSystem;
            this.xmlReader = xmlReader;
            this.rootPath = rootPath;
        }

        public List<TransformConfiguration> ReadConfig(string path)
        {
            var doc = xmlReader.Read(path);
            var valuesGroups = ReadValuesGroups(doc);
            var rootPostTransformations = GetPostTransformation(doc, new List<IPostTransformation>());
            var result = doc.Elements()
                .Where(x => x.IsElementWithName(TransformationGroupElementName))
                .Select(x =>
                {
                    var groupPostTransformations = GetPostTransformation(x, rootPostTransformations);
                    var pattern = x.Attribute(PatternSourceElementName)?.Value;

                    if (string.IsNullOrWhiteSpace(pattern))
                    {
                        throw InvalidConfigurationException.BecauseMissingPattern();
                    }

                    var placeholderPattern = x.Attribute(PlaceholderPattern)?.Value;

                    var transformations = x.Elements()
                        .Where(y => y.IsElementWithName(TransformationElementName))
                        .Select(y =>
                        {
                            var valuesProvider = CreateValuesProvider(y, valuesGroups);
                            if (valuesProvider == null)
                            {
                                throw InvalidConfigurationException.BecauseMissingValuesSource(pattern);
                            }

                            var outputFilePath = y.Attribute(OutputPathElementName)?.Value;

                            if (string.IsNullOrWhiteSpace(outputFilePath))
                            {
                                throw InvalidConfigurationException.BecauseMissingOutput(pattern);
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

        private  Dictionary<string, XmlInlineConfigurationValuesProvider> ReadValuesGroups(XElement rootElement)
        {
            return rootElement.Elements()
                .Where(x => x.IsElementWithName(ValuesGroupElementName))
                .Select(x =>
                {
                    var groupName = x.Attribute("name")?.Value;
                    if (string.IsNullOrWhiteSpace(groupName))
                    {
                        throw InvalidConfigurationException.BecauseMissingGroupName();
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
                throw InvalidConfigurationException.BecauseDuplicatedPostTransformations();
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
                if (fileSystem.FileExists(fileFullPath) == false)
                {
                    throw InvalidConfigurationException.BecauseIncludedFileDoesNotExist(fileFullPath);
                }
                yield return new XmlFileConfigurationValuesProvider(fileFullPath, xmlReader);
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
                    throw InvalidConfigurationException.BecauseUnknowValuesGroup(valuesGroupName);
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