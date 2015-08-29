# ProceduralVoxelMesh
Library to create procedural voxel meshes in Unity3D.  Meshes are generated in a seprate thread and copied to the GameObject when ready.

## Building
The solution has references set for `UnityEngine.dll` and `UnityEditor.dll`, but the paths for these assemblies are not set. To build the library, a reference path must be set to the Managed directory (Default is C:\Program Files\Unity\Editor\Data\Managed):
![Reference Path](../../../Screenshots/blob/master/VoxelMeshReferencePath.png?raw=true "Reference Path")

When working in the editor, use the `DebugUnityEditor` build configuration.  This will use editor specific code to get the mesh generator thread running when the editor opens.

## Installation
When the build is complete, move the `ProceduralVoxelMesh.dll` to `[PROJECT DIR]\Assets\Plugins`.  This should trigger a recomple. 

## Usage
Creating a voxel mesh at runtime is simple.  The following example will create a randomly colored cube on a GameObject with a VoxelMesh component:

```
using UnityEngine;
using ProceduralVoxelMesh;

public class ExampleCube : MonoBehaviour {

  void Start ()
  {
    VoxelMesh voxelMesh = GetComponent<VoxelMesh>();

    Voxel[,,] voxels = new Voxel[16, 16, 16];
    for(int w = 0; w < 16; ++w)
    {
      for(int h = 0; h < 16; ++h)
      {
        for(int d = 0; d < 16; ++d)
        {
          voxels[w, h, d] = new Voxel(false,
            new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        }
      }
    }

    voxelMesh.SetVoxels(voxels);
  }
}
```

Something like this should be the result:
![Random Cube](../../../Screenshots/blob/master/VoxelMeshEditor.png?raw=true "Random Cube")

Enjoy!  

If any issues or bugs are found, please let me know [@afuzzyllama](https://twitter.com/afuzzyllama)



