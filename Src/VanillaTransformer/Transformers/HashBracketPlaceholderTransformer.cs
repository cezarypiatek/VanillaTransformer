using System;

namespace VanillaTransformer.Core.Transformers
{
    [Obsolete("Use GenericPlaceholderTransformer", false)]
    public class HashBracketPlaceholderTransformer : GenericPlaceholderTransformer
    {
        public HashBracketPlaceholderTransformer() : base("#[KEY]")
        {
        }
    }
}
