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
            var tokens = GetTokens(configurationPattern);
            var transformedConfiguration = ReplacePlaceholdersWithValues(configurationPattern, configurationValues);
            ValidateTransformedText(transformedConfiguration, tokens);
            return transformedConfiguration;
        }

        private IReadOnlyList<string> GetTokens(string text)
        {
            var placeholderPattern = new Regex(placeholderPatternString, RegexOptions.Multiline);
            var matches = placeholderPattern.Matches(text);
            return matches.OfType<Match>()
                .Select(match => match.Groups[1].Value)
                .Distinct()
                .ToList();
        }

        private string ReplacePlaceholdersWithValues(string configurationPattern, IDictionary<string, string> configurationValues)
        {
            var transformedConfiguration = configurationPattern;
            foreach (var value in configurationValues)
            {
                var valuePlaceholder = GetValuePlaceholder(value.Key);
                transformedConfiguration = transformedConfiguration.Replace(valuePlaceholder, value.Value);
            }
            return transformedConfiguration;
        }

        private string GetValuePlaceholder(string valueKey)
        {
            return string.Format(placeholderFormatString, valueKey);
        }

        private void ValidateTransformedText(string transformedText, IReadOnlyList<string> tokens)
        {
            var missingValuesNames = tokens.Where(x => transformedText.Contains(GetValuePlaceholder(x))).ToList();
            if (missingValuesNames.Count > 0)
            {
                throw new MissingValuesException(missingValuesNames);
            }
        }
    }
}