Write-Host "Copying dlls"

Copy-Item -Force "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.dll" "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\Assets\Plugins\"

$testResult = Test-Path "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\Assets\Plugins\ProceduralVoxelMesh.dll"

if($testResult -eq $false)
{
	Write-Error "Library not found. ProceduralVoxelMesh.dll file not found in the $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\Assets\ folder"
}
else
{
    Write-Host "Running test in Unity3D"
    Start-Process -ArgumentList @("-batchmode","-projectpath $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\", "-executeMethod Assets.Test.StartTest") -Wait -NoNewWindow "C:\agent\dependencies\Unity\latest\Unity\Editor\Unity.exe"

	$testNames = @("ColorVoxelMesh","TextureVoxelMesh")
	
	foreach($testName in $testNames)
	{
		for($i = 0; $i -lt 6; $i++)
		{
            $currentScreenshot = "{0}\ProceduralVoxelMeshTester\{1}_{2}.png" -f "$env:BUILD_SOURCESDIRECTORY", "$testName", "$i"
            $originalScreenshot = "{0}\ProceduralVoxelMeshTester\{1}_{2}_original.png" -f "$env:BUILD_SOURCESDIRECTORY", "$testName", "$i"
            $diffFile = "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\diff.png"
            
			$testResult = Test-Path $currentScreenshot
			if($testResult -eq $false)
			{
				Write-Error "Screenshot not found. $currentScreenshot file not found"
			}
			else
			{
				$compareResultString = Invoke-Expression("C:\agent\dependencies\ImageMagick\latest\compare.exe -metric mae $currentScreenshot $originalScreenshot $diffFile  2>&1")
				$compareResult = ([string]$compareResultString).Substring(([string]$compareResultString).IndexOf("(") + 1, ([string]$compareResultString).IndexOf(")") - ([string]$compareResultString).IndexOf("(") - 1);
				if ($compareResult -lt 0.1)
				{
					Write-Host "$currentScreenshot PASS"
				}
				else
				{
					Write-Error "Screenshot did not match. $currentScreenshot did not match the original"
				}
			}

			Remove-Item $currentScreenshot
			Remove-Item $diffFile
		}
	}
}

exit 0
