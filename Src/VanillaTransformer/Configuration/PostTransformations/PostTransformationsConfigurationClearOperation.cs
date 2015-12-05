using System.Collections.Generic;
using VanillaTransformer.PostTransformations;

namespace VanillaTransformer.Configuration.PostTransformations
{
    public class PostTransformationsConfigurationClearOperation : IPostTransformationsConfigurationOperation
    {
        public void Execute(List<IPostTransformation> postTransformations)
        {
            postTransformations.Clear();
        }
    }
}