@echo off
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)
nuget pack VanillaTransformer.nuspec -NoPackageAnalysis -verbosity detailed -o Build -Version %version% -p Configuration="%config%"