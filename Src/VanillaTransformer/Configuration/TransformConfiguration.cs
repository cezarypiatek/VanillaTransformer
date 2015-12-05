using System.Collections.Generic;
using VanillaTransformer.PostTransformations;
using VanillaTransformer.ValuesProviders;

namespace VanillaTransformer.Configuration
{
    public class TransformConfiguration
    {
        public string PatternFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public IValuesProvider ValuesProvider { get; set; }
        public List<IPostTransformation> PostTransformations { get; set; }

        public string RunPostTransformations(string transformedText)
        {
            var result = transformedText;
            if (PostTransformations != null)
            {
                foreach (var postTransformation in PostTransformations)
                {
                    result = postTransformation.Execute(result);
                }
            }
            return result;
        }
    }
}