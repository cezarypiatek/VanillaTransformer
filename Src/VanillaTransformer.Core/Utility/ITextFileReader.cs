using System.IO;

namespace VanillaTransformer.Core.Utility
{
    public interface ITextFileReader
    {
        Stream ReadFile(string path);
        bool FileExists(string path);
    }
}