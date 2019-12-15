# VanillaTransformer  
[![Build status](https://ci.appveyor.com/api/projects/status/i0p9uvjxn0or6aif/branch/master?svg=true)](https://ci.appveyor.com/project/cezarypiatek/vanillatransformer/branch/master)

VanillaTransformer is a simple generic text file transformer. It was designed for configuration transforming as an alternative for [XML-Document-Transform](https://msdn.microsoft.com/en-us/library/dd465326%28v=vs.110%29.aspx) tool. Unlike XML-DT, it works with any kind of text file (not only XML) and is much simpler to use.

## How it works

- Prepare template files for your config files by replacing fragments that should be changed with placeholders
- Prepare `TransformConfiguration` file that describes all the transformations
- Run `VanillaTransformer` against `TransformConfiguration` file

## How to install
The installation of VanillaTransformer depends on the use case. VanillaTransformer is shipped in the following forms:

- **MsBuildTask** - use Nuget to install [VanillaTransformer](https://www.nuget.org/packages/VanillaTransformer/) package
- **Console application** - You can download the latest binaries from the `Release` section.
- **dotnet CLI tool** - install using `dotnet tool install dotnet-configtransform`

## How to use it.
- [Use as MSBuildTask](/Doc/MsBuildTask.md)
- [Use as console application](/Doc/ConsoleApp.md)
- [Use as dotnet cli tool](/Doc/DotnetCli.md)

## TransformConfiguration file formats
Currently Vanilla transformer supports the following formats for describing transformations:

- [default](/Doc/VanillaFormat.md) - the default format invented at the beginning of VanillaTransformer
- [deployment](/Doc/DeploymentFormat.md) - an improve form of describing transformations, better aligned to deployments scenarios

## Post-Transformations
VanillaTransformer supports post-transformations which are applied to a transformed configuration in the form of the pipelines.
The way of defining post-transformations depends on the `TransformConfiguration` file format. 

Currently available post-transformations:
* StripXMLComments
* ReFormatXML
* ReFormatJSON
