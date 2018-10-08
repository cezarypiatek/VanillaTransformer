using System.Collections.Generic;
using System.Linq;
using VanillaTransformer.Core.Transformers;
using Xunit;

namespace VanillaTransformer.Tests.Transformers
{
    public class DollarPlaceholderTransformerTests
    {
        [Fact]
        public void should_be_able_to_transform_pattern_single_occurrence_of_placeholder()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"}
            };
            const string pattern = @"<element1>${Val1}</element1>";
            var transformer = new DollarPlaceholderTransformer();

            //ACT
            var result = transformer.Transform(pattern, values);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("<element1>XX</element1>", result);
        }

        [Fact]
        public void should_be_able_to_transform_pattern_single_occurrence_of_placeholder_with_generic_transformer()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"}
            };
            const string pattern = @"<element1>${{Val1}}</element1>";
            var transformer = new GenericPlaceholderTransformer("${{KEY}}");

            //ACT
            var result = transformer.Transform(pattern, values);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("<element1>XX</element1>", result);
        }

        [Fact]
        public void should_be_able_to_transform_pattern_multiple_occurrences_of_the_same_placeholder()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"}
            };
            const string pattern = @"<element attr=""${Val1}"" >${Val1}</element>";
            var transformer = new DollarPlaceholderTransformer();

            //ACT
            var result = transformer.Transform(pattern, values);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(@"<element attr=""XX"" >XX</element>", result);
        }

        [Fact]
        public void should_be_able_to_transform_pattern_using_many_values()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"},
                {"Val2","YY"}
            };
            const string pattern = @"<element attr=""${Val1}"" >${Val2}</element>";
            var transformer = new DollarPlaceholderTransformer();

            //ACT
            var result = transformer.Transform(pattern, values);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(@"<element attr=""XX"" >YY</element>", result);
        }

        [Fact]
        public void should_be_able_to_detect_missing_values_for_template()
        {
            //ARRANGE
            var values = new Dictionary<string, string>
            {
                {"Val1","XX"},
                {"Val3","${NEW_Val3}"}
            };
            const string pattern = @"<element attr=""${Val1}"" >${Val2}${Val3}</element>";
            var transformer = new DollarPlaceholderTransformer();

            //ACT & ASSERT
            var exception = Assert.Throws<MissingValuesException>(() => transformer.Transform(pattern, values));
            Assert.Single(exception.MissingValuesNames);
            Assert.Equal("Val2", exception.MissingValuesNames.First());
        }
    }
}
