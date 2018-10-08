using VanillaTransformer.Core.PostTransformations.XML;
using Xunit;

namespace VanillaTransformer.Tests.PostTransformations
{
    public class ReFormatXMLTest
    {
        [Fact]
        public void should_be_able_to_reformat_xml()
        {
            //ARRANGE
            var transformer = new ReFormatXMLTransformation();
            const string sampleXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><body>Test</body></root>";

            //ACT
            var result = transformer.Execute(sampleXML);

            //ASSERT
            const string expectedResult = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n    <body>Test</body>\r\n</root>";
            Assert.Equal(expectedResult, result);
        }
    }
}