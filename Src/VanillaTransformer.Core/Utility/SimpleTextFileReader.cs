using System.IO;

namespace VanillaTransformer.Core.Utility
{
    public class SimpleTextFileReader : ITextFileReader
    {
        public Stream ReadFile(string path) => File.OpenRead(path);

        public bool FileExists(string path) => File.Exists(path);
    }
}