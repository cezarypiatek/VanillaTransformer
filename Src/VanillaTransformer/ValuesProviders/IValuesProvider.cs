using System.Collections.Generic;

namespace VanillaTransformer.ValuesProviders
{
    public interface IValuesProvider
    {
        IDictionary<string, string> GetValues();
    }
}