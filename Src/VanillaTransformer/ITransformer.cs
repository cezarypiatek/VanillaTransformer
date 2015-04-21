using System.Collections.Generic;

namespace VanillaTransformer
{
    public interface ITransformer
    {
        string Transform(string configurationPattern, IDictionary<string,string> configurationValues);
    }
}