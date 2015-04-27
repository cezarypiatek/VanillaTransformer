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