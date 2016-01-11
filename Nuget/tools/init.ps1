param($installPath, $toolsPath, $package)

Import-Module (Join-Path $toolsPath VanillaTransformerCommands.psm1) -ArgumentList $installPath, $toolsPath, $package