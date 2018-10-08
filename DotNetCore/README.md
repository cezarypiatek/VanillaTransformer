### `VanillaTransformer` targeting .NET Standard 2.0 ###

1. MSBuild Task
Usage:
```xml
<ItemGroup>
    <PackageReference Include="VanillaTransformer" Version="1.0.0" />
</ItemGroup>

<VanillaTransformerTask
    PatternFile="input-pattern.xml"
    PlaceholderPattern="${{KEY}}"
    ValuesSource="replacement-values.xml"
    OutputPath="output.xml" />
</Target>
```

2. .NET Core global tool
a. Development
`-PackageOutputPath ` - optional parameter that specifies .nupkg file output path.
`-Version` - optional parameter that specifies the version of NuGet package; '1.0.0' by default.
To create a package
`PS> ./pack-as-tool.ps1 -PackageOutputPath '<script location>' -Version '<1.0.0>'`
To install .NET Core global tool
`> dotnet tool install --global --add-source ./nupkg VanillaTransformer.Tool`
usage
`> transform -?`
b. From NuGet
install 
`> dotnet tool install --global VanillaTransformer.Tool`
usage
`> transform -?`
