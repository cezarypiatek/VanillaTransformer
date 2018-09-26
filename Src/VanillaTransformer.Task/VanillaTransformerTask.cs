using System;

namespace VanillaTransformer.Task
{
    /// <summary>
    /// MsBuild Task that allows to make a transformation on text files
    /// </summary>
    public class VanillaTransformerTask : Microsoft.Build.Utilities.Task
    {
        public string PatternFile { get; set; }

        /// <summary>
        /// Path to a output file
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Path to output archive
        /// </summary>
        public string OutputArchivePath { get; set; }

        /// <summary>
        /// Path to a file with values required by the pattern file
        /// </summary>
        public string ValuesSource { get; set; }

        /// <summary>
        /// Name of the value provider. If empty use default returned by <see cref="GetDefaultValuesProviderName"/>.
        /// </summary>
        public string ValuesProviderName { get; set; }

        /// <summary>
        /// String that define placeholders format. Every placeholder must contain KEY token. Example "${KEY}"
        /// </summary>
        public string PlaceholderPattern { get; set; }

        /// <summary>
        /// Path to a file with transform configuration
        /// </summary>
        public string TransformConfiguration { get; set; }

        public override bool Execute()
        {
            try
            {
                var inputParameters = new InputParameters
                {
                    OutputArchivePath = OutputArchivePath,
                    OutputPath = OutputPath,
                    PatternFile = PatternFile,
                    PlaceholderPattern = PlaceholderPattern,
                    TransformConfiguration = TransformConfiguration
                };

                IVanillaTransformer vanillaTransformer = new VanillaTransformer(inputParameters);
                vanillaTransformer.PrepareAndLaunchTransform();
                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"VanillaTransformer: {e.Message}");
                return false;
            }
        }


    
    }
}