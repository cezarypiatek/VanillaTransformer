# VanillaTransformer  
[![vanillafeed MyGet Build Status](https://www.myget.org/BuildSource/Badge/vanillafeed?identifier=a5bad1f8-7580-4e5d-a7ad-2952d2c88719)](https://www.myget.org/)

VanillaTransformer is a simple generic text file transformer. It was designed for configuration transforming as an alternative for [XML-Document-Transform](https://msdn.microsoft.com/en-us/library/dd465326%28v=vs.110%29.aspx) tool. Unlike XML-DT, it works with any kind of text file (not only XML) and is much simpler to use.
##Nuget package
https://www.nuget.org/packages/VanillaTransformer/

##How to use it.
1. Create pattern file
2. Create values files
3. Add transformation within build target

Currently VanillaTransformer works as **MsBuildTask**. You can install it using nuget or add it manually to your *.csproj file as follows:
```XML
<UsingTask TaskName="VanillaTransformerTask" AssemblyFile="VanillaTransformer.dll" />
```
After that you can start adding transformation within given build target
```XML
<Target Name="AfterBuild">   
	<VanillaTransformerTask PatternFile="Configs\NHibernate.pattern.config" ValuesSource="Configs\NHibernate.values.dev.config" OutputPath="NHibernate.config" />
    <VanillaTransformerTask PatternFile="Configs\NHibernate.pattern.config" ValuesSource="Configs\NHibernate.values.test.config" OutputPath="Configs\Transformed\NHibernate.Test.config" />
	<VanillaTransformerTask PatternFile="Configs\NHibernate.pattern.config" ValuesSource="Configs\NHibernate.values.staging.config" OutputPath="Configs\Transformed\NHibernate.Staging.config" />
 </Target>
```
**Example Pattern file**
```XML
<?xml version="1.0" encoding="utf-8"?>
<hibernate-configuration xmlns="urn:nhibernate-configuration-2.2">
  <session-factory>
    <property name="connection.connection_string">${ConnectionString}</property>
    <property name="connection.driver_class">NHibernate.Driver.SqlClientDriver</property>
    <property name="dialect">NHibernate.Dialect.MsSql2008Dialect</property>
  </session-factory>
</hibernate-configuration>
```
**Example Values file**

```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <ConnectionString>Data Source=localhost;Database=TestDB;Integrated Security=true;</ConnectionString>
</root>
```

You can also add transformation using powershell command provided with nuget package:
```PowerShell
Add-Transformation "Configs\NHibernate.pattern.config" "Configs\NHibernate.values.dev.config" "NHibernate.config"
```

##Transformation Configuration File
Instead of adding each transformation by editing *.csproj file (what is very uncomfortable) you can create transformation configuration file with all transformations and add it once to project file. To register the configuration file add the following element into csproj file:

```XML
<Target Name="AfterBuild">
	<VanillaTransformerTask TransformConfiguration="Configs\transformations.xml" />
</Target>
```

or use PowerShell command

```PowerShell
Add-TransformationConfig "Configs\transformations.xml"
```

**Example transformation configuration file**
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation values="NHibernate.values.dev.config" output="NHibernate.config" />
    <transformation values="NHibernate.values.staging.config" output="Configs\Transformed\NHibernate.config" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <transformation values="Web.values.dev.config" output="Web.config" />
    <transformation values="Web.values.staging.config" output="Configs\Transformed\Web.config" />
  </transformationGroup>
</root>
```
You can also define inline values for transformation instead of putting it in separated files.
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation output="NHibernate.config">
    	<values>
    		<ConnectionString>Data Source=localhost;Database=TestDB;Integrated Security=true;</ConnectionString>
    	</values>
    </transformation>
    <transformation output="Configs\Transformed\NHibernate.config">
    	<values>
    		<ConnectionString>Data Source=localhost;Database=StagingDB;Integrated Security=true;</ConnectionString>
    	</values>
    </transformation>
  </transformationGroup>
</root>
```
##How to run transformations
By default VanilaTransformerTask is added to AfterBuild target, so everytime you build your project transformation should be run (unless you have condition attribute on AfterBuild target). You can also run the transformation using the following command

```PowerShell
Invoke-Transformations
`````
This will search for all VanillaTransformerTask inside the AfterBuild target and run it. If you don't want to run all the transformations, you can specify file with transformations (Transformation Configuration File) to run

```PowerShell
Invoke-Transformations -ConfigFilePath transformations.xml
`````

##Bootstrapping transformations
If you already have configurations files and you wan to add transformation for few evenronments (for example: dev, test, stag, prod) you can use the following command
```PowerShell
Add-BoostrapConfig -Enviroments dev, test, prep, prod -DefaultEnvironment dev -TransformationsOut TransformedConfigs
```
Add-BootstrapConfig command does the following:


1. Search for config files inside your project directory (by default files with *.config extension, but you could change that by using -ConfigFilter parameter) It search only inside the project main directory (you can use -Recurse parameter to search also in subdirectories)

2. For each file creates pattern file using original config file and adds values file for each environment. Result of transformation for DefaultEnviroment will override original config file. The rest of transformed files will be placed in directory pointed by  -TransformationsOut parameter.

3. Creates transformations.xml file that describes all the transformations

4. Adds VanillaTransformerTask to AfterBuild target.


Sample project before bootstrapping


![Before bootstrapping](https://raw.githubusercontent.com/cezarypiatek/VanillaTransformer/master/Doc/bootstrap_before.jpg)

... and after bootsrapping


![After bootstrapping](https://raw.githubusercontent.com/cezarypiatek/VanillaTransformer/master/Doc/bootstrap_after.jpg)


##Post-Transformations
VanillaTransformer supports post-transformations which are apllied to transformed configuration in the form of pipeline.
You can add post-transformations on any level of VanillaTransformer configuration (root, transformationGroup or transformation level) in the following manner:

```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <postTransformations>
    <add name="StripXMLComments" />
    <add name="ReFormatXML" />
  </postTransformations>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation values="Configs\NHibernate.values.dev.config" output="NHibernate.config" />
    <transformation values="Configs\NHibernate.values.prod.config" output="Configs\Transformed\NHibernate.prod.config" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <transformation values="Configs\Web.values.dev.config" output="Web.config" />
    <transformation values="Configs\Web.values.prod.config" output="Configs\Transformed\Web.prod.config" />
  </transformationGroup>
</root>
```

You can also modify post-transformations set defined on higher level using "add" , "remove", and "clear" tags as follows
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <postTransformations>
    <add name="StripXMLComments" />
    <add name="ReFormatXML" />
  </postTransformations>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation values="Configs\NHibernate.values.dev.config" output="NHibernate.config">
      <postTransformations>
        <remove name="StripXMLComments" />
      </postTransformations>
    </transformation>
    <transformation values="Configs\NHibernate.values.prod.config" output="Configs\Transformed\NHibernate.prod.config" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <postTransformations>
      <clear />
    </postTransformations>
    <transformation values="Configs\Web.values.dev.config" output="Web.config" >
      <postTransformations>
        <add name="ReFormatXML" />
      </postTransformations>
    </transformation>
    <transformation values="Configs\Web.values.prod.config" output="Configs\Transformed\Web.prod.config" />
  </transformationGroup>
</root>
```

Currently available post-transformations:
* StripXMLComments
* ReFormatXML
* DateTimeStamp
