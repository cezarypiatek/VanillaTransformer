using System;
using System.Collections.Generic;
using System.Linq;
using VanillaTransformer.Core.PostTransformations.XML;

namespace VanillaTransformer.Core.PostTransformations
{
    public static class PostTransformationsPool
    {
        private static readonly Dictionary<string, IPostTransformation> Pool;

        static PostTransformationsPool()
        {
            Pool = new List<IPostTransformation>()
            {
                new ReFormatXMLTransformation(),
                new StripXMLCommentsTransformation(),
                new ReFormatJSONTransformation()
            }.ToDictionary(x => x.Name, x => x);
        }

        public static IPostTransformation Get(string transformationName)
        {
            if (Pool.TryGetValue(transformationName, out var postTransformation))
            {
                return postTransformation;
            }
            throw new ArgumentException($"Unknown post transformation: {transformationName}");
        }
    }
}