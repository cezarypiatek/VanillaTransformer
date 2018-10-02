
namespace VanillaTransformer.Core
{
    public class InputParameters
    {
        /// <summary>
        /// Path to a file with configuration pattern
        /// </summary>
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

        /// <summary>
        /// If not set, the path of the executable will be taken
        /// </summary>
        public string ProjectRootPath { get; set; }
    }
}
