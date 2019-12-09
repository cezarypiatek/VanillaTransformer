using System.IO;
using System.Text;
using Moq;
using VanillaTransformer.Core.Utility;

namespace VanillaTransformer.Tests.ValuesProviders
{
    public static class TextFileReaderTestsHelpers
    {
        public static ITextFileReader GetTextFileReaderMock(string filePath, string fileContent)
        {
            var fileReaderMockObj = new Mock<ITextFileReader>();
            var fileContentStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            fileReaderMockObj.Setup(fr => fr.ReadFile(filePath)).Returns(fileContentStream);
            fileReaderMockObj.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            return fileReaderMockObj.Object;
        }
    }
}