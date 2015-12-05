using System;
using System.Collections.Generic;
using System.Xml.Linq;
using VanillaTransformer.PostTransformations;

namespace VanillaTransformer
{
    public interface IPostTransformationsConfigurationOperation
    {
        void Execute(List<IPostTransformation> postTransformations);
    }

    public class PostTransformationsConfigurationClearOperation : IPostTransformationsConfigurationOperation
    {
        public void Execute(List<IPostTransformation> postTransformations)
        {
            postTransformations.Clear();
        }
    }

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


    public static class PostTransformationsConfigurationOperationFactory
    {
        private const string PostTransformationNameAttribute = "name";

        public static IPostTransformationsConfigurationOperation Create(XElement xElement)
        {
            var operationName = xElement.Name.LocalName.ToLower();
            switch (operationName)
            {
                case "add":
                    var postTransformationName = xElement.Attribute(PostTransformationNameAttribute).Value;
                    return new PostTransformationsConfigurationAddOperation(postTransformationName);
                case "remove":
                    var transformationName = xElement.Attribute(PostTransformationNameAttribute).Value;
                    return new PostTransformationsConfigurationRemoveOperation(transformationName);
                case "clear":
                    return new PostTransformationsConfigurationClearOperation();
                default:
                    throw new ArgumentException("Unknown configuration operation: '{0}'", operationName);
            }
        }
    }
}