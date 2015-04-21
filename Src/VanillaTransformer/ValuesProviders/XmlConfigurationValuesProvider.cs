using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace VanillaTransformer.ValuesProviders
{
    public class XmlConfigurationValuesProvider : IValuesProvider
    {
        public IDictionary<string, string> GetValues(string source)
        {
            var doc = XDocument.Load(source);
            if (doc.Root == null)
            {
                throw new ApplicationException("Invalid configuration file structure");
            }
            var result = doc.Root.Elements().ToDictionary(el => el.Name.LocalName, el => el.Value);
            return result;
        }
    }
}