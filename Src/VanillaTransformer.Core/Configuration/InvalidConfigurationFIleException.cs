using System;

namespace VanillaTransformer.Core.Configuration
{
    public class InvalidConfigurationFIleException:Exception
    {
        public InvalidConfigurationFIleException(string fileName, Exception innerException)
            :base($"Invalid configuration file: {fileName}", innerException)
        {
        }
    }
}