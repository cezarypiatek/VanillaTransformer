using System.Collections.Generic;

namespace VanillaTransformer
{
    public interface IValuesProvider
    {
        IDictionary<string, string> GetValues();
    }
}