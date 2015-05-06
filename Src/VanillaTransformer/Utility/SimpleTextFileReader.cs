using System.IO;

namespace VanillaTransformer.Utility
{
    public class SimpleTextFileReader : ITextFileReader
    {
        public Stream ReadFile(string path)
        {
            return File.OpenRead(path);
        }
    }
}