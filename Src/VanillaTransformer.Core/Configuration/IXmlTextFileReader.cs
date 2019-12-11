using System.Xml.Linq;

namespace VanillaTransformer.Core.Configuration
{
    public interface IXmlTextFileReader
    {
        XElement Read(string path);
    }
}