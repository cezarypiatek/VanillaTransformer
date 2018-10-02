using System.Text.RegularExpressions;

namespace VanillaTransformer.Core.PostTransformations.XML
{
    public class StripXMLCommentsTransformation : IPostTransformation
    {
        public string Name
        {
            get { return "StripXMLComments"; }
        }

        private static readonly Regex CommentPattern = new Regex(@"((?<=\r\n|\n)([ \t])*)?<!--(.*?)-->(\r\n|\n)?", RegexOptions.Singleline);

        public string Execute(string configContent)
        {
            return CommentPattern.Replace(configContent, "");
        }
    }
}