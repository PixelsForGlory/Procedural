ECHO ##teamcity[testStarted name='RenderTest']

copy /Y %system.teamcity.build.workingDir%\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.dll %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\Assets\Plugins\

IF NOT EXIST %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\Assets\Plugins\ProceduralVoxelMesh.dll (
	ECHO ##teamcity[testFailed name='RenderTest' message='Library not found' details='ProceduralVoxelMesh.dll file not found in the %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\Assets\ folder']
	ECHO ##teamcity[testFinished name='RenderTest']
	EXIT
)
"C:\Program Files\Unity\Editor\Unity.exe" -batchmode -projectpath %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\ -executeMethod Assets.Test.StartTest


IF NOT EXIST %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\0.png (
	ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot not found' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\0.png file not found']
) ELSE (
	FC /B %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\0.png %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\original0.png > NUL && ECHO %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\0.png PASS || ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot did not match' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\0.png did not match the original']
)

IF NOT EXIST %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\1.png (
	ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot not found' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\1.png file not found']
) ELSE (
	FC /B %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\1.png %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\original1.png > NUL && ECHO %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\1.png PASS || ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot did not match' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\1.png did not match the original']
)

IF NOT EXIST %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\2.png (
	ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot not found' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\2.png file not found']
) ELSE (
	FC /B %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\2.png %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\original2.png > NUL && ECHO %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\2.png PASS || ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot did not match' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\2.png did not match the original']
)

IF NOT EXIST %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\3.png (
	ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot not found' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\3.png file not found']
) ELSE (
	FC /B %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\3.png %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\original3.png > NUL && ECHO %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\3.png PASS || ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot did not match' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\3.png did not match the original']
)

IF NOT EXIST %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\4.png (
	ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot not found' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\4.png file not found']
) ELSE (
	FC /B %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\4.png %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\original4.png > NUL && ECHO %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\4.png PASS || ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot did not match' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\4.png did not match the original']
)

IF NOT EXIST %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\5.png (
	ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot not found' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\5.png file not found']
) ELSE (
	FC /B %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\5.png %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\original5.png > NUL && ECHO %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\5.png PASS || ECHO ##teamcity[testFailed name='RenderTest' message='Screenshot did not match' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\5.png did not match the original']
)

DEL %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\0.png
DEL %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\1.png 
DEL %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\2.png 
DEL %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\3.png 
DEL %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\4.png 
DEL %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\5.png 


ECHO ##teamcity[testFinished name='RenderTest']
