# VanillaTransformer  
[![vanillafeed MyGet Build Status](https://www.myget.org/BuildSource/Badge/vanillafeed?identifier=a5bad1f8-7580-4e5d-a7ad-2952d2c88719)](https://www.myget.org/)

VanillaTransformer is a simple generic text file transformer. It was designed for configuration transforming as an alternative for [XML-Document-Transform](https://msdn.microsoft.com/en-us/library/dd465326%28v=vs.110%29.aspx) tool. Unlike XML-DT, it works with any kind of text file (not only XML) and is much simpler to use.
##Nuget package
https://www.nuget.org/packages/VanillaTransformer/

##How to use it.
1. Create pattern file
2. Create values files
3. Add transformation within build target

Currently VanillaTransformer works as **MsBuildTask**. You can install it using nuget or add it manually to your ***.csproj** file as follows:
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


