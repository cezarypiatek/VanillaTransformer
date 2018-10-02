using System.Collections.Generic;

namespace VanillaTransformer.Core.ValuesProviders
{
    public interface IValuesProvider
    {
        IDictionary<string, string> GetValues();
    }
}