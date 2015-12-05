namespace VanillaTransformer.PostTransformations
{
    public interface IPostTransformation
    {
        string Name { get; }
        string Execute(string configContent);
    }
}