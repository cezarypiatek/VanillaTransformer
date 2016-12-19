namespace VanillaTransformer.Transformers
{
    public class HashBracketPlaceholderTransformer : GenericPlaceholderTransformer
    {
        public HashBracketPlaceholderTransformer() : base("#[{0}]", @"\#\[(.*?)\]")
        {
        }
    }
}
