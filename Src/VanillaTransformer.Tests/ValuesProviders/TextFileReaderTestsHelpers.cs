using System.IO;
using System.Text;
using Moq;
using VanillaTransformer.Core.Utility;

namespace VanillaTransformer.Tests.ValuesProviders
{
    public static class TextFileReaderTestsHelpers
    {
        public static ITextFileReader GetTextFileReaderMock(string path, string fileContent)
        {
            var fileReaderMockObj = new Mock<ITextFileReader>();
            var fileContentStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            fileReaderMockObj.Setup(fr => fr.ReadFile(path)).Returns(fileContentStream);
            return fileReaderMockObj.Object;
        }
    }
}