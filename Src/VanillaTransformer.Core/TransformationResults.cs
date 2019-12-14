using System;
using System.Collections.Generic;
using System.Linq;
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

        public void PrintDescription(Action<string> okPrinter, Action<string> errorPrinter)
        {
            foreach (var transformationResult in results)
            {
                var description = $"{transformationResult.Transformation.PatternFilePath} -> {transformationResult.Transformation.OutputDescription}";
                if (transformationResult.Exception == null)
                {
                    okPrinter($"{description}  [OK]");
                }
                else
                {
                    errorPrinter($"{description}  [ERROR]");
                    var exception = transformationResult.Exception;
                    while (exception != null)
                    {
                        errorPrinter($"\tCause by: [{exception.GetType().FullName}] {exception.Message}");
                        exception = exception.InnerException;
                    }
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