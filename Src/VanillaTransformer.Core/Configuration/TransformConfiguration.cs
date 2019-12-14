using System;
using System.Collections.Generic;
using VanillaTransformer.Core.PostTransformations;
using VanillaTransformer.Core.ValuesProviders;

namespace VanillaTransformer.Core.Configuration
{
    public class TransformConfiguration
    {
        public string PatternFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public string OutputArchive { get; set; }

        public string PlaceholderPattern { get; set; }

        public string OutputDescription => ShouldOutputToArchive() ? $"{OutputArchive}!{OutputFilePath}" : OutputFilePath;

        public IValuesProvider ValuesProvider { get; set; }
        public List<IPostTransformation> PostTransformations { get; set; }

        public bool ShouldOutputToArchive()
        {
            return string.IsNullOrWhiteSpace(OutputArchive) == false;
        }

        public string RunPostTransformations(string transformedText)
        {
            var result = transformedText;
            if (PostTransformations != null)
            {
                foreach (var postTransformation in PostTransformations)
                {
                    try
                    {
                        result = postTransformation.Execute(result);
                    }
                    catch (Exception exception)
                    {
                        throw new PostTransformationException(postTransformation.Name, exception);
                    }
                }
            }
            return result;
        }
    }
}