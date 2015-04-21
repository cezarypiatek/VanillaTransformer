using System.Collections.Generic;

namespace VanillaTransformer.Transformers
{
    public class DollarPlaceholderTransformer : ITransformer
    {
        public string Transform(string configurationPattern, IDictionary<string,string> configurationValues)
        {
            var transformedConfiguration = configurationPattern;
            foreach (var value in configurationValues)
            {
                var valuePlaceholder = string.Format("${{{0}}}", value.Key);
                transformedConfiguration = transformedConfiguration.Replace(valuePlaceholder, value.Value);
            }
            return transformedConfiguration;
        }
    }
}