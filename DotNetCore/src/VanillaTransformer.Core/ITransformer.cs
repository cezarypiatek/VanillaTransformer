using System.Collections.Generic;

namespace VanillaTransformer.Core
{
    public interface ITransformer
    {
        string Transform(string configurationPattern, IDictionary<string,string> configurationValues);
    }
}