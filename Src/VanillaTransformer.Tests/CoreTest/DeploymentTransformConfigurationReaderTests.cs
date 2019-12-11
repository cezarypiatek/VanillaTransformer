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
                <template name=""nlog.config.template"" pattern=""nlog.config.template"" />
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
                            <add key=""HostIp"">192.168.0.1</add>
                        </values>
                    </machine>
                </machines>
            </environment>
    </environments>
    <transformations>
        <transformation output=""{app}/{environment}/{machine}/{template}"" />
    </transformations>
    <postTransformations>
        <postTransformation fileExtension=""xml"">
             <add name=""StripXMLComments"" />
             <add name=""ReFormatXML"" />
        </postTransformation>
    </postTransformations>
</root>
";
           var xmlReader = TransformConfgReaderTestFactory.CreateXmlTextFileReader("test.xml", config);
           var deploymentReader = new DeploymentTransformConfigurationReader(xmlReader);

           //ACT
           var result = deploymentReader.ReadConfig("test.xml");
           
           //ASSERT
           Assert.IsNotNull(result);
           Assert.AreEqual(4, result.ToList().Count);
        }
    }
}