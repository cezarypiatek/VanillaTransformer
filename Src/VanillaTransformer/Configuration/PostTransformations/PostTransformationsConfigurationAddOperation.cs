using System.Collections.Generic;
using VanillaTransformer.PostTransformations;

namespace VanillaTransformer.Configuration.PostTransformations
{
    public class PostTransformationsConfigurationAddOperation : IPostTransformationsConfigurationOperation
    {
        private readonly string postTransformationName;

        public PostTransformationsConfigurationAddOperation(string postTransformationName)
        {
            this.postTransformationName = postTransformationName;
        }

        public void Execute(List<IPostTransformation> postTransformations)
        {
            var postTransformation = PostTransformationsPool.Get(postTransformationName);
            postTransformations.Add(postTransformation);
        }
    }
}