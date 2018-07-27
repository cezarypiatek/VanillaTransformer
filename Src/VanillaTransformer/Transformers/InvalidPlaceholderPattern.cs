using System;

namespace VanillaTransformer.Transformers
{
    public class InvalidPlaceholderPattern:Exception
    {
        public InvalidPlaceholderPattern(string invalidPlaceholderPattern)
            :base($"Invalid placeholder pattern '{invalidPlaceholderPattern}'. Missing KEY token")
        {
        }
    }
}