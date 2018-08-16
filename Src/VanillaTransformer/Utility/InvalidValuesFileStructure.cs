using System;

namespace VanillaTransformer.Utility
{
    public class InvalidValuesFileStructure: Exception
    {
        private InvalidValuesFileStructure(string filePath, string message) 
            : base($"Invalid value file '{filePath}': {message}")
        {
        }

        public static InvalidValuesFileStructure BecauseMissingRoot(string filePath)
        {
            return new InvalidValuesFileStructure(filePath, "There is no root element");
        }

        public static InvalidValuesFileStructure BecauseDuplicatedKey(string filePath, string duplicatedKey)
        {
            return new InvalidValuesFileStructure(filePath, $"Duplicated value for key '{duplicatedKey}'");
        }
    }
}