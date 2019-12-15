using System.Linq;
using NUnit.Framework;
using VanillaTransformer.Core.Configuration;

namespace VanillaTransformer.Tests.CoreTest
{
    public class DeploymentTransformConfigurationReaderTests
    {
        [Test]
        public void should_be_able_to_read_deploy_config_format()
        {
           var config = @"
<root>
    <apps>
        <app name=""SampleApp1"">
            <templates>
                <template name=""appsettings.json"" pattern=""appsettings.json.template"" placeholder=""[%KEY%]"" />
                <template name=""nlog.config"" pattern=""nlog.config.template"" />
            </templates>
        </app>
    </apps>
    <environments>
        <environment name=""PPE"">
                <values>
                    <add key=""Value1"">1</add>
                    <add key=""Value2"">2</add>
                    <add key=""Value3"">3</add>
                </values>
                <machines>
                    <machine name=""ASP-01"">
                        <values>
                            <add key=""HostName"">ASP-01</add>
                            <add key=""HostIp"">192.168.0.1</add>
                            <add key=""Value1"">One</add>
                        </values>
                    </machine>
                    <machine name=""ASP-02"" apps=""SampleApp1"">
                        <values>
                            <add key=""HostName"">ASP-02</add>
                            <add key=""HostIp"">192.168.0.2</add>
                        </values>
                    </machine>
                </machines>
            </environment>
    </environments>
    <transformations>
        <transformation output=""{app}\{environment}\{machine}\{template}"" />
    </transformations>
    <postTransformations>
        <postTransformation fileExtension=""config"">
             <add name=""StripXMLComments"" />
             <add name=""ReFormatXML"" />
        </postTransformation>
        <postTransformation fileExtension=""json"">
             <add name=""ReFormatJSON"" />
        </postTransformation>
    </postTransformations>
</root>
";
           var xmlReader = TransformConfgReaderTestFactory.CreateXmlTextFileReader("test.xml", config);
           var deploymentReader = new DeploymentTransformConfigurationReader(xmlReader);

           //ACT
           var result = deploymentReader.ReadConfig("test.xml")?.ToList();
           
           //ASSERT
           Assert.IsNotNull(result);
           Assert.AreEqual(4, result.Count);

           {
               var transformation = result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PPE\\ASP-01\\appsettings.json"));
               Assert.IsNotNull(transformation);
               Assert.AreEqual("[%KEY%]", transformation.PlaceholderPattern);
               var values = transformation.ValuesProvider.GetValues();
               Assert.AreEqual(5, values.Count);
               Assert.AreEqual("One", values["Value1"]);
               Assert.AreEqual("2", values["Value2"]);
               Assert.AreEqual("3", values["Value3"]);
               Assert.AreEqual("ASP-01", values["HostName"]);
               Assert.AreEqual("192.168.0.1", values["HostIp"]);
               Assert.AreEqual(1, transformation.PostTransformations.Count);
           }
           {
               var transformation = result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PPE\\ASP-01\\nlog.config"));
               Assert.IsNotNull(transformation);
               Assert.AreEqual(null, transformation.PlaceholderPattern);
               var values = transformation.ValuesProvider.GetValues();
               Assert.AreEqual(5, values.Count);
               Assert.AreEqual("One", values["Value1"]);
               Assert.AreEqual("2", values["Value2"]);
               Assert.AreEqual("3", values["Value3"]);
               Assert.AreEqual("ASP-01", values["HostName"]);
               Assert.AreEqual("192.168.0.1", values["HostIp"]);
               Assert.AreEqual(2, transformation.PostTransformations.Count);
           }
           {
               var transformation = result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PPE\\ASP-02\\appsettings.json"));
               Assert.IsNotNull(transformation);
               Assert.AreEqual("[%KEY%]", transformation.PlaceholderPattern);
               var values = transformation.ValuesProvider.GetValues();
               Assert.AreEqual(5, values.Count);
               Assert.AreEqual("1", values["Value1"]);
               Assert.AreEqual("2", values["Value2"]);
               Assert.AreEqual("3", values["Value3"]);
               Assert.AreEqual("ASP-02", values["HostName"]);
               Assert.AreEqual("192.168.0.2", values["HostIp"]);
               Assert.AreEqual(1, transformation.PostTransformations.Count);
           }
           {
               var transformation = result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PPE\\ASP-02\\nlog.config"));
               Assert.IsNotNull(transformation);
               Assert.AreEqual(null, transformation.PlaceholderPattern);
               var values = transformation.ValuesProvider.GetValues();
               Assert.AreEqual(5, values.Count);
               Assert.AreEqual("1", values["Value1"]);
               Assert.AreEqual("2", values["Value2"]);
               Assert.AreEqual("3", values["Value3"]);
               Assert.AreEqual("ASP-02", values["HostName"]);
               Assert.AreEqual("192.168.0.2", values["HostIp"]);
               Assert.AreEqual(2, transformation.PostTransformations.Count);
           }
        }
        
        [Test]
        public void should_be_able_to_read_deploy_config_format_with_unified_environments()
        {
           var config = @"
<root>
    <apps>
        <app name=""SampleApp1"">
            <templates>
                <template name=""appsettings.json"" pattern=""appsettings.json.template"" />
            </templates>
        </app>
    </apps>
    <environments>
        <environment name=""PPE"">
                <values>
                    <add key=""Value1"">1</add>
                    <add key=""Value2"">2</add>
                    <add key=""Value3"">3</add>
                </values>
        </environment>
        <environment name=""PROD"">
                <values>
                    <add key=""Value1"">PROD1</add>
                    <add key=""Value2"">PROD2</add>
                    <add key=""Value3"">PROD3</add>
                </values>
        </environment>
    </environments>
    <transformations>
        <transformation output=""{app}\{environment}\{template}"" />
    </transformations>
</root>
";
           var xmlReader = TransformConfgReaderTestFactory.CreateXmlTextFileReader("test.xml", config);
           var deploymentReader = new DeploymentTransformConfigurationReader(xmlReader);

           //ACT
           var result = deploymentReader.ReadConfig("test.xml")?.ToList();
           
           //ASSERT
           Assert.IsNotNull(result);
           Assert.AreEqual(2, result.Count);

           {
               var transformation = result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PPE\\appsettings.json"));
               Assert.IsNotNull(transformation);
               var values = transformation.ValuesProvider.GetValues();
               Assert.AreEqual(3, values.Count);
               Assert.AreEqual("1", values["Value1"]);
               Assert.AreEqual("2", values["Value2"]);
               Assert.AreEqual("3", values["Value3"]);
           }
           {
               var transformation = result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PROD\\appsettings.json"));
               Assert.IsNotNull(transformation);
               var values = transformation.ValuesProvider.GetValues();
               Assert.AreEqual(3, values.Count);
               Assert.AreEqual("PROD1", values["Value1"]);
               Assert.AreEqual("PROD2", values["Value2"]);
               Assert.AreEqual("PROD3", values["Value3"]);

           }
        } 
        
        [Test]
        public void should_be_able_to_restrict_transformation_on_env_and_machine_level()
        {
           var config = @"
<root>
    <apps>
        <app name=""SampleApp1"">
            <templates>
                <template name=""appsettings.json"" pattern=""appsettings.json.template"" />
            </templates>
        </app>
        <app name=""SampleApp2"">
            <templates>
                <template name=""appsettings.json"" pattern=""appsettings.json.template"" />
            </templates>
        </app>
    </apps>
    <environments>
        <environment name=""PPE"">
             <machines>
                <machine name=""ASP-01"" />
                <machine name=""ASP-02"" apps=""SampleApp1"" />
            </machines>
        </environment>
        <environment name=""PROD"" apps=""SampleApp2"">
            <machines>
                <machine name=""PASP-01"" />
                <machine name=""PASP-02"" />
            </machines>
        </environment>
    </environments>
    <transformations>
        <transformation output=""{app}\{environment}\{machine}\{template}"" />
    </transformations>
</root>
";
           var xmlReader = TransformConfgReaderTestFactory.CreateXmlTextFileReader("test.xml", config);
           var deploymentReader = new DeploymentTransformConfigurationReader(xmlReader);

           //ACT
           var result = deploymentReader.ReadConfig("test.xml")?.ToList();
           
           //ASSERT
           Assert.IsNotNull(result);
           Assert.AreEqual(5, result.Count);
           Assert.IsNotNull(result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PPE\\ASP-01\\appsettings.json")));
           Assert.IsNotNull(result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp1\\PPE\\ASP-02\\appsettings.json")));
           Assert.IsNotNull(result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp2\\PPE\\ASP-01\\appsettings.json")));
           Assert.IsNotNull(result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp2\\PROD\\PASP-01\\appsettings.json")));
           Assert.IsNotNull(result.FirstOrDefault(x => x.OutputFilePath.EndsWith("SampleApp2\\PROD\\PASP-02\\appsettings.json")));
        }
    }
}