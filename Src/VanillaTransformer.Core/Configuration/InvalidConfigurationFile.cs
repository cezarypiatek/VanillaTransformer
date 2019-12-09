using System;

namespace VanillaTransformer.Core.Configuration
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
        
        public static InvalidConfigurationFile BecauseMissingGroupName(string filePath)
        {
            return new InvalidConfigurationFile(filePath, "All 'valuesGroup' should have a non-empty 'Name' attribute");
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
        
        public static Exception BecauseIncludedFileDoesNotExist(string filePath, string expectedFilePath)
        {
            return new InvalidConfigurationFile(filePath, $"Included file '{expectedFilePath}' does not exist");
        }
        
        public static Exception BecauseUnknowValuesGroup(string filePath, string expectedGroupName)
        {
            return new InvalidConfigurationFile(filePath, $"Values group '{expectedGroupName}' is not defined");
        }
    }
}