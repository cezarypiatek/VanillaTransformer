using System;

namespace VanillaTransformer.Transformers
{
    [Obsolete("Use GenericPlaceholderTransformer", false)]
    public class DollarPlaceholderTransformer : GenericPlaceholderTransformer
    {
        public DollarPlaceholderTransformer() : base("${KEY}")
        {
        }
    }
}