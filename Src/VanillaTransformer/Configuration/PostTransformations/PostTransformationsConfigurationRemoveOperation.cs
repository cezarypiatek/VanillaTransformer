using System.Collections.Generic;
using VanillaTransformer.PostTransformations;

namespace VanillaTransformer.Configuration.PostTransformations
{
    public class PostTransformationsConfigurationRemoveOperation : IPostTransformationsConfigurationOperation
    {
        private readonly string postTransformationName;

        public PostTransformationsConfigurationRemoveOperation(string postTransformationName)
        {
            this.postTransformationName = postTransformationName;
        }

        public void Execute(List<IPostTransformation> postTransformations)
        {
            var postTransformation = PostTransformationsPool.Get(postTransformationName);
            postTransformations.Remove(postTransformation);
        }
    }
}