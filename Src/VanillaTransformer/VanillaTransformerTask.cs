using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using VanillaTransformer.Transformers;
using VanillaTransformer.Utility;
using VanillaTransformer.ValuesProviders;

namespace VanillaTransformer
{
    /// <summary>
    /// MsBuild Task that allows to make a transformation on text files
    /// </summary>
    public class VanillaTransformerTask : Task
    {
        /// <summary>
        /// Path to a file with configuration pattern
        /// </summary>
        [Required]
        public string PatternFile { get; set; }

        /// <summary>
        /// Path to a output file
        /// </summary>
        [Required]
        public string OutputPath { get; set; }

        /// <summary>
        /// Path to a file with values required by the pattern file
        /// </summary>
        [Required]
        public string ValuesSource { get; set; }

        /// <summary>
        /// Name of the value provider. If empty use default returned by <see cref="GetDefaultValuesProviderName"/>.
        /// </summary>
        public string ValuesProviderName { get; set; }

        /// <summary>
        /// Name of the transformer. If empty use default returned by <see cref="GetTransformerName"/>. 
        /// </summary>
        public string TransformerName { get; set; }

        public override bool Execute()
        {
            try
            {
                var configurationPattern = File.ReadAllText(PatternFile);
                var valuesProvider = GetValuesProvider();
                var configurationTransformer = GetTransformer();
                var configurationValues = valuesProvider.GetValues(ValuesSource);
                var transformedConfiguration = configurationTransformer.Transform(configurationPattern, configurationValues);
                File.WriteAllText(OutputPath, transformedConfiguration);
                return true;
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                return false;
            }
        }


        private IValuesProvider GetValuesProvider()
        {
            var providerName = string.IsNullOrWhiteSpace(ValuesProviderName)
                ? GetDefaultValuesProviderName()
                : ValuesProviderName;
            return ReflectionHelper.GetInstanceOf<IValuesProvider>(providerName);
        }
        
        private ITransformer GetTransformer()
        {
            var transformerName = string.IsNullOrWhiteSpace(TransformerName)
                ? GetTransformerName()
                : TransformerName;
            return ReflectionHelper.GetInstanceOf<ITransformer>(transformerName);
        }

        private static string GetDefaultValuesProviderName()
        {
            return typeof(XmlConfigurationValuesProvider).Name;
        }

        private static string GetTransformerName()
        {
            return typeof (DollarPlaceholderTransformer).Name;
        }
    }
}