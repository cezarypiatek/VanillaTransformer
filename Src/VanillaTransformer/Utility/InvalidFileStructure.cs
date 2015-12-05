using System;

namespace VanillaTransformer.Utility
{
    public class InvalidFileStructure:ApplicationException
    {
        public InvalidFileStructure(string message) : base(message)
        {
        }
    }
}