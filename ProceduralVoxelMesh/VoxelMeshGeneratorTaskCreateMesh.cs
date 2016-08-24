// Copyright 2012 Mikola Lysenko
// Implementation based off of blog post and javascript code written by Mikola Lysenko
// Reference: 
//  Blog post:  http://0fps.net/2012/06/30/meshing-in-a-minecraft-game/
//  Github:     https://github.com/mikolalysenko/mikolalysenko.github.com/tree/gh-pages/MinecraftMeshes
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    /// <summary>
    /// Faces of a cube
    /// </summary>
    [Flags]
    public enum FaceType
    {
        None        = 0,
        XPositive   = 1,
        XNegative   = 2,
        YPositive   = 4,
        YNegative   = 8,
        ZPositive   = 16,
        ZNegative   = 32
    }

    public partial class VoxelMeshGeneratorTask<T> : IVoxelMeshGeneratorTask where T : IVoxel<T>
    {

        /// <summary>
        /// Create the mesh from the current voxel data
        /// </summary>
        public void CreateMesh()
        {
            try
            {
                var vertices = new List<Vector3>();
                var normals = new List<Vector3>();
                var colors = new List<Color>();
                var uv = new List<Vector2>();
                var uv2 = new List<Vector2>();
                var uv3 = new List<Vector2>();
                var triangles = new List<int>();

                Vector3 offset = 
                    new Vector3(
                        _width  * _voxelScaleFactor.x, 
                        _height * _voxelScaleFactor.y, 
                        _depth  * _voxelScaleFactor.z) * _levelOfDetailDivisor / 2.0f;

                // GreedyMesh (volume, dims)
                // Sweep over 3-axes
                // dimension = 0 -> y, z
                // dimension = 1 -> z, x
                // dimension = 2 -> x, y
                for(int dimension = 0; dimension < 3; dimension++)
                {
                    // 0 -> x, 1 -> y, 2 -> z
                    int u = (dimension + 1) % 3;
                    int v = (dimension + 2) % 3;

                    int dimensionU = 0;
                    int dimensionV = 0;
                    int dimensionMax = 0;

                    switch(dimension)
                    {
                        case 0:
                            dimensionMax = _width / _levelOfDetailDivisor;
                            break;
                        case 1:
                            dimensionMax = _height / _levelOfDetailDivisor;
                            break;
                        case 2:
                            dimensionMax = _depth / _levelOfDetailDivisor;
                            break;
                    }

                    switch(u)
                    {
                        case 0:
                            dimensionU = _width / _levelOfDetailDivisor;
                            break;
                        case 1:
                            dimensionU = _height / _levelOfDetailDivisor;
                            break;
                        case 2:
                            dimensionU = _depth / _levelOfDetailDivisor;
                            break;
                    }

                    switch(v)
                    {
                        case 0:
                            dimensionV = _width / _levelOfDetailDivisor;
                            break;
                        case 1:
                            dimensionV = _height / _levelOfDetailDivisor;
                            break;
                        case 2:
                            dimensionV = _depth / _levelOfDetailDivisor;
                            break;
                    }

                    int maskSize = dimensionU * dimensionV;

                    FaceMask<T>[] mask = new FaceMask<T>[maskSize];

                    int[] x = { 0, 0, 0 };
                    int[] q = { 0, 0, 0 };

                    q[dimension] = 1;
                    x[dimension] = -1;

                    // All normal directions start in the negative direction
                    int[,] normalDirections = new int[dimensionU, dimensionV];

                    for(int normalU = 0; normalU < dimensionU; normalU++)
                    {
                        for(int normalV = 0; normalV < dimensionV; normalV++)
                        {
                            normalDirections[normalU, normalV] = -1;
                        }
                    }

                    while(x[dimension] < dimensionMax)
                    {
                        // Compute mask
                        int maskIndex = 0;
                        for(x[v] = 0; x[v] < dimensionV; x[v]++)
                        {
                            for(x[u] = 0; x[u] < dimensionU; x[u]++)
                            {
                                bool first;
                                if(0 > x[dimension] || _voxels[Utilities.GetIndex(x[0] * _levelOfDetailDivisor, x[1] * _levelOfDetailDivisor, x[2] * _levelOfDetailDivisor, _width, _height, _depth)].Empty)
                                {
                                    first = false;
                                }
                                else
                                {
                                    first = true;
                                }

                                bool second;
                                if(x[dimension] >= dimensionMax - 1 || _voxels[Utilities.GetIndex((x[0] + q[0]) * _levelOfDetailDivisor, (x[1] + q[1]) * _levelOfDetailDivisor, (x[2] + q[2]) * _levelOfDetailDivisor, _width, _height, _depth)].Empty)
                                {
                                    second = false;
                                }
                                else
                                {
                                    second = true;
                                }

                                if(first != second)
                                {
                                    mask[maskIndex].HasFace = true;
                                    if(first)
                                    {
                                        mask[maskIndex].Voxel = _voxels[Utilities.GetIndex(x[0] * _levelOfDetailDivisor, x[1] * _levelOfDetailDivisor, x[2] * _levelOfDetailDivisor, _width, _height, _depth)];
                                        mask[maskIndex].FirstOrSecond = 1;
                                    }
                                    else
                                    {
                                        mask[maskIndex].Voxel = _voxels[Utilities.GetIndex((x[0] + q[0]) * _levelOfDetailDivisor, (x[1] + q[1]) * _levelOfDetailDivisor, (x[2] + q[2]) * _levelOfDetailDivisor, _width, _height, _depth)];
                                        mask[maskIndex].FirstOrSecond = 2;
                                    }
                                }
                                else
                                {
                                    mask[maskIndex].HasFace = false;
                                }

                                ++maskIndex;
                            }
                        }

                        //Increment x[dimension]
                        x[dimension]++;

                        //Generate mesh for mask using lexicographic ordering
                        int n = 0;
                        for(int j = 0; j < dimensionV; j++)
                        {
                            int i = 0;
                            while(i < dimensionU)
                            {
                                if(mask[n].HasFace)
                                {
                                    //Compute width
                                    int w = 1;
                                    if(n + w < mask.Length)
                                    {
                                        while(
                                            mask[n].HasFace == mask[n + w].HasFace
                                            && i + w < dimensionU && mask[n].FirstOrSecond == mask[n + w].FirstOrSecond
                                            && mask[n].Voxel.Equals(mask[n + w].Voxel)
                                        )
                                        {
                                            w++;

                                            if(n + w >= mask.Length)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    //Compute height (this is slightly awkward)
                                    bool done = false;
                                    int h;
                                    for(h = 1; j + h < dimensionV; h++)
                                    {
                                        for(int k = 0; k < w; k++)
                                        {
                                            if(
                                                mask[n].HasFace != mask[n + k + h * dimensionU].HasFace
                                                || mask[n].FirstOrSecond != mask[n + k + h * dimensionU].FirstOrSecond
                                                || !mask[n].Voxel.Equals(mask[n + k + h * dimensionU].Voxel)
                                            )
                                            {
                                                done = true;
                                                break;
                                            }
                                        }

                                        if(done)
                                        {
                                            break;
                                        }
                                    }

                                    //Add quad

                                    // Get the starting point
                                    x[u] = i;
                                    x[v] = j;

                                    // Get the change in the dimensions for end point
                                    int[] du = { 0, 0, 0 };
                                    du[u] = w;
                                    int[] dv = { 0, 0, 0 };
                                    dv[v] = h;

                                    // Get mask direction
                                    int maskNormalDirection = normalDirections[i, j];

                                    // +/- represents normal direction
                                    FaceType faceType =  FaceType.None;
                                    if(u == 1 && v == 2) // X
                                    {
                                        faceType = maskNormalDirection > 0 ? FaceType.XPositive : FaceType.XNegative;
                                    }
                                    else if(u == 2 && v == 0) // Y
                                    {
                                        faceType = maskNormalDirection > 0 ? FaceType.YPositive : FaceType.YNegative;
                                    }
                                    else if(u == 0 && v == 1) // Z
                                    {
                                        faceType = maskNormalDirection > 0 ? FaceType.ZPositive : FaceType.ZNegative;
                                    }

                                    int baseVerticesNum = vertices.Count;

                                    // If the face is rendered, add it to the triangle list, otherwise skip it
                                    if(mask[n].Voxel.AddVoxelToMesh(faceType, w, h, colors, uv, uv2, uv3))
                                    {
                                        // Vertices
                                        vertices.Add(new Vector3((x[0]                ) * _voxelScaleFactor.x, (x[1]                ) * _voxelScaleFactor.y, (x[2]                ) * _voxelScaleFactor.z) * _levelOfDetailDivisor - offset); // 0
                                        vertices.Add(new Vector3((x[0] + du[0]        ) * _voxelScaleFactor.x, (x[1] + du[1]        ) * _voxelScaleFactor.y, (x[2] + du[2]        ) * _voxelScaleFactor.z) * _levelOfDetailDivisor - offset); // 1
                                        vertices.Add(new Vector3((x[0] + du[0] + dv[0]) * _voxelScaleFactor.x, (x[1] + du[1] + dv[1]) * _voxelScaleFactor.y, (x[2] + du[2] + dv[2]) * _voxelScaleFactor.z) * _levelOfDetailDivisor - offset); // 2
                                        vertices.Add(new Vector3((x[0] + dv[0]        ) * _voxelScaleFactor.x, (x[1] + dv[1]        ) * _voxelScaleFactor.y, (x[2] + dv[2]        ) * _voxelScaleFactor.z) * _levelOfDetailDivisor - offset); // 3

                                        // Normals
                                        switch(faceType)
                                        {
                                            case FaceType.XPositive:
                                            case FaceType.XNegative:
                                                normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                                normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                                normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                                normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                                break;
                                            case FaceType.YPositive:
                                            case FaceType.YNegative:
                                                normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                                normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                                normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                                normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                                break;
                                            case FaceType.ZPositive:
                                            case FaceType.ZNegative:
                                                normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                                normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                                normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                                normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                                break;
                                        }

                                    
                                        if(maskNormalDirection == 1)
                                        {
                                            // Triangles
                                            triangles.Add(baseVerticesNum); // 0
                                            triangles.Add(baseVerticesNum + 1); // 1
                                            triangles.Add(baseVerticesNum + 2); // 2

                                            triangles.Add(baseVerticesNum); // 0
                                            triangles.Add(baseVerticesNum + 2); // 2
                                            triangles.Add(baseVerticesNum + 3); // 3

                                        }
                                        else
                                        {
                                            // Triangles
                                            triangles.Add(baseVerticesNum); // 0
                                            triangles.Add(baseVerticesNum + 2); // 2
                                            triangles.Add(baseVerticesNum + 1); // 1


                                            triangles.Add(baseVerticesNum); // 0
                                            triangles.Add(baseVerticesNum + 3); // 3
                                            triangles.Add(baseVerticesNum + 2); // 2
                                        }
                                    }

                                    // Set covered area to opposite normal direction
                                    for(int normalU = i; normalU < i + w; ++normalU)
                                    {
                                        for(int normalV = j; normalV < j + h; ++normalV)
                                        {
                                            normalDirections[normalU, normalV] *= -1;
                                        }
                                    }

                                    //Zero-out mask
                                    for(int l = 0; l < h; l++)
                                    {
                                        for(int k = 0; k < w; k++)
                                        {
                                            mask[n + k + l * dimensionU].HasFace = false;
                                        }
                                    }

                                    //Increment counters and continue
                                    i += w;
                                    n += w;
                                }
                                else
                                {
                                    i++;
                                    n++;
                                }
                            }
                        }
                    }
                }

                Vertices = vertices.ToArray();
                Normals = normals.ToArray();
                Colors = colors.ToArray();
                UV = uv.ToArray();
                UV2 = uv2.ToArray();
                UV3 = uv3.ToArray();
                Triangles = triangles.ToArray();

                Utilities.TangentSolver(Triangles, Vertices, Normals, UV);

                Completed = true;
            }
            catch(Exception exception)
            {
                Debug.Log(string.Format("Error creating mesh: {0}", exception.Message));
                
            }
        }
    }
}
