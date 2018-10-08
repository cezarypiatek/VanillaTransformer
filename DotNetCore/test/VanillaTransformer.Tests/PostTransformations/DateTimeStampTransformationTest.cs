using System.Text.RegularExpressions;
using VanillaTransformer.Core.PostTransformations.XML;
using Xunit;

namespace VanillaTransformer.Tests.PostTransformations
{
    public class DateTimeStampTransformationTest
    {
        [Fact]
        public void should_be_able_to_add_datetime_stamp_to_transformed_text()
        {
            //ARRANGE
            var transformed = new DateTimeStampTransformation();
            const string sampleXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n    <body>Test</body>\r\n</root>";

            //ACT
            var result = transformed.Execute(sampleXml);
            

            //ASSERT
            Assert.True(Regex.IsMatch(result, @"<!-- Generated on: (.*?) -->$", RegexOptions.Singleline));
        }
    }
}