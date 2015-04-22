using System.IO;

namespace VanillaTransformer.Utility
{
    public interface ITextFileReader
    {
        Stream ReadFile(string path);
    }
}