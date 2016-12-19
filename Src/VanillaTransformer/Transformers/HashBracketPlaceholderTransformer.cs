using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VanillaTransformer.Transformers
{
    public class HashBracketPlaceholderTransformer : ITransformer
    {
        public string Transform(string configurationPattern, IDictionary<string, string> configurationValues)
        {
            var transformedConfiguration = ReplacePlaceholdersWithValues(configurationPattern, configurationValues);
            ValidateTransformedText(transformedConfiguration);
            return transformedConfiguration;
        }

        private static string ReplacePlaceholdersWithValues(string configurationPattern, IDictionary<string, string> configurationValues)
        {
            var transformedConfiguration = configurationPattern;
            foreach (var value in configurationValues)
            {
                var valuePlaceholder = string.Format("#[{0}]", value.Key);
                transformedConfiguration = transformedConfiguration.Replace(valuePlaceholder, value.Value);
            }
            return transformedConfiguration;
        }

        private static void ValidateTransformedText(string transformedText)
        {
            var placeholderPattern = new Regex(@"\#\[(.*?)\]", RegexOptions.Multiline);
            var matches = placeholderPattern.Matches(transformedText);
            if (matches.Count > 0)
            {
                var missingValuesNames = matches.OfType<Match>()
                    .Select(match => match.Groups[1].Value)
                    .Distinct()
                    .ToList();

                throw new MissingValuesException(missingValuesNames);
            }
        }
    }
}
