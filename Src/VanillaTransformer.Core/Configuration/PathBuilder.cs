using System;
using System.IO;

namespace VanillaTransformer.Core.Configuration
{
    public class PathBuilder
    {
        private readonly string rootPath;
        private readonly string outputPathPattern;
        private readonly string archivePathPattern;

        public PathBuilder(string rootPath, string outputPathPattern, string archivePathPattern)
        {
            this.rootPath = rootPath;
            this.outputPathPattern = outputPathPattern;
            this.archivePathPattern = archivePathPattern;
        }

        public (string filePath, string archivePath) CreateOutputPaths(string appName, string envName,
            string machineName, string templateName)
        {
            string FormatPath(string pathPattern)
            {
                return pathPattern.Replace("{app}", appName)
                    .Replace("{environment}", envName)
                    .Replace("{machine}", machineName)
                    .Replace("{template}", templateName);
            }

            var filePath = FormatPath(outputPathPattern);
            
            if(String.IsNullOrWhiteSpace(archivePathPattern))
            {

                return (Path.Combine(rootPath, filePath), null);
            }

            var archivePath = FormatPath(archivePathPattern);
            return (filePath, Path.Combine(rootPath, archivePath));
        }
    }
}