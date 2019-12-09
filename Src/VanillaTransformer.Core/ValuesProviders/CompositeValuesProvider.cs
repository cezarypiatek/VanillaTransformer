using System.Collections.Generic;

namespace VanillaTransformer.Core.ValuesProviders
{
    public class CompositeValuesProvider : IValuesProvider
    {
        private readonly IReadOnlyList<IValuesProvider> valuesProviders;

        public CompositeValuesProvider(IReadOnlyList<IValuesProvider> valuesProviders)
        {
            this.valuesProviders = valuesProviders;
        }

        public IDictionary<string, string> GetValues()
        {
            var result = new Dictionary<string, string>();

            foreach (var valuesProvider in valuesProviders)
            {
                foreach (var valuePair in valuesProvider.GetValues())
                {
                    if (result.ContainsKey(valuePair.Key) == false)
                    {
                        result.Add(valuePair.Key, valuePair.Value);
                    }
                }
            }

            return result;
        }
    }
}