using Moq;
using VanillaTransformer.Core.Configuration;
using VanillaTransformer.Tests.ValuesProviders;

namespace VanillaTransformer.Tests.CoreTest
{
    public static class TransformConfgReaderTestFactory
    {
        public static VanillaTransformConfigurationReader CreateConfigReader(string path, string config)
        {
            var xmlTextFileReader = CreateXmlTextFileReader(path, config);
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            return  new VanillaTransformConfigurationReader(fileSystemMock.Object,  xmlTextFileReader);
        }

        public static XmlTextFileReader CreateXmlTextFileReader(string path, string config)
        {
            var textFileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(path, config);
            var xmlTextFileReader = new XmlTextFileReader(textFileReader);
            return xmlTextFileReader;
        }
    }
}