using System;

namespace VanillaTransformer.Core.Configuration
{
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(string message) 
            : base(message)
        {
        }

        public static InvalidConfigurationException BecauseMissingValuesSource(string patternFile)
        {
            return new InvalidConfigurationException($"One of the transformations for'{patternFile}' has missing values definition");
        }
        public static InvalidConfigurationException BecauseMissingPattern()
        {
            return new InvalidConfigurationException("One of the transformations has missing pattern definition");
        }
        
        public static InvalidConfigurationException BecauseMissingGroupName()
        {
            return new InvalidConfigurationException("All 'valuesGroup' should have a non-empty 'Name' attribute");
        }

        public static InvalidConfigurationException BecauseMissingRoot()
        {
            return new InvalidConfigurationException("There is no root element");
        }

        public static Exception BecauseMissingOutput(string patternFile)
        {
            return new InvalidConfigurationException($"One of the transformations for'{patternFile}' has missing output definition");
        }

        public static Exception BecauseDuplicatedPostTransformations()
        {
            return new InvalidConfigurationException("Only one 'PostTransformations' element is allowed on given level");
        }
        
        public static Exception BecauseIncludedFileDoesNotExist(string expectedFilePath)
        {
            return new InvalidConfigurationException($"Included file '{expectedFilePath}' does not exist");
        }
        
        public static Exception BecauseUnknowValuesGroup(string expectedGroupName)
        {
            return new InvalidConfigurationException($"Values group '{expectedGroupName}' is not defined");
        }
    }
}