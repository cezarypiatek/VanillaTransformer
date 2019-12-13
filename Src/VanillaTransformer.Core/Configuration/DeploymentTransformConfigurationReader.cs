using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VanillaTransformer.Core.PostTransformations;
using VanillaTransformer.Core.Utility;
using VanillaTransformer.Core.ValuesProviders;

namespace VanillaTransformer.Core.Configuration
{
    public class DeploymentTransformConfigurationReader : ITransformConfigurationReader
    {
        private readonly IXmlTextFileReader xmlTextFileReader;

        public DeploymentTransformConfigurationReader(IXmlTextFileReader xmlTextFileReader)
        {
            this.xmlTextFileReader = xmlTextFileReader;
        }

        public List<TransformConfiguration> ReadConfig(string path)
        {
            var rootPath = Path.GetDirectoryName(Path.GetFullPath(path));
            var doc = xmlTextFileReader.Read(path);
            var appNodes = doc.GetChildren("apps","app", isRequired:true);
            var environmentsNodes = doc.GetChildren("environments","environment", isRequired:true);
            var transformationsNodes = doc.GetChildren("transformations","transformation", isRequired:true);
            var postTransformations = doc.GetChildren("postTransformations","postTransformation", isRequired:false);
            return GetTransformations(appNodes, environmentsNodes, transformationsNodes, postTransformations, rootPath).ToList();
        }

        private IEnumerable<TransformConfiguration> GetTransformations(IReadOnlyList<XElement> appNodes,
            IReadOnlyList<XElement> environmentsNodes, IReadOnlyList<XElement> transformationsNodes,
            IReadOnlyList<XElement> postTransformations, string rootPath)
        {
            foreach (var transformationsNode in transformationsNodes)
            {
                var outputPathPattern = transformationsNode.GetRequiredAttribute("output");
                foreach (var appNode in appNodes)
                {
                    var appName = appNode.GetRequiredAttribute("name");
                    var templateNodesContainer = appNode.GetRequiredElement("templates");

                    foreach (var templateNode in templateNodesContainer.Elements("template"))
                    {
                        var templateName = templateNode.GetRequiredAttribute("name");
                        var patternFilePath = templateNode.GetRequiredAttribute("pattern");
                        var placeholder = templateNode.Attribute("placeholder")?.Value;

                        foreach (var environmentsNode in environmentsNodes)
                        {
                            var envName = environmentsNode.GetRequiredAttribute("name");
                            var envValuesProvider = GetValueProvider(environmentsNode);
                            var machineNodeContainer = environmentsNode.Element("machines");

                            if (machineNodeContainer == null)
                            {
                                var outputFilePath = outputPathPattern.Replace("{app}", appName)
                                    .Replace("{environment}", envName)
                                    .Replace("{machine}", string.Empty)
                                    .Replace("{template}", templateName);

                                yield return new TransformConfiguration()
                                {
                                    OutputFilePath = Path.Combine(rootPath, outputFilePath),
                                    PatternFilePath = Path.Combine(rootPath, patternFilePath),
                                    PlaceholderPattern = placeholder,
                                    ValuesProvider = envValuesProvider,
                                    PostTransformations = CreatePostTransformations(postTransformations, templateName),
                                };
                            }
                            else foreach (var machineNode in machineNodeContainer.Elements("machine"))
                            {
                                var machineName = machineNode.GetRequiredAttribute("name");
                                var machineValuesProvider = GetValueProvider(machineNode);
                                if (ShouldDeployTo(appName, machineNode) == false)
                                {
                                    continue;
                                }
                                var outputFilePath = outputPathPattern.Replace("{app}", appName)
                                    .Replace("{environment}", envName)
                                    .Replace("{machine}", machineName)
                                    .Replace("{template}", templateName);

                                
                                yield return new TransformConfiguration()
                                {
                                    OutputFilePath = Path.Combine(rootPath, outputFilePath),
                                    PatternFilePath = Path.Combine(rootPath, patternFilePath),
                                    PlaceholderPattern = placeholder,
                                    ValuesProvider = new CompositeValuesProvider(new[]{machineValuesProvider, envValuesProvider}),
                                    PostTransformations = CreatePostTransformations(postTransformations, templateName),
                                };
                            }
                        }
                    }
                }
            }
        }

        private static bool ShouldDeployTo(string appName, XElement machineNode)
        {
            var availableForApps = machineNode.Attribute("apps")?.Value.Split(';');
            if (availableForApps == null || availableForApps.Length == 0)
            {
                return true;
            }

            return availableForApps.Any(x => x == appName);
        }

        private static List<IPostTransformation> CreatePostTransformations(IReadOnlyList<XElement> postTransformations, string templateName)
        {
            return postTransformations.Where(x =>
                {
                    var expectedExtension = x.Attribute("fileExtension")?.Value;
                    if (string.IsNullOrWhiteSpace(expectedExtension))
                    {
                        return true;
                    }
                    return templateName.EndsWith($".{expectedExtension}");
                }).SelectMany(x => x.Elements("add"))
                .Select(x => x.Attribute("name")?.Value)
                .Where(name => string.IsNullOrWhiteSpace(name) == false).ToList()
                .Select(PostTransformationsPool.Get).ToList();
        }

        private IValuesProvider GetValueProvider(XElement parentNode)
        {
            var valuesContainer = parentNode.Element("values");
            return new XmlInlineConfigurationValuesProvider(valuesContainer);
        }
    }
}