using System;
using System.Collections.Generic;
using NUnit.Framework;
using VanillaTransformer.Core;
using VanillaTransformer.Core.Configuration;

namespace VanillaTransformer.Tests.CoreTest
{
    public class TransformationResultsTests
    {
        [Test]
        public void should_be_able_to_present_paths_correctly_for_success()
        {
            //ARRANGE
            var transformationResults = new TransformationResults();
            transformationResults.AddSuccess(new TransformConfiguration()
            {
                PatternFilePath = @"c:\test1\test2\test4\templates\Apps/App1/appsettings.json",
                OutputFilePath = @"c:\test1\test2\test4/templates\../config/Apps/App1/appsettings.json",
            });
            string result = null;

            //ACT
            transformationResults.PrintDescription(s => { result = s;}, s => {}, @"c:\test1\test2\test4\");

            //ASSERT
            Assert.AreEqual(@"templates\Apps\App1\appsettings.json -> config\Apps\App1\appsettings.json  [OK]", result);
        }
        
        [Test]
        public void should_be_able_to_present_paths_correctly_for_success_when_archive()
        {
            //ARRANGE
            var transformationResults = new TransformationResults();
            transformationResults.AddSuccess(new TransformConfiguration()
            {
                PatternFilePath = @"c:\test1\test2\test4\templates\Apps/App1/appsettings.json",
                OutputFilePath = @"appsettings.json",
                OutputArchive = @"c:\test1\test2\test4/templates\../config/Apps/App1.zip"
            });
            string result = null;

            //ACT
            transformationResults.PrintDescription(s => { result = s;}, s => {}, @"c:\test1\test2\test4\");

            //ASSERT
            Assert.AreEqual(@"templates\Apps\App1\appsettings.json -> config\Apps\App1.zip!appsettings.json  [OK]", result);
        }
        
        [Test]
        public void should_be_able_to_present_paths_correctly_for_fail()
        {
            //ARRANGE
            var transformationResults = new TransformationResults();
            transformationResults.AddFail(new TransformConfiguration()
            {
                PatternFilePath = @"c:\test1\test2\test4\templates\Apps/App1/appsettings.json",
                OutputFilePath = @"c:\test1\test2\test4/templates\../config/Apps/App1/appsettings.json",
            }, new Exception("Something went wrong"));
            var results = new List<string>();

            //ACT
            transformationResults.PrintDescription(s => { }, s => { results.Add(s); }, @"c:\test1\test2\test4\");

            //ASSERT
            Assert.AreEqual(@"templates\Apps\App1\appsettings.json -> config\Apps\App1\appsettings.json  [ERROR]", results[0]);
            Assert.AreEqual("\tCause by: [System.Exception] Something went wrong", results[1]);
        }
    }
}
