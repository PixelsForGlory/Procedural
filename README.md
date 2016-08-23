# ProceduralVoxelMesh
Library to create procedural voxel meshes in Unity3D.  Meshes are generated in a seprate thread and copied to the GameObject when ready.

Build status:<br />
[![Build status](https://ci.appveyor.com/api/projects/status/2lsxqcv6dcc5vve1/branch/master?svg=true)](https://ci.appveyor.com/project/LlamaBot/proceduralvoxelmesh/branch/master)

## Building
The solution has references set for `UnityEngine.dll` and `UnityEditor.dll`, but the paths for these assemblies are not set. To build the library, a reference path must be set to the Managed directory (Default is C:\Program Files\Unity\Editor\Data\Managed):
![Reference Path](../../../Screenshots/blob/master/VoxelMeshReferencePath.png?raw=true "Reference Path")

## Installation
From a build or downloaded released, copy the `ProceduralVoxelMesh.dll` to `[PROJECT DIR]\Assets\Plugins` and `ProceduralVoxelMeshEditor.dll` to `[PROJECT DIR]\Assets\Plugins\Editor`.  Additionally, move the shader files and the material files found in the Resources directory to a Resources directory somewhere in your `[PROJECT DIR]\Assets` directory (example `[PROJECT DIR]\Assets\Resources`) so it can be found by `Resources.Load()`.

If using the Pixels for Glory NuGet repository at http://pixelsforglory.azurewebsites.net/nuget, install the `PixelsForGlory.Unity3D.ProceduralVoxelMesh` package into a Unity3D project.

After copying in the assets, a few things to setup:

1. The thread to generate meshes will be started automatically in the editor, but to have it startup automatically in a standalone game, make sure there is a GameObject with the thread script attached to it.  There is logic to make sure that only one instance of the thread is setup.

![Voxel Generator Thread](../../../Screenshots/blob/master/VoxelThreadSetup.png?raw=true)

2. Make sure the "ColorVoxelMaterial" has the "AlphaMap" texture set for the "Metallic Map", "Smoothness Map", and "Emission Map".

3. Make sure that the AlphaMap.png has the following texture import settings:
![AlphaMap Import Settings](../../../Screenshots/blob/master/AlphaMapImport.png?raw=true "AlphaMap Import Settings")

4. If you want to use the TextureVoxelMesh, use similar settings as the following.  Not using mipmaps and filtering as point will make a really small texture render sharply.

![Texture Import Settings](../../../Screenshots/blob/master/TextureVoxelSetup.png?raw=true "Texture Import Settings")

## Usage

### Resource setup

If the material is not setup in the root directory of the Resource folder, then the resource folder must be set in each `VoxelMesh` type.  For example, if installing using the NuGet package, make sure the following is set:

```
ColorVoxelMesh.MaterialResourcePath = @"PixelsForGlory\ProceduralVoxelMesh\";
ColorVoxelTransparentMesh.MaterialResourcePath = @"PixelsForGlory\ProceduralVoxelMesh\";
TextureVoxelMesh.MaterialResourcePath = @"PixelsForGlory\ProceduralVoxelMesh\";
TextureVoxelTransparentMesh.MaterialResourcePath = @"PixelsForGlory\ProceduralVoxelMesh\";
```

### Mesh Size

The mesh will always produce a 1 x 1 x 1 scale version of the mesh.  It can be scaled up and down as needed with the Unity3D scale.

### Color Voxel Mesh

The following example will create a randomly colored cube on a `GameObject` with a `ColorVoxelMesh` component:

```
using UnityEngine;
using PixelsForGlory.ProceduralVoxelMesh;

public class ExampleCube : MonoBehaviour 
{

  void Start ()
  {
    ColorVoxelMesh voxelMesh = GetComponent<ColorVoxelMesh>();

    int width = 16;
    int height = 16;
    int depth = 16;
    
    
    var voxels = new List<ColorVoxel>(new ColorVoxel[width * height * depth]);
    for(int w = 0; w < width; ++w)
    {
      for(int h = 0; h < height; ++h)
      {
        for(int d = 0; d < depth; ++d)
        {
          voxels[Utilities.GetIndex(w, h, d, width, height, depth)] = new ColorVoxel(new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        }
      }
    }

    var voxelData = new ColorVoxelMeshData("Test", width, height, depth, voxels);
    voxelMesh.SetVoxelData(voxelData);
  }
}
```

### Texture Voxel Mesh

The following example will create a randomly colored cube on a `GameObject` with a `TextureVoxelMesh` component:

```
public class ExampleTextureCube : MonoBehaviour
{
    void Start()
    {
        TextureVoxel.TextureVoxelMap.Clear();
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
    
    
        TextureVoxelMesh voxelMesh = GetComponent<TextureVoxelMesh>();
    
        int width = 2;
        int height = 2;
        int depth = 2;

        var voxels = new List<TextureVoxel>(new TextureVoxel[width * height * depth]);
        for(int w = 0; w < width; ++w)
        {
            for(int h = 0; h < height; ++h)
            {
                for(int d = 0; d < depth; ++d)
                {
                    if(w == d && h == d)
                    {
                        voxels[Utilities.GetIndex(w, h, d, width, height, depth)] = new TextureVoxel(0, 1);
                    }
                    else
                    {
                        voxels[Utilities.GetIndex(w, h, d, width, height, depth)] = new TextureVoxel(0);
                    }

                }
            }
        }
        
        var voxelData = new TextureVoxelMeshData("Test", width, height, depth, voxels);
        voxelMesh.SetVoxelData(voxelData);
    }
}
```

Notes on the setup:

 - The texture map, like the test map provided in this repository, is setup through a mapping configuration in the static `TextureVoxelMap` list
 - Index 0 of the texture map should be empty. For example, the test texture map has 32x32 squares to map.  The first 32x32 is completely zeroed out (`float4(0, 0, 0, 0)`) 
 - The texture map needs to save the alpha channel to work properly.

### Additional settings

#### Alpha Channel
If you want the mesh to use the material that uses the alpha channel, call `mesh.UseAlphaChannel(true)`

#### Faces to render
In the `ColorVoxel` and `TextureVoxel` constructors there is a bit flag variable `facesToRender`.  Set this to face types that should render.  For example `FaceType.YPositive | FaceType.YNegative` will cause the mesh to only render the positive and negative y faces of the voxel.  This is useful for rendering transparent meshes next to each other.

Level of detail can be set on the voxel mesh.  It will reduce the amount of voxels in the mesh by 2^LOD.

## Testing
The `ProceduralVoxelMeshTest` folder contains a unit test project to be run against the code base in VSTtest.

The `ProceduralVoxelMeshTester` folder contains a Unity3D project that can be built with a build release of the ProceduralVoxelMesh.dll. The program should produce the same screenshots as previous runs of that program.

Color Voxel Mesh will look something like this:

![Color Cube](../../../Screenshots/blob/master/ColorCube.png?raw=true "Color Cube")

Texture Voxel Mesh will look something like this:

![Texture Cube](../../../Screenshots/blob/master/TextureCube.png?raw=true "Texture Cube")

Enjoy!  

If any issues or bugs are found, please let me know [@afuzzyllama](https://twitter.com/afuzzyllama)



