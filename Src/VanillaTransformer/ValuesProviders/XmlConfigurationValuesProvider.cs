using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VanillaTransformer.Utility;

namespace VanillaTransformer.ValuesProviders
{
    public class XmlConfigurationValuesProvider : IValuesProvider
    {
        private ITextFileReader fileReader;

        public ITextFileReader FileReader
        {
            get
            {
                if (fileReader == null)
                {
                    fileReader = new SimpleTextFileReader();
                }
                return fileReader;
            }
            set { fileReader = value; }
        }

        public IDictionary<string, string> GetValues(string source)
        {
            using (var str = FileReader.ReadFile(source))
            {
                var doc = XDocument.Load(str);
                if (doc.Root == null)
                {
                    throw new ApplicationException("Invalid configuration file structure");
                }
                var result = doc.Root.Elements().ToDictionary(el => el.Name.LocalName, el => el.Value);
                return result;
            }
        }

        private class SimpleTextFileReader : ITextFileReader
        {
            public Stream ReadFile(string path)
            {
                return File.OpenRead(path);
            }
        }
    }
}