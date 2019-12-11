namespace VanillaTransformer.Core.Configuration
{
    public interface IFileSystem
    {
        bool FileExists(string path);
    }
}