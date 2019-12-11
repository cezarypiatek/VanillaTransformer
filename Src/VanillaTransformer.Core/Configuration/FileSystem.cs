using System.IO;

namespace VanillaTransformer.Core.Configuration
{
    public class FileSystem : IFileSystem
    {
        public bool FileExists(string path) => File.Exists(path);
    }
}