namespace VanillaTransformer.Core.PostTransformations
{
    public interface IPostTransformation
    {
        string Name { get; }
        string Execute(string configContent);
    }
}