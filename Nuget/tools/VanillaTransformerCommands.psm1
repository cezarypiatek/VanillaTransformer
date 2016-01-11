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

function Invoke-Transformations
{
    [CmdletBinding()]
    param($configFilePath)
    Load-VanillaTransformerLib
    
    $oldPath = Get-Location
    $projectPath = (Get-Project | Select -ExpandProperty FullName |Split-Path -Parent)      
    [System.IO.Directory]::SetCurrentDirectory($projectPath)    
        
    if(-not([System.String]::IsNullOrWhiteSpace($configFilePath)))
    {
        $transformationTask = New-Object VanillaTransformer.VanillaTransformerTask
        $transformationTask.TransformConfiguration = $configFilePath        
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

Export-ModuleMember -Function Add-Transformation, Add-TransformationConfig, Invoke-Transformations