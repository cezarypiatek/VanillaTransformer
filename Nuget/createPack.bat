@echo off
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)
set config="Release"
nuget pack ..\Src\VanillaTransformer\VanillaTransformerTask.nuspec -NoPackageAnalysis -verbosity detailed -o ./ -Version %version% -p Configuration="%config%"