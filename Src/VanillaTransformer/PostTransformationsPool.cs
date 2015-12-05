using System;
using System.Collections.Generic;
using System.Linq;
using VanillaTransformer.PostTransformations;

namespace VanillaTransformer
{
    public static class PostTransformationsPool
    {
        private static readonly Dictionary<string, IPostTransformation> Pool;

        static PostTransformationsPool()
        {
            Pool = new List<IPostTransformation>()
            {
                new ReFormatXMLTransformation(),
                new StripXMLCommentsTransformation()
            }.ToDictionary(x => x.Name, x => x);
        }

        public static IPostTransformation Get(string transformationName)
        {
            if (Pool.ContainsKey(transformationName))
            {
                return Pool[transformationName];
            }
            throw new ArgumentException("Unknown post transformation");
        }
    }
}