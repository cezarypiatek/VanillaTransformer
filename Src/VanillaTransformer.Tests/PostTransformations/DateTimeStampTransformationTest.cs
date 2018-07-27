using System.Text.RegularExpressions;
using NUnit.Framework;
using VanillaTransformer.PostTransformations.XML;

namespace VanillaTransformer.Tests.PostTransformations
{
    [TestFixture]
    public class DateTimeStampTransformationTest
    {
        [Test]
        public void should_be_able_to_add_datetime_stamp_to_transformed_text()
        {
            //ARRANGE
            var transformed = new DateTimeStampTransformation();
            const string sampleXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n    <body>Test</body>\r\n</root>";

            //ACT
            var result = transformed.Execute(sampleXml);
            

            //ASSERT
            Assert.IsTrue(Regex.IsMatch(result, @"<!-- Generated on: (.*?) -->$", RegexOptions.Singleline));
        }
    }
}