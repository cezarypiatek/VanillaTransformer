namespace VanillaTransformer.Transformers
{
    public class DollarPlaceholderTransformer : GenericPlaceholderTransformer
    {
        public DollarPlaceholderTransformer() : base("${{{0}}}", @"\$\{(.*?)\}")
        {
        }
    }
}