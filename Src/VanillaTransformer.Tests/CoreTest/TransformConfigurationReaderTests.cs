using System.Linq;
using NUnit.Framework;
using VanillaTransformer.Tests.ValuesProviders;

namespace VanillaTransformer.Tests.CoreTest
{
    [TestFixture]
    public class TransformConfigurationReaderTests
    {
        [Test]
        public void should_be_able_to_read_transformations_from_file()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                </transformationGroup>
                            </root>")
            };
            
            //ACT
            var result = configurationReader.ReadFromFile(testFilePath);

            //ASSERT
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual("aaa.pattern.xml",result[0].PatternFilePath);
            Assert.IsNotNull(result[0].ValuesProvider);
            Assert.AreEqual("output.xml",result[0].OutputFilePath);
        }

        [Test]
        public void should_be_able_to_read_many_transformation_for_single_pattern()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                    <transformation values=""aaa1.values.xml"" output=""output1.xml"" />
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var result = configurationReader.ReadFromFile(testFilePath);

            //ASSERT
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
        }


        [Test]
        public void should_be_able_to_read_inline_values_from_transformation_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation output=""output.xml"">
                                        <values>
                                            <Val1>AAA</Val1>
                                        </values>                                    
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);
            var transformationToTest = transformConfigurations.First();

            //ACT
            var values = transformationToTest.ValuesProvider.GetValues();


            //ASSERT
            Assert.IsNotNull(values);
            Assert.IsTrue(values.ContainsKey("Val1"));
            Assert.AreEqual(values["Val1"], "AAA");
        }
    }
}
