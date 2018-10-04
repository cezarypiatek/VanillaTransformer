using NUnit.Framework;
using VanillaTransformer.Core.PostTransformations;
using VanillaTransformer.Core.PostTransformations.XML;

namespace VanillaTransformer.Tests.PostTransformations
{
    [TestFixture]
    public class StripXMLCommentTransformationTest
    {
        [Test]
        public void should_be_able_to_strip_comments()
        {
            //ARRANGE
            var transformer = new StripXMLCommentsTransformation();
            const string sampleXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><!--Sample comment--><body></body></root>";

            //ACT
            var result = transformer.Execute(sampleXML);

            //ASSERT
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><root><body></body></root>", result);
        } 
        
        [Test]
        public void should_be_able_to_strip_multiple_comments()
        {
            //ARRANGE
            var transformer = new StripXMLCommentsTransformation();
            const string sampleXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><!--Sample comment 1--><body></body><!--Sample comment 2--></root>";

            //ACT
            var result = transformer.Execute(sampleXML);

            //ASSERT
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><root><body></body></root>", result);
        }

        [Test]
        public void should_be_able_to_strip_comments_without_changing_formatting()
        {
            //ARRANGE
            var transformer = new StripXMLCommentsTransformation();
            const string sampleXML =
@"<?xml version=""1.0"" encoding=""utf-8""?>
   <root>
    <!--Sample comment-->
      <body>
        </body>
</root>";

            //ACT
            var result = transformer.Execute(sampleXML);

            //ASSERT
            const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-8""?>
   <root>
      <body>
        </body>
</root>";
            Assert.AreEqual(expectedResult, result);
        } 
        
        [Test]
        public void should_be_able_to_strip_multiline_comments()
        {
            //ARRANGE
            var transformer = new StripXMLCommentsTransformation();
            const string sampleXML =
@"<?xml version=""1.0"" encoding=""utf-8""?>
    <root>
    <!--Sample 
comment-->
        <body>
        </body>
</root>";

            //ACT
            var result = transformer.Execute(sampleXML);

            //ASSERT
            const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-8""?>
    <root>
        <body>
        </body>
</root>";
            Assert.AreEqual(expectedResult, result);
        }

    }
}