using System.IO;

namespace VanillaTransformer.OutputWriters
{
    public class FileTransformedOutputWriter : ITransformedOutputWriter
    {
        private readonly string filePath;

        public FileTransformedOutputWriter(string filePath)
        {
            this.filePath = filePath;
        }

        public void Save(string result)
        {
            File.WriteAllText(filePath, result);
        }
    }
}