using System.Linq;
using VanillaTransformer.Core.ValuesProviders;
using Xunit;

namespace VanillaTransformer.Tests.ValuesProviders
{
    public class XmlValuesProviderTests
    {
        [Fact]
        public void should_be_able_to_read_properly_xml_file_structure()
        {
            //ARRANGE
            const string testFileName = "test.xml";
            var xmlValueProvider = new XmlFileConfigurationValuesProvider(testFileName);
            xmlValueProvider.FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFileName, @"<root><Var1>Val1</Var1></root>");

            //ACT
            var result = xmlValueProvider.GetValues();

            //ASSERT
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            var firstPair = result.First();
            Assert.Equal("Var1", firstPair.Key);
        }

        [Fact]
        public void should_be_able_to_read_simple_value_from_xml_file()
        {
            //ARRANGE
            const string testFileName = "test.xml";
            var xmlValueProvider = new XmlFileConfigurationValuesProvider(testFileName);
            xmlValueProvider.FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFileName, @"<root><Var1>Val1</Var1></root>");
            
            //ACT
            var result = xmlValueProvider.GetValues();

            //ASSERT
            var firstPair = result.First();
            Assert.Equal("Val1", firstPair.Value);
        }

        [Fact]
        public void should_be_able_read_value_with_xml_content()
        {
             //ARRANGE
            const string testFileName = "test.xml";
            var xmlValueProvider = new XmlFileConfigurationValuesProvider(testFileName);
            xmlValueProvider.FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFileName, @"<root><Var1><InsideXml>Sample</InsideXml></Var1></root>");
            
            //ACT
            var result = xmlValueProvider.GetValues();

            //ASSERT
            var firstPair = result.First();
            Assert.Equal("<InsideXml>Sample</InsideXml>", firstPair.Value);

        }


        [Fact]
        public void should_be_able_read_value_with_xml_content_and_preserve_whitespaces()
        {
             //ARRANGE
            const string testFileName = "test.xml";
            var xmlValueProvider = new XmlFileConfigurationValuesProvider(testFileName);
            xmlValueProvider.FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFileName,
@"<root>
    <Var1>
        <InsideXml>Sample</InsideXml>
        <InsideXml>Sample2</InsideXml>
    </Var1>
</root>");
            
            //ACT
            var result = xmlValueProvider.GetValues();

            //ASSERT
            var firstPair = result.First();
            Assert.Equal(
@"
        <InsideXml>Sample</InsideXml>
        <InsideXml>Sample2</InsideXml>
    ", firstPair.Value);

        }

        [Fact]
        public void should_be_able_read_value_with_xml_attribute()
        {
            //ARRANGE
            const string testFileName = "test.xml";
            var xmlValueProvider = new XmlFileConfigurationValuesProvider(testFileName);
            xmlValueProvider.FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFileName, @"<root><Var1>attribute=""val""</Var1></root>");

            //ACT
            var result = xmlValueProvider.GetValues();

            //ASSERT
            var firstPair = result.First();
            Assert.Equal(@"attribute=""val""", firstPair.Value);
        }
    }
}
