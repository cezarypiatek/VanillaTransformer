using System.Collections.Generic;
using VanillaTransformer.Core.PostTransformations;

namespace VanillaTransformer.Core.Configuration.PostTransformations
{
    public class PostTransformationsConfigurationClearOperation : IPostTransformationsConfigurationOperation
    {
        public void Execute(List<IPostTransformation> postTransformations)
        {
            postTransformations.Clear();
        }
    }
}