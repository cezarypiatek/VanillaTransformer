using System.Collections.Generic;
using VanillaTransformer.Core.PostTransformations;

namespace VanillaTransformer.Core.Configuration.PostTransformations
{
    public interface IPostTransformationsConfigurationOperation
    {
        void Execute(List<IPostTransformation> postTransformations);
    }
}