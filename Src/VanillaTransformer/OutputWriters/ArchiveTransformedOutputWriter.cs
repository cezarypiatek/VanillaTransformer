using System.IO;
using System.IO.Compression;

namespace VanillaTransformer.OutputWriters
{
    public class ArchiveTransformedOutputWriter : ITransformedOutputWriter
    {
        private readonly string archivePath;
        private readonly string filePath;

        public ArchiveTransformedOutputWriter(string archivePath, string filePath)
        {
            this.archivePath = archivePath;
            this.filePath = filePath;
        }

        public void Save(string result)
        {
            var archiveFileInfo = new FileInfo(archivePath);
            archiveFileInfo.Directory?.Create();

            using (var archive = GetArchive(archivePath))
            {
                var archiveFile = GetNewEntry(archive);
                using (var s = archiveFile.Open())
                {
                    using (var w = new StreamWriter(s))
                    {
                        w.Write(result);
                    }
                }
            }
        }

        private ZipArchiveEntry GetNewEntry(ZipArchive archive)
        {
            var archiveFile = archive.GetEntry(filePath);
            archiveFile?.Delete();
            return archive.CreateEntry(filePath);
        }

        private static ZipArchive GetArchive(string archivePath)
        {
            return ZipFile.Open(archivePath, ZipArchiveMode.Update);
        }
    }
}
