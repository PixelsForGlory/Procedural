Write-Host "Comparing screenshots"

Copy-Item -Force "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.dll" "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\Assets\Plugins\"

$testResult = Test-Path "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\Assets\Plugins\ProceduralVoxelMesh.dll"

if($testResult -eq $false)
{
	Write-Error "Library not found. ProceduralVoxelMesh.dll file not found in the $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\Assets\ folder"
}
else
{
    Start-Process -ArgumentList @("-batchmode","-projectpath $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\", "-executeMethod Assets.Test.StartTest") -Wait -NoNewWindow "C:\agent\dependencies\Unity\latest\Unity\Editor\Unity.exe"

	$testNames = "ColorVoxelMesh","TextureVoxelMesh"
	
	foreach($testName in $testNames)
	{
		for($i = 0; $i -lt 6; $i++)
		{
			$testResult = Test-Path "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\$testName_$i.png"
			if($testResult -eq $false)
			{
				Write-Error "Screenshot not found. $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\$testName_$i.png file not found"
			}
			else
			{
				$compareResultString = Invoke-Expression("compare.exe -metric mae $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\$testName_$i.png $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\$testName_$i_original.png $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\diff.png 2>&1")
				$compareResult = ([string]$compareResultString).Substring(([string]$compareResultString).IndexOf("(") + 1, ([string]$compareResultString).IndexOf(")") - ([string]$compareResultString).IndexOf("(") - 1);
				if ($compareResult -lt 0.1)
				{
					Write-Host "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\$testName_$i.png PASS"
				}
				else
				{
					Write-Error "Screenshot did not match. $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\$testName_$i.png did not match the original"
				}
			}

			Remove-Item "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\$testName_$i.png"
			Remove-Item "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\diff.png"
		}
	}
}
