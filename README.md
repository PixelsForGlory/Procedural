# ProceduralVoxelMesh
Library to create procedural voxel meshes in Unity3D.  Meshes are generated in a seprate thread and copied to the GameObject when ready.

Build status:<br />
![Build status](../../..//Screenshots/blob/master/ProceduralVoxelMeshStatus.png?raw=true "Build status")

## Building
The solution has references set for `UnityEngine.dll` and `UnityEditor.dll`, but the paths for these assemblies are not set. To build the library, a reference path must be set to the Managed directory (Default is C:\Program Files\Unity\Editor\Data\Managed):
![Reference Path](../../../Screenshots/blob/master/VoxelMeshReferencePath.png?raw=true "Reference Path")

When working in the editor, use the `DebugUnityEditor` build configuration.  This will use editor specific code to get the mesh generator thread running when the editor opens.  Otherwise, use Debug/Release for running the stand alone game.

## Installation
When the build is complete, move the `ProceduralVoxelMesh.dll` to `[PROJECT DIR]\Assets\Plugins`.  Additionally, move the shader file and the material file found in the Resources directory to a Resources directory somewhere in your `[PROJECT DIR]\Assets` directory (example `[PROJECT DIR]\Assets\Resources) so it can be found by `Resources.Load()`.

After copying in the assets, make sure that the ColorMap.png has the following texture import settings:
![AlphaMap Import Settings](../../../Screenshots/blob/master/AlphaMapImport.png?raw=true "AlphaMap Import Settings")

If the import settings aren't correct, the colors might not render correctly.

## Usage
There are two types of voxel meshes that can be created.  A color voxel mesh and a texture voxel mesh.

Creating a color voxel mesh at runtime is simple.  The following example will create a randomly colored cube on a GameObject with a ColorVoxelMesh component:

```
using UnityEngine;
using ProceduralVoxelMesh;

public class ExampleCube : MonoBehaviour 
{

  void Start ()
  {
    ColorVoxelMesh voxelMesh = GetComponent<ColorVoxelMesh>();

    ColorVoxel[,,] voxels = new ColorVoxel[16, 16, 16];
    for(int w = 0; w < 16; ++w)
    {
      for(int h = 0; h < 16; ++h)
      {
        for(int d = 0; d < 16; ++d)
        {
          voxels[w, h, d] = new ColorVoxel(new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        }
      }
    }

    voxelMesh.SetVoxels(voxels);
  }
}
```

When working with a texture map, like the test map provided in this repo, you have to setup a mapping configuration first in the static `TextureVoxelMap` list:

```
TextureVoxel.TextureVoxelMap.Add(
	new TextureVoxelSetup()
	{
		XNegativeTextureIndex = 1,
		XPositiveTextureIndex = 1,
		YNegativeTextureIndex = 3,
		YPositiveTextureIndex = 2,
		ZNegativeTextureIndex = 1,
		ZPositiveTextureIndex = 1
	});

TextureVoxel.TextureVoxelMap.Add(
	new TextureVoxelSetup()
	{
		XNegativeTextureIndex = 4,
		XPositiveTextureIndex = 4,
		YNegativeTextureIndex = 4,
		YPositiveTextureIndex = 4,
		ZNegativeTextureIndex = 4,
		ZPositiveTextureIndex = 4
	});
```

then you can setup a TextureVoxelMesh as follows:

```
public class ExampleTextureCube : MonoBehaviour
{
	void Start()
	{
		TextureVoxelMesh voxelMesh = GetComponent<TextureVoxelMesh>();
	
		int width = 2;
		int height = 2;
		int depth = 2;

		TextureVoxel[,,] voxels = new TextureVoxel[width, height, depth];
		for(int w = 0; w < width; ++w)
		{
			for(int h = 0; h < height; ++h)
			{
				for(int d = 0; d < depth; ++d)
				{
					if(w == d && h == d)
					{
						voxels[w, h, d] = new TextureVoxel(0, 1);
					}
					else
					{
						voxels[w, h, d] = new TextureVoxel(0);
					}

				}
			}
		}
		voxelMesh.SetVoxels(voxels);
	}
}
```

## Testing
The `ProceduralVoxelMeshTest` folder contains a unit test project to be run against the code base in NUnit.


The `ProceduralVoxelMeshTester` folder contains a Unity3D project that can be built with a build release of the ProceduralVoxelMesh.dll. The program should produce the same screenshots as previous runs of that program.

Something like this should be the result:
![Random Cube](../../../Screenshots/blob/master/VoxelMeshEditor.png?raw=true "Random Cube")

Enjoy!  

If any issues or bugs are found, please let me know [@afuzzyllama](https://twitter.com/afuzzyllama)



