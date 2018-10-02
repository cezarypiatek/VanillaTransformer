using System.IO;

namespace VanillaTransformer.Core.Utility
{
    public class SimpleTextFileReader : ITextFileReader
    {
        public Stream ReadFile(string path)
        {
            return File.OpenRead(path);
        }
    }
}