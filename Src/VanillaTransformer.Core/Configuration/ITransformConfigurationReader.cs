using System.Collections.Generic;

namespace VanillaTransformer.Core.Configuration
{
    public interface ITransformConfigurationReader
    {
        List<TransformConfiguration> ReadConfig(string path);
    }
}