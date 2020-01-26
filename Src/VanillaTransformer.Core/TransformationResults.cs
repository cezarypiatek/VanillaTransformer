using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VanillaTransformer.Core.Configuration;

namespace VanillaTransformer.Core
{
    public class TransformationResults
    {
        class TransformationResult
        {
            public TransformConfiguration Transformation { get; }
            public Exception Exception { get; }

            public TransformationResult(TransformConfiguration transformation, Exception exception = null)
            {
                Transformation = transformation;
                Exception = exception;
            }
        }
        private readonly List<TransformationResult> results = new List<TransformationResult>();

        public void PrintDescription(Action<string> okPrinter, Action<string> errorPrinter, string rootPath)
        {
            string ToRelativePath(string path)
            {
                var fullPath = Path.GetFullPath(path);
                return string.IsNullOrWhiteSpace(rootPath) ? fullPath : fullPath.Replace(rootPath, "");
            }

            foreach (var transformationResult in results)
            {
                var transformation = transformationResult.Transformation;

                var outputDescription = transformation.ShouldOutputToArchive() ? 
                    $"{ToRelativePath(transformation.OutputArchive)}!{transformation.OutputFilePath}" 
                    : ToRelativePath(transformation.OutputFilePath);

                var description = $"{ToRelativePath(transformation.PatternFilePath)} -> {outputDescription}";
                if (transformationResult.Exception == null)
                {
                    okPrinter($"{description}  [OK]");
                }
                else
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine($"{description}  [ERROR]");
                    var exception = transformationResult.Exception;
                    while (exception != null)
                    {
                        messageBuilder.AppendLine($"\tCause by: [{exception.GetType().FullName}] {exception.Message}");
                        exception = exception.InnerException;
                    }

                    errorPrinter(messageBuilder.ToString().Trim());
                }
            }
        }

        public bool Success => results.Any(x => x.Exception != null) == false;

        public void AddSuccess(TransformConfiguration processedConfiguration) 
            => results.Add(new TransformationResult(processedConfiguration));
        public  void AddFail(TransformConfiguration processedConfiguration, Exception exception) 
            => results.Add(new TransformationResult(processedConfiguration, exception));
    }
}