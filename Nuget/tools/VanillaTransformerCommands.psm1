param($installPath, $toolsPath, $package)

function Add-Transformation($pattern, $values, $output){
	$project = Get-Project
	$buildProject =  @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName))[0]
	$afterBuildTarget= $buildProject.Xml.Targets | Where-Object {$_.Name -eq "AfterBuild"}
	if($afterBuildTarget -eq $null)
	{
		$afterBuildTarget = $buildProject.Xml.AddTarget("AfterBuild")
		Write-Host "Build target 'AfterBuild' has been created successfully."
	}
    
    $existingTask = Get-VanillaTransformerTasks |? {($_.GetParameter("PatternFile") -eq $pattern) -and ($_.GetParameter("ValuesSource") -eq $values) -and ($_.GetParameter("OutputPath") -eq $output)}
    if($existingTask)
    {
        Write-Host "Task with PatternFile '$pattern', ValuesSource '$values' and OutputPath '$output' already exists"
        return
    }

	$task = $afterBuildTarget.AddTask("VanillaTransformerTask")
	$task.SetParameter("PatternFile", $pattern)
	$task.SetParameter("ValuesSource", $values)
	$task.SetParameter("OutputPath",$output)
	$buildProject.Save()
	$project.Save()
	Write-Host "Transformation has been added successfully."
}

function Add-TransformationConfig($configFilePath){
	$project = Get-Project
	$buildProject =  @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName))[0]
	$afterBuildTarget= $buildProject.Xml.Targets | Where-Object {$_.Name -eq "AfterBuild"}
	if($afterBuildTarget -eq $null)
	{
		$afterBuildTarget = $buildProject.Xml.AddTarget("AfterBuild")
		Write-Host "Build target 'AfterBuild' has been created successfully."
	}
	
    $existingTask = Get-VanillaTransformerTasks |? {$_.GetParameter("TransformConfiguration") -eq $configFilePath}
    if($existingTask)
    {
        Write-Host "Task with transformation config '$configFilePath' already exists"
        return
    }
    $task = $afterBuildTarget.AddTask("VanillaTransformerTask")
	$task.SetParameter("TransformConfiguration", $configFilePath)	
	$buildProject.Save()
	$project.Save()
	Write-Host "Transformation configuration has been added successfully."
}

function Get-VanillaTransformerTasks{
    $project = Get-Project
	$buildProject =  @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName))[0]
	$afterBuildTarget= $buildProject.Xml.Targets | Where-Object {$_.Name -eq "AfterBuild"}
	if($afterBuildTarget -ne $null)
	{
        $afterBuildTarget.Tasks | Where-Object {$_.Name -eq "VanillaTransformerTask"}
    }
}

function Invoke-TransformationTask($transformationTask)
{
    if(-not([System.String]::IsNullOrWhiteSpace($transformationTask.TransformConfiguration)))
    {
        Write-Verbose "Execute VanillaTransformerTask with parameter: 'TransformConfiguration'='$($transformationTask.TransformConfiguration)'"
    }else{
        Write-Verbose "Execute VanillaTransformerTask with parameters: 'PatternFile'='$($transformationTask.PatternFile)' 'ValuesSource'='$($transformationTask.ValuesSource)' 'OutputPath'='$($transformationTask.OutputPath)'"
    }
            
    if(-not ($transformationTask.Execute()))
    {
        Write-Verbose "Transformation Failed"
    }else{
        Write-Verbose "Transformation Success"
    }
}

function Load-VanillaTransformerLib{
    $dllPath = Join-Path $toolsPath VanillaTransformer.dll
    if((Test-Path $dllPath) -eq $false)
    {
        $dllPath = VanillaTransformer.dll
    }    
    Add-Type -Path $dllPath
}

<#
.SYNOPSIS

Run transformations of config files

.Parameter ConfigFilePath
Define file with transformation configuration for which transformation should be run. 
If not specified, all transformations defined as VanillaTransformerTask inside AfterBuild traget will be run.

#>
function Invoke-Transformations
{
    [CmdletBinding()]
    param($ConfigFilePath)
    Load-VanillaTransformerLib
    
    $oldPath = Get-Location
    $projectPath = (Get-Project | Select -ExpandProperty FullName |Split-Path -Parent)
    [System.IO.Directory]::SetCurrentDirectory($projectPath)
        
    if(-not([System.String]::IsNullOrWhiteSpace($ConfigFilePath)))
    {
        $transformationTask = New-Object VanillaTransformer.VanillaTransformerTask
        $transformationTask.TransformConfiguration = $ConfigFilePath
        Invoke-TransformationTask $transformationTask
    }else{
        Get-VanillaTransformerTasks |% {
            $transformationTask = New-Object VanillaTransformer.VanillaTransformerTask
            $transformationTask.TransformConfiguration = $_.GetParameter("TransformConfiguration")
            $transformationTask.PatternFile = $_.GetParameter("PatternFile")
	        $transformationTask.ValuesSource = $_.GetParameter("ValuesSource")
	        $transformationTask.OutputPath = $_.GetParameter("OutputPath")
            Invoke-TransformationTask $transformationTask
        }
    }    
    [System.IO.Directory]::SetCurrentDirectory($oldPath)
}

function Get-Confirmation($message){

    $title = ""
    $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Create transformation config."
    $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Ignore file."
    $options = [System.Management.Automation.Host.ChoiceDescription[]]($yes, $no)
    $result = $host.ui.PromptForChoice($title, $message, $options, 0) 
    switch ($result)
    {
        0 { $true }
        1 { $false}
    }
}

function Search-ItemsRecursive{
  [CmdletBinding()]
  param($projectItems, $filter, [switch]$recurse=$false)
  foreach ($item in $projectItems) {
    if($(. $filter $item))
    {
        $item
    }else{
        if(($item.ProjectItems -ne $null) -and $recurse){
            Search-ItemsRecursive -projectItems $item.ProjectItems -filter $filter -recurse:$recurse
        }
    }
  }
}

function Get-RelativePath($path, $relativeTo){
    $path.Replace($relativeTo,"").TrimStart("\")
}

function Add-ProjectDirectoryIfNotExist($Project, $DirPath)
{
    $projectPath = Split-Path $Project.FileName -Parent
    $fullPathToNewDire ="$projectPath\$DirPath"
    if((Test-Path $fullPathToNewDire) -ne $true){
        [void](New-Item -ItemType Directory -Force -Path  $fullPathToNewDire)
        $outRoot = ($DirPath -split "\\")[0]
        if([string]::IsNullOrWhiteSpace($outRoot) -ne $true)
        {
            [void]$Project.ProjectItems.AddFromDirectory("$projectPath\$outRoot")
        }
    }
}

function Create-TransformationConfigurationFile($TransformationDetails, $OutPath, [switch]$Force=$true)
{
    [System.XML.XMLDocument]$configDocument = if(($Force -eq $false) -and (Test-Path $OutPath)){
        $existingConf = [xml](Get-Content $OutPath)
        if($existingConf -eq $null)
        {
            New-Object System.XML.XMLDocument
        }else{
            $existingConf
        }
    } else{
        New-Object System.XML.XMLDocument
    } 


    [System.XML.XMLElement]$configRoot = $configDocument.SelectSingleNode("//root")
    if($configRoot -eq $null)
    {
        $configRoot = $configDocument.CreateElement("root")
        [void]$configDocument.appendChild($configRoot)
    }    

    foreach($group in $TransformationDetails.Groups)
    {
        $transformationGroupNode = $configRoot.SelectSingleNode("//transformationGroup[@pattern='$($group.Pattern)']")
        if($transformationGroupNode -eq $null)
        {
            $transformationGroupNode = $configDocument.CreateElement("transformationGroup")
            [void]$configRoot.appendChild($transformationGroupNode)
        }
        [void]$transformationGroupNode.SetAttribute("pattern", $group.Pattern)


        foreach($transformation in $group.Transformations)
        {
            $transformationNode = $transformationGroupNode.SelectSingleNode("//transformation[@values='$($transformation.Values)']")
            if($transformationNode -eq $null)
            {
                $transformationNode = $configDocument.CreateElement("transformation")
                [void]$transformationGroupNode.appendChild($transformationNode)
            }
            
            [void]$transformationNode.SetAttribute("values",$transformation.Values)
            [void]$transformationNode.SetAttribute("output", $transformation.Output)
        }
    }
   
    [void]$configDocument.Save($OutPath)
}

<#
.SYNOPSIS 

Bootstrap config files transformations

.PARAMETER Environments
List of environments for your configs. For each enviroments separate value file will be created.


.PARAMETER ConfigFilter
Define config files extensions for which transformations will be created. If not specied it looks for *.config files


.PARAMETER SearchRecurse
Search for config files in subdirectories


.PARAMETER DefaultEnvironment
The result of transformation for default environment will override original config file.


.PARAMETER TransformationsOut
Output directory for transformations results


.PARAMETER Force
If true, Transformation Configuration Fille will be overrided.
#>
function Add-BoostrapConfig
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string[]]$Environments,
        [string]$ConfigFilter="*.config",
        [switch]$SearchRecurse=$false,
        [string]$DefaultEnvironment,
        [string]$TransformationsOut,
        [switch]$Force
     )
    $project = Get-Project
    $projectPath = Split-Path $project.FileName -Parent
    Add-ProjectDirectoryIfNotExist -Project $project -DirPath $TransformationsOut

    $emptyValuesFileContent= "<?xml version=`"1.0`" encoding=`"utf-8`" ?>`r`n<root>`r`n</root>" 
    $transformationDetails = @{Groups=@()}

    Search-ItemsRecursive -projectItems $project.ProjectItems -filter {param($item) $item.Name -like $ConfigFilter } -recurse:$SearchRecurse |% {        
        $filePath = $_.Properties.Item("FullPath").Value
        if(Get-Confirmation "Do you want to create config transformation for: '$($filePath)'")
        {  
            Write-Verbose "Create Transformation for '$($filePath)'"
            $fileName = Split-Path $filePath -Leaf
            $fileDirectory = Split-Path $filePath -Parent
            $fileExtension = [System.IO.Path]::GetExtension($fileName)
            $filePrefix =$fileName -replace $fileExtension,""
            $patternFilePath = "$fileDirectory\$filePrefix.pattern$($fileExtension)"
            if((Test-Path $patternFilePath) -eq $false)
            {
                Copy-Item -Path $filePath -Destination $patternFilePath
                [void]$_.ProjectItems.AddFromFile($patternFilePath)
            }else{
                Write-Verbose "`tPattern file for '$($filePath)' already exists"
            }
            $transformationGroup = @{Pattern = $(Get-RelativePath $patternFilePath -relativeTo $projectPath); Transformations=@()}
            $transformationDetails.Groups += $transformationGroup

            foreach($environment in $Environments)
            {
              $valuesFilePath = "$fileDirectory\$filePrefix.values.$environment$($fileExtension)"
              if($(Test-Path $valuesFilePath) -eq $false )
              {
                Out-File -FilePath  $valuesFilePath -Encoding utf8 -InputObject $emptyValuesFileContent
                [void]$_.ProjectItems.AddFromFile($valuesFilePath)
                Write-Verbose "`tSuccessfully created file with values for environment: '$environment'"
              }else{
                Write-Verbose "`tFile with values for environment: '$environment' already exists"
              }
              
              $outputPath = if($environment -eq $DefaultEnvironment){ $filePath } else{ "$TransformationsOut\$($filePrefix)_$($environment)$($fileExtension)" }
              $transformationGroup.Transformations+= @{
                Values= $(Get-RelativePath $valuesFilePath -relativeTo $projectPath);
                Output= $(Get-RelativePath $outputPath -relativeTo $projectPath)
              }
            }
        }
    }
   
    $transformationsFilePath = "$projectPath\transformations.xml"
    Create-TransformationConfigurationFile -TransformationDetails $transformationDetails -OutPath $transformationsFilePath -Force:$Force
    [void]$project.ProjectItems.AddFromFile($transformationsFilePath)
    $project.Save()
    Add-TransformationConfig "transformations.xml"
}

Export-ModuleMember -Function Add-Transformation, Add-TransformationConfig, Invoke-Transformations, Add-BoostrapConfig