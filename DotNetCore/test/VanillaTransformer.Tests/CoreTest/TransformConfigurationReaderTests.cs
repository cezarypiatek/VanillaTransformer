using System.Linq;
using VanillaTransformer.Core.Configuration;
using VanillaTransformer.Tests.ValuesProviders;
using Xunit;

namespace VanillaTransformer.Tests.CoreTest
{
    public class TransformConfigurationReaderTests
    {
        [Fact]
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
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("aaa.pattern.xml",result[0].PatternFilePath);
            Assert.NotNull(result[0].ValuesProvider);
            Assert.Equal("output.xml",result[0].OutputFilePath);
        }


        [Fact]
        public void should_be_able_to_read_transformations_from_file_with_placeholder_pattern()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"" placeholderPattern=""#[KEY]"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                </transformationGroup>
                            </root>")
            };
            
            //ACT
            var result = configurationReader.ReadFromFile(testFilePath);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("aaa.pattern.xml",result[0].PatternFilePath);
            Assert.NotNull(result[0].ValuesProvider);
            Assert.Equal("#[KEY]", result[0].PlaceholderPattern);
            Assert.Equal("output.xml",result[0].OutputFilePath);
        }

        [Fact]
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
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }


        [Fact]
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
            Assert.NotNull(values);
            Assert.True(values.ContainsKey("Val1"));
            Assert.Equal("AAA", values["Val1"]);
        }  
        
        
        [Fact]
        public void should_be_able_to_read_post_transformation_from_root_node_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""ReFormatXML"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.NotNull(transformationToTest.PostTransformations);
            Assert.Single(transformationToTest.PostTransformations);
            Assert.Equal("ReFormatXML", transformationToTest.PostTransformations.First().Name);
        }


        [Fact]
        public void should_be_able_to_read_post_transformation_from_group_node_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                     <postTransformations>
                                        <add name=""ReFormatXML"" />
                                    </postTransformations>
                                    <transformation values=""aaa.values.xml"" output=""output.xml"" />
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.NotNull(transformationToTest.PostTransformations);
            Assert.Single(transformationToTest.PostTransformations);
            Assert.Equal("ReFormatXML", transformationToTest.PostTransformations.First().Name);
        }

        [Fact]
        public void should_be_able_to_read_post_transformation_from_transformation_node_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <add name=""ReFormatXML"" />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.NotNull(transformationToTest.PostTransformations);
            Assert.Single(transformationToTest.PostTransformations);
            Assert.Equal("ReFormatXML", transformationToTest.PostTransformations.First().Name);
        }
        
        [Fact]
        public void should_be_able_to_extend_post_transformation_on_lower_level_of_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""StripXMLComments"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <add name=""ReFormatXML"" />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.NotNull(transformationToTest.PostTransformations);
            Assert.Equal(2, transformationToTest.PostTransformations.Count);
            Assert.Equal("StripXMLComments", transformationToTest.PostTransformations[0].Name);
            Assert.Equal("ReFormatXML", transformationToTest.PostTransformations[1].Name);
        } 
        
        [Fact]
        public void should_be_able_to_supress_post_transformation_on_lower_level_of_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""StripXMLComments"" />
                                    <add name=""ReFormatXML"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <remove name=""StripXMLComments"" />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.NotNull(transformationToTest.PostTransformations);
            Assert.Single(transformationToTest.PostTransformations);
            Assert.Equal("ReFormatXML", transformationToTest.PostTransformations[0].Name);
        } 
        
        [Fact]
        public void should_be_able_to_suppress_all_post_transformation_on_lower_level_of_configuration()
        {
            //ARRANGE
            const string testFilePath = "test.xml";
            var configurationReader = new TransformConfigurationReader
            {
                FileReader = TextFileReaderTestsHelpers.GetTextFileReaderMock(testFilePath, @"
                            <root>
                                <postTransformations>
                                    <add name=""StripXMLComments"" />
                                    <add name=""ReFormatXML"" />
                                </postTransformations>
                                <transformationGroup pattern=""aaa.pattern.xml"">
                                    <transformation values=""aaa.values.xml"" output=""output.xml"">
                                        <postTransformations>
                                            <clear />
                                        </postTransformations>
                                    </transformation>
                                </transformationGroup>
                            </root>")
            };

            //ACT
            var transformConfigurations = configurationReader.ReadFromFile(testFilePath);


            //ASSERT
            var transformationToTest = transformConfigurations.First();
            Assert.NotNull(transformationToTest.PostTransformations);
            Assert.Empty(transformationToTest.PostTransformations);
        }
    }
}
