using System;

namespace VanillaTransformer.PostTransformations.XML
{
    public class DateTimeStampTransformation : IPostTransformation
    {
        public string Name
        {
            get { return "DateTimeStamp"; }
        }

        public string Execute(string configContent)
        {
            return string.Format("{1}\r\n<!-- Generated on: {0} -->", configContent, DateTime.Now);
        }
    }
}