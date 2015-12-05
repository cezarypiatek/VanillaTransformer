using System;
using System.Xml.Linq;

namespace VanillaTransformer.Configuration.PostTransformations
{
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