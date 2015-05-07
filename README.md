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
	<VanillaTransformerTask PatternFile="Configs\NHibernate.pattern.config" ValuesSource="Configs\NHibernate.values.stagging.config" OutputPath="Configs\Transformed\NHibernate.Stagging.config" />
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
    <transformation values="NHibernate.values.stagging.config" output="Configs\Transformed\NHibernate.config" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <transformation values="Web.values.dev.config" output="Web.config" />
    <transformation values="Web.values.stagging.config" output="Configs\Transformed\Web.config" />
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
    		<ConnectionString>Data Source=localhost;Database=StaggingDB;Integrated Security=true;</ConnectionString>
    	</values>
    </transformation>
  </transformationGroup>
</root>
```
