using System;

namespace VanillaTransformer.Core.Configuration
{
    public class PostTransformationException : Exception
    {
        public PostTransformationException(string postTransformationName, Exception innerException)
            : base($"An error occured while applying '{postTransformationName}' post-transformation.", innerException)
        {
        }
    }
}