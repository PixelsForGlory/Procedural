$exitCode = 0

$outputfile = "$env:COMMON_TESTRESULTSDIRECTORY\CompareImages.coverage"
$vsPath = [Microsoft.Win32.Registry]::GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\14.0\", "ShellFolder", $null);

Start-Process -FilePath "$vsPath\Team Tools\Performance Tools\vsperfmon.exe" -ArgumentList "-coverage -output:$outputFile" -NoNewWindow
Write-Host "Waiting 5s for vsperfmon to start up..."
Start-Sleep -s 5

$counter = 0
while($counter -lt 6)
{
    $e = & "$vsPath\Team Tools\Performance Tools\vsperfcmd.exe" /status

    if ($LASTEXITCODE -eq 0){
        Write-Host $e
        break       
    }

    if ($LASTEXITCODE -eq 1 -and $counter -lt 5){
        Write-Host "Still waiting. Give it 5s more ($counter)..."
        Start-Sleep -s 5
        $counter++
        continue
    }

    throw "vsperfmon Error: $e"                 
}

Write-Host "Running test in Unity3D"
Start-Process -ArgumentList @("-batchmode","-projectpath $env:BUILD_SOURCESDIRECTORY\ProceduralVoxelMeshTester\", "-executeMethod Assets.Test.StartTest") -Wait -NoNewWindow "$env:AGENT_ROOTDIRECTORY\dependencies\Unity\latest\Unity\Editor\Unity.exe"

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
			$compareResultString = Invoke-Expression("$env:AGENT_ROOTDIRECTORY\dependencies\ImageMagick\latest\ImageMagick\compare.exe -metric mae $currentScreenshot $originalScreenshot $diffFile  2>&1")
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

& "$vsPath\Team Tools\Performance Tools\vsperfcmd.exe" -shutdown

exit $exitCode
