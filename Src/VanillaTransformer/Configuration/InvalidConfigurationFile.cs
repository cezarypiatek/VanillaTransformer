using System;

namespace VanillaTransformer.Configuration
{
    public class InvalidConfigurationFile : Exception
    {
        private InvalidConfigurationFile(string filePath, string message) 
            : base($"Invalid configuration file '{filePath}': {message}")
        {
        }

        public static InvalidConfigurationFile BecauseMissingValuesSource(string filePath, string patternFile)
        {
            return new InvalidConfigurationFile(filePath, $"One of the transformations for'{patternFile}' has missing values definition");
        }
        public static InvalidConfigurationFile BecauseMissingPattern(string filePath)
        {
            return new InvalidConfigurationFile(filePath, "One of the transformations has missing pattern definition");
        }

        public static InvalidConfigurationFile BecauseMissingRoot(string filePath)
        {
            return new InvalidConfigurationFile(filePath, "There is no root element");
        }

        public static Exception BecauseMissingOutput(string filePath, string patternFile)
        {
            return new InvalidConfigurationFile(filePath, $"One of the transformations for'{patternFile}' has missing output definition");
        }

        public static Exception BecauseDuplicatedPostTransformations(string filePath)
        {
            return new InvalidConfigurationFile(filePath, "Only one 'PostTransformations' element is allowed on given level");
        }
    }
}