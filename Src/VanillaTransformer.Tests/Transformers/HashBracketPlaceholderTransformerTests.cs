using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VanillaTransformer.Transformers;

namespace VanillaTransformer.Tests.Transformers
{
    [TestFixture]
    public class HashBracketPlaceholderTransformerTests
    {
        [Test]
        public void should_be_able_to_transform_pattern_single_occurrence_of_placeholder()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"}
            };
            const string pattern = @"<element1>#[Val1]</element1>";
            var tansformer = new HashBracketPlaceholderTransformer();

            //ACT
            var result = tansformer.Transform(pattern, values);

            //ASSERT
            Assert.IsNotNullOrEmpty(result);
            Assert.AreEqual("<element1>XX</element1>", result);
        }

        [Test]
        public void should_be_able_to_transform_pattern_multiple_occurrences_of_the_same_placeholder()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"}
            };
            const string pattern = @"<element attr=""#[Val1]"" >#[Val1]</element>";
            var tansformer = new HashBracketPlaceholderTransformer();

            //ACT
            var result = tansformer.Transform(pattern, values);

            //ASSERT
            Assert.IsNotNullOrEmpty(result);
            Assert.AreEqual(@"<element attr=""XX"" >XX</element>", result);
        }

        [Test]
        public void should_be_able_to_transform_pattern_using_many_values()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"},
                {"Val2","YY"}
            };
            const string pattern = @"<element attr=""#[Val1]"" >#[Val2]</element>";
            var tansformer = new HashBracketPlaceholderTransformer();

            //ACT
            var result = tansformer.Transform(pattern, values);

            //ASSERT
            Assert.IsNotNullOrEmpty(result);
            Assert.AreEqual(@"<element attr=""XX"" >YY</element>", result);
        }

        [Test]
        public void should_be_able_to_detect_missing_values_for_template()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"}
            };
            const string pattern = @"<element attr=""#[Val1]"" >#[Val2]</element>";
            var tansformer = new HashBracketPlaceholderTransformer();

            //ACT & ASSERT
            var exception = Assert.Throws<MissingValuesException>(() => tansformer.Transform(pattern, values));
            Assert.AreEqual(1, exception.MissingValuesNames.Count);
            Assert.AreEqual("Val2", exception.MissingValuesNames.First());
        }
    }
}
