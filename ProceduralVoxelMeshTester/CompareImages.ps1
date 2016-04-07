$exitCode = 0

Write-Host "Running test in Unity3D"
Start-Process -ArgumentList @("-batchmode") -Wait -NoNewWindow "$env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\Bin\Tester.exe"

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
            $exitCode = 1
		}
		else
		{
			$compareResultString = Invoke-Expression("$env:DEPENDENCIESDIR\ImageMagick\latest\ImageMagick\compare.exe -metric mae $currentScreenshot $originalScreenshot $diffFile  2>&1")
			$compareResult = ([string]$compareResultString).Substring(([string]$compareResultString).IndexOf("(") + 1, ([string]$compareResultString).IndexOf(")") - ([string]$compareResultString).IndexOf("(") - 1);
			if ($compareResult -lt 0.1)
			{
				Write-Host "$currentScreenshot PASS"
			}
			else
			{
				Write-Error "Screenshot did not match. $currentScreenshot did not match the original"
                $exitCode = 1
			}
		}

		Remove-Item $currentScreenshot
		Remove-Item $diffFile
	}
}

exit $exitCode