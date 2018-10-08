using System.Collections.Generic;
using VanillaTransformer.Core.PostTransformations;

namespace VanillaTransformer.Core.Configuration.PostTransformations
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