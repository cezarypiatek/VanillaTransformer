using NUnit.Framework;
using VanillaTransformer.Core.PostTransformations;

namespace VanillaTransformer.Tests.PostTransformations
{
    [TestFixture]
    public class ReFormatJSONTest
    {
        [Test]
        public void should_be_able_to_reformat_json()
        {
            //ARRANGE
            var transformer = new ReFormatJSONTransformation();
            const string sampleJson = "{\"someproperty1\":\"value1\", \"someproperty2\":\"value2\"}";

            //ACT
            var result = transformer.Execute(sampleJson);

            //ASSERT
            const string expectedResult = "{\r\n  \"someproperty1\": \"value1\",\r\n  \"someproperty2\": \"value2\"\r\n}";
            Assert.AreEqual(expectedResult, result);
        }
        
        [Test]
        public void should_strip_forbidden_comments()
        {
            //ARRANGE
            var transformer = new ReFormatJSONTransformation();
            const string sampleJson = "{/*Invalid comment*/ \"someproperty1\":\"value1\", \"someproperty2\":\"value2\"}";

            //ACT
            var result = transformer.Execute(sampleJson);

            //ASSERT
            const string expectedResult = "{\r\n  \"someproperty1\": \"value1\",\r\n  \"someproperty2\": \"value2\"\r\n}";
            Assert.AreEqual(expectedResult, result);
        }
        
        [Test]
        public void should_break_trying_to_reformat_invalid_json()
        {
            //ARRANGE
            var transformer = new ReFormatJSONTransformation();
            const string sampleJson = "{\"someproperty1\":\"va\\lue1\", \"someproperty2\":\"value2\"}";

            //ACT
            Assert.Catch(()=> transformer.Execute(sampleJson));
        }
    }
}