using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VanillaTransformer.Transformers
{
    public abstract class GenericPlaceholderTransformer : ITransformer
    {
        private readonly string placeholderFormatString;
        private readonly string placeholderPatternString;

        protected GenericPlaceholderTransformer(string placeholderFormatString, string placeholderPatternString)
        {
            this.placeholderFormatString = placeholderFormatString;
            this.placeholderPatternString = placeholderPatternString;
        }

        public string Transform(string configurationPattern, IDictionary<string, string> configurationValues)
        {
            var transformedConfiguration = ReplacePlaceholdersWithValues(configurationPattern, configurationValues);
            ValidateTransformedText(transformedConfiguration);
            return transformedConfiguration;
        }

        private string ReplacePlaceholdersWithValues(string configurationPattern, IDictionary<string, string> configurationValues)
        {
            var transformedConfiguration = configurationPattern;
            foreach (var value in configurationValues)
            {
                var valuePlaceholder = string.Format(placeholderFormatString, value.Key);
                transformedConfiguration = transformedConfiguration.Replace(valuePlaceholder, value.Value);
            }
            return transformedConfiguration;
        }

        private void ValidateTransformedText(string transformedText)
        {
            var placeholderPattern = new Regex(placeholderPatternString, RegexOptions.Multiline);
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