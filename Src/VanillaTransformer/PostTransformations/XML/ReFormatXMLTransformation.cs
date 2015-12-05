using System;
using System.IO;
using System.Xml;

namespace VanillaTransformer.PostTransformations.XML
{
    public class ReFormatXMLTransformation:IPostTransformation
    {
        public string Name { get { return "ReFormatXML"; } }
        public string Execute(string configContent)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    ",
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace,
            };
            var xDocument = new XmlDocument();
            xDocument.LoadXml(configContent);
            using (var textWriter = new StringWriter())
            {
                using (var writer = XmlWriter.Create(textWriter,settings))
                {
                    xDocument.WriteTo(writer);
                }
                return textWriter.ToString();
            }
        }
    }
}
