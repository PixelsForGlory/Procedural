# Procedural
Library to create procedural meshes in Unity3D.  Meshes are generated in a seprate thread and copied to the GameObject when ready.

## Installation
Requires [PixelsForGlory.Base](https://github.com/PixelsForGlory/Base) and [PixelsForGlory.Extensions](https://github.com/PixelsForGlory/Extensions)
Requires Player to be using .NET 4.6+

Add as a submodule to your Unity3D project directory:

`git submodule add git@github.com:PixelsForGlory/Procedural.git ${ProjectRoot}/Assets/Plugins/PixelsForGlory/Procedural`

1. Make sure the "ColorVoxelMaterial" has the "AlphaMap" texture set for the "Metallic Map", "Smoothness Map", and "Emission Map".

1. Make sure that the AlphaMap.png has the following texture import settings:
![AlphaMap Import Settings](../../../Screenshots/blob/master/AlphaMapImport.png?raw=true "AlphaMap Import Settings")

1. If you want to use the TextureVoxelMesh, use similar settings as the following.  Not using mipmaps and filtering as point will make a really small texture render sharply.

![Texture Import Settings](../../../Screenshots/blob/master/TextureVoxelSetup.png?raw=true "Texture Import Settings")

## Usage

### Mesh Size

The mesh will always produce a 1 x 1 x 1 scale version of the mesh.  It can be scaled up and down as needed with the Unity3D scale.

### Color Voxel Mesh

The following example will create a randomly colored cube on a `GameObject`:

```
using System.Threading.Tasks;
using UnityEngine;

using PixelsForGlory;
using PixelsForGlory.Collections;
using PixelsForGlory.Procedural;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class VoxelTest : MonoBehaviour
{
    private VoxelMeshData _data;
    private VoxelMeshTask _meshTask;
    private Task _task;

    public void Start()
    {
        var dimensions = new Dimensions3D(32, 32, 32);
        var voxels = new List3D<IVoxel>(dimensions);

        for (int x = 0; x < dimensions.X; x++)
        {
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int z = 0; z < dimensions.Z; z++)
                {
                    voxels[x, y, z] = new ColorVoxel(new Color(Random.value, Random.value, Random.value));
                }
            }
        }

        _data = new VoxelMeshData("test", voxels);

        _meshTask = new VoxelMeshTask(_data);
        _task = Task.Run(() => _meshTask.RunTask());
    }
    public void Update()
    {
        if (_task != null && _task.Status == TaskStatus.RanToCompletion)
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();

            var mesh = new Mesh()
            {
                vertices = _meshTask.Vertices,
                normals = _meshTask.Normals,
                colors = _meshTask.Colors,
                uv = _meshTask.UV,
                uv2 = _meshTask.UV2,
                uv3 = _meshTask.UV3,
                triangles = _meshTask.Triangles
            };
            meshFilter.sharedMesh = mesh;

            _task = null;
        }
    }
}
```

### Texture Voxel Mesh

The following example will create a texted cube on a `GameObject`:

```
using System.Threading.Tasks;
using UnityEngine;

using PixelsForGlory;
using PixelsForGlory.Collections;
using PixelsForGlory.Procedural;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class VoxelTest : MonoBehaviour
{
    private VoxelMeshData _data;
    private VoxelMeshTask _meshTask;
    private Task _task;

    public void Start()
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

        var dimensions = new Dimensions3D(2, 2, 2);
        var voxels = new List3D<IVoxel>(dimensions);

        for (int x = 0; x < dimensions.X; x++)
        {
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int z = 0; z < dimensions.Z; z++)
                {
                    if (x == z && y == z)
                    {
                        voxels[x, y, z] = new TextureVoxel(0, 1);
                    }
                    else
                    {
                        voxels[x, y, z] = new TextureVoxel(0);
                    }
                }
            }
        }

        _data = new VoxelMeshData("test", voxels);

        _meshTask = new VoxelMeshTask(_data);
        _task = Task.Run(() => _meshTask.RunTask());
    }

    public void Update()
    {
        if (_task != null && _task.Status == TaskStatus.RanToCompletion)
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();

            var mesh = new Mesh()
            {
                vertices = _meshTask.Vertices,
                normals = _meshTask.Normals,
                colors = _meshTask.Colors,
                uv = _meshTask.UV,
                uv2 = _meshTask.UV2,
                uv3 = _meshTask.UV3,
                triangles = _meshTask.Triangles
            };
            meshFilter.sharedMesh = mesh;

            _task = null;
        }
    }
}
```

### Notes `TextureVoxel` the setup:

 - The texture map, like the test map provided in this repository, is setup through a mapping configuration in the static `TextureVoxelMap` list
 - Index 0 of the texture map should be empty. For example, the test texture map has 32x32 squares to map.  The first 32x32 is completely zeroed out (`float4(0, 0, 0, 0)`) 
 - The texture map needs to save the alpha channel to work properly.

### Examples

Color Voxel Mesh will look something like this:

![Color Cube](../../../Screenshots/blob/master/ColorCube.png?raw=true "Color Cube")

Texture Voxel Mesh will look something like this:

![Texture Cube](../../../Screenshots/blob/master/TextureCube.png?raw=true "Texture Cube")

Enjoy!  

If any issues or bugs are found, please let me know [@afuzzyllama](https://twitter.com/afuzzyllama)



