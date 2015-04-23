@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)
 
set nuget=
if "%nuget%" == "" (
	set nuget=nuget
)
 
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Src\VanillaTransformer\VanillaTransformer.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false 
xcopy  "Src\VanillaTransformer\bin\%config%" "Nuget\tools\" /s/h/e/k/f/c/Y

mkdir Build

%nuget% pack "Nuget\VanillaTransformer.nuspec" -NoPackageAnalysis -verbosity detailed -o Build -Version %version% -p Configuration="%config%"