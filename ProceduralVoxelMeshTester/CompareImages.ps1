Write-Host "Running test in Unity3D Test Program"

# Execute
Start-Process -ArgumentList @("-batchmode"), -Wait -NoNewWindow "$env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTester\Build\Tester.exe"

$testNames = @("ColorVoxelMesh","TextureVoxelMesh")
	
foreach($testName in $testNames)
{
	for($i = 0; $i -lt 6; $i++)
	{
        $currentScreenshot = "{0}\ProceduralVoxelMeshTester\{1}_{2}.png" -f "$env:APPVEYOR_BUILD_FOLDER", "$testName", "$i"
        $originalScreenshot = "{0}\ProceduralVoxelMeshTester\{1}_{2}_original.png" -f "$env:APPVEYOR_BUILD_FOLDER", "$testName", "$i"
        $diffFile = "$env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTester\diff.png"
            
		$testResult = Test-Path $currentScreenshot
		if($testResult -eq $false)
		{
			Add-AppveyorTest "Compare image" -Outcome Failed -FileName "ProceduralVoxelMesh/ProceduralVoxelMeshTester/CompareImages.ps1" -ErrorMessage "Screenshot not found. $currentScreenshot file not found" 
            		$host.SetShouldExit(1)
		}
		else
		{
			$compareResultString = Invoke-Expression("$env:DEPENDENCIES_DIR\ImageMagick\compare.exe -metric mae $currentScreenshot $originalScreenshot $diffFile  2>&1")
			$compareResult = ([string]$compareResultString).Substring(([string]$compareResultString).IndexOf("(") + 1, ([string]$compareResultString).IndexOf(")") - ([string]$compareResultString).IndexOf("(") - 1);
			if ($compareResult -gt 0.1)
			{
				Add-AppveyorTest "Compare image" -Outcome Failed -FileName "ProceduralVoxelMesh/ProceduralVoxelMeshTester/CompareImages.ps1" -ErrorMessage "Screenshot did not match. $currentScreenshot did not match the original"
                		$host.SetShouldExit(1)
			}
		}

		Remove-Item $currentScreenshot
		Remove-Item $diffFile
	}
}

Add-AppveyorTest "Compare image" -Outcome Passed -FileName "ProceduralVoxelMesh/ProceduralVoxelMeshTester/CompareImages.ps1"
