using System;
using System.Collections.Generic;

namespace VanillaTransformer.Transformers
{
    public class MissingValuesException:ApplicationException
    {
        public List<string> MissingValuesNames { get; private set; }

        public override string Message
        {
            get { return string.Format("Cannot find the following values: {0}", string.Join(",", MissingValuesNames)); }
        }

        public MissingValuesException(List<string> missingValuesNames)
        {
            MissingValuesNames = missingValuesNames;
        }
    }
}