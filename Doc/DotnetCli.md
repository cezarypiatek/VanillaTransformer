# Dotnet CLI tool

## Use it as a global tool

Installation:


```shell
dotnet tool install --global dotnet-configtransform
```

Usage:

```shell
dotnet-configtransform --TransformConfiguration "transformations.xml" --ConfigurationFormat "deployment"
```


## Use it as a local tool

Installation:

```shell
dotnet new tool-manifest
dotnet tool install dotnet-configtransform
```

Usage:

```shell
dotnet-configtransform --TransformConfiguration "transformations.xml" --ConfigurationFormat "deployment"
```

## Parameters

All the parameters are the same as for [Console app](/Doc/ConsoleApp.md) version.
