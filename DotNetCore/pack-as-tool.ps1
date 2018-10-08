param(
    [String]$PackageOutputPath=$PSScriptRoot,
    [String]$Version='1.0.0'
)

dotnet pack .\src\VanillaTransformer.Console\VanillaTransformer.Tool.csproj `
    /p:PackageOutputPath="$PackageOutputPath" `
    /p:Version=$Version