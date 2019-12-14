VanillaTransformer works as **MsBuildTask**. You can install it using nuget or add it manually to your *.csproj file as follows:
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
By default VanillaTransformer uses DollarPlaceholderTransformer which replaces placeholders in the following format

```plaintext
${Placeholder_Name}
```
If this type of placeholder collides with your file content (ex. NLog config files) you can define custom placeholder pattern using `PlaceholderPatter` parameter (the value must contain `KEY` keyword, for example `${KEY}`).

To change placeholder pattern set **PlaceholderPatter** parameter to appropriate value
```XML
<VanillaTransformerTask 
	PatternFile="Configs\NHibernate.pattern.config" 
	ValuesSource="Configs\NHibernate.values.dev.config" 
	OutputPath="NHibernate.config" 
	PlaceholderPatter="#[KEY]" />
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

## Transformation Configuration File
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

## How to run transformations
By default VanillaTransformerTask is added to AfterBuild target, so every time you build your project transformation should be run (unless you have condition attribute on AfterBuild target). You can also run the transformation using the following command

```PowerShell
Invoke-Transformations
```
This will search for all VanillaTransformerTask inside the AfterBuild target and run it. If you don't want to run all the transformations, you can specify file with transformations (Transformation Configuration File) to run

```PowerShell
Invoke-Transformations -ConfigFilePath transformations.xml
```

## Bootstrapping transformations
If you already have configurations files and you wan to add transformation for few environments (for example: dev, test, stag, prod) you can use the following command
```PowerShell
Add-BootstrapConfig -Environments dev, test, prep, prod -DefaultEnvironment dev -TransformationsOut TransformedConfigs
```

Add-BootstrapConfig command does the following:

1. Search for config files inside your project directory (by default files with *.config extension, but you could change that by using -ConfigFilter parameter) It search only inside the project main directory (you can use -Recurse parameter to search also in subdirectories)

2. For each file creates pattern file using original config file and adds values file for each environment. Result of transformation for DefaultEnvironment will override original config file. The rest of transformed files will be placed in directory pointed by  -TransformationsOut parameter.

3. Creates transformations.xml file that describes all the transformations

4. Adds VanillaTransformerTask to AfterBuild target.


Sample project before bootstrapping


![Before bootstrapping](/Doc/bootstrap_before.jpg)

... and after bootsrapping


![After bootstrapping](/Doc/bootstrap_after.jpg)

