using System;

namespace VanillaTransformer.Core.Transformers
{
    [Obsolete("Use GenericPlaceholderTransformer", false)]
    public class DollarPlaceholderTransformer : GenericPlaceholderTransformer
    {
        public DollarPlaceholderTransformer() : base("${KEY}")
        {
        }
    }
}