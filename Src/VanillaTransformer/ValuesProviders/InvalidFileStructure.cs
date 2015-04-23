using System;

namespace VanillaTransformer.ValuesProviders
{
    public class InvalidFileStructure:ApplicationException
    {
        public InvalidFileStructure(string message) : base(message)
        {
        }
    }
}