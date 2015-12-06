using NUnit.Framework;
using VanillaTransformer.PostTransformations;
using VanillaTransformer.PostTransformations.XML;

namespace VanillaTransformer.Tests.PostTransformations
{
    [TestFixture]
    public class ReFormatXMLTest
    {
        [Test]
        public void should_be_able_to_reformat_xml()
        {
            //ARRANGE
            var transformer = new ReFormatXMLTransformation();
            const string sampleXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><body>Test</body></root>";

            //ACT
            var result = transformer.Execute(sampleXML);

            //ASSERT
            const string expectedResult = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n    <body>Test</body>\r\n</root>";
            Assert.AreEqual(expectedResult, result);
        }
    }
}