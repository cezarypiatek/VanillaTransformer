using System.Collections.Generic;
using VanillaTransformer.PostTransformations;

namespace VanillaTransformer.Configuration.PostTransformations
{
    public interface IPostTransformationsConfigurationOperation
    {
        void Execute(List<IPostTransformation> postTransformations);
    }
}