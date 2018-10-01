using System.Collections.Generic;
using System.IO;
using VanillaTransformer.Configuration;
using VanillaTransformer.OutputWriters;
using VanillaTransformer.Transformers;
using VanillaTransformer.Utility;
using VanillaTransformer.ValuesProviders;

namespace VanillaTransformer
{
    public class VanillaTransformer : IVanillaTransformer
    {
        private readonly InputParameters _inputParameters;

        public VanillaTransformer(InputParameters inputParameters)
        {
            _inputParameters = inputParameters;
        }

        public void PrepareAndLaunchTransform()
        {
            foreach (var transformation in GetTransformations())
            {
                UpdatePathsWithRootPath(transformation);
                var configurationPattern = File.ReadAllText(transformation.PatternFilePath);
                var configurationTransformer = GetTransformer(transformation.PlaceholderPattern);
                var configurationValues = transformation.ValuesProvider.GetValues();
                var transformedConfiguration =
                    configurationTransformer.Transform(configurationPattern, configurationValues);
                var result = transformation.RunPostTransformations(transformedConfiguration);
                var outputWriter = GetOutputWriter(transformation);
                outputWriter.Save(result);
            }
        }

        private ITransformedOutputWriter GetOutputWriter(TransformConfiguration configuration)
        {
            if (configuration.ShouldOutputToArchive())
            {
                return new ArchiveTransformedOutputWriter(configuration.OutputArchive, configuration.OutputFilePath);
            }

            return new FileTransformedOutputWriter(configuration.OutputFilePath);
        }

        private List<TransformConfiguration> GetTransformations()
        {
            if (IsTransformConfigurationFileSpecified())
            {
                var configurationReader = new TransformConfigurationReader();
                return configurationReader.ReadFromFile(_inputParameters.TransformConfiguration, _inputParameters.ProjectRootPath);
            }

            return new List<TransformConfiguration>
            {
                new TransformConfiguration
                {
                    PatternFilePath = _inputParameters.PatternFile,
                    OutputFilePath = _inputParameters.OutputPath,
                    OutputArchive = _inputParameters.OutputArchivePath,
                    ValuesProvider = GetValuesProvider()
                }
            };
        }

        private bool IsTransformConfigurationFileSpecified()
        {
            return string.IsNullOrWhiteSpace(_inputParameters.TransformConfiguration) == false;
        }


        private IValuesProvider GetValuesProvider()
        {
            var providerName = string.IsNullOrWhiteSpace(_inputParameters.ValuesProviderName)
                ? GetDefaultValuesProviderName()
                : _inputParameters.ValuesProviderName;
            return ReflectionHelper.GetInstanceOf<IValuesProvider>(providerName, new[] { _inputParameters.ValuesSource });
        }

        private ITransformer GetTransformer(string transformationPlaceholderPattern)
        {
            return new GenericPlaceholderTransformer(transformationPlaceholderPattern ??
                                                     _inputParameters.PlaceholderPattern ?? "${KEY}");
        }

        private static string GetDefaultValuesProviderName()
        {
            return typeof(XmlFileConfigurationValuesProvider).Name;
        }

        private void UpdatePathsWithRootPath(TransformConfiguration transformConfiguration)
        {
            if (string.IsNullOrWhiteSpace(_inputParameters.ProjectRootPath) == false)
            {
                transformConfiguration.PatternFilePath =
                    Path.Combine(_inputParameters.ProjectRootPath, transformConfiguration.PatternFilePath);
                transformConfiguration.OutputFilePath =
                    Path.Combine(_inputParameters.ProjectRootPath, transformConfiguration.OutputFilePath);
            }
        }
    }

    public interface IVanillaTransformer
    {
        void PrepareAndLaunchTransform();
    }
}
