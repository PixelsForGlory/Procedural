// Copyright 2012 Mikola Lysenko
// Implementation based off of blog post and javascript code written by Mikola Lysenko
// Reference: 
//  Blog post:  http://0fps.net/2012/06/30/meshing-in-a-minecraft-game/
//  Github:     https://github.com/mikolalysenko/mikolalysenko.github.com/tree/gh-pages/MinecraftMeshes
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Faces of a cube
    /// </summary>
    public enum FaceType
    {
        None,
        XPositive,
        XNegative,
        YPositive,
        YNegative,
        ZPositive,
        ZNegative
    }

    public partial class VoxelMeshGeneratorTask<T> : IVoxelMeshGeneratorTask where T : Voxel
    {

        /// <summary>
        /// Create the mesh from the current voxel data
        /// </summary>
        public void CreateMesh()
        {
            try
            {
                Vertices = new List<Vector3>();
                Normals = new List<Vector3>();
                Colors = new List<Color>();
                UV = new List<Vector2>();
                UV2 = new List<Vector2>();
                UV3 = new List<Vector2>();
                Triangles = new List<int>();

                Vector3 offset = new Vector3(_width / 2.0f, _height / 2.0f, _depth / 2.0f);

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
                            dimensionMax = _width;
                            break;
                        case 1:
                            dimensionMax = _height;
                            break;
                        case 2:
                            dimensionMax = _depth;
                            break;
                    }

                    switch(u)
                    {
                        case 0:
                            dimensionU = _width;
                            break;
                        case 1:
                            dimensionU = _height;
                            break;
                        case 2:
                            dimensionU = _depth;
                            break;
                    }

                    switch(v)
                    {
                        case 0:
                            dimensionV = _width;
                            break;
                        case 1:
                            dimensionV = _height;
                            break;
                        case 2:
                            dimensionV = _depth;
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
                                if(0 > x[dimension] || _voxels[x[0], x[1], x[2]].Empty)
                                {
                                    first = false;
                                }
                                else
                                {
                                    first = true;
                                }

                                bool second;
                                if(x[dimension] >= dimensionMax - 1 || _voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]].Empty)
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
                                        mask[maskIndex].Voxel = _voxels[x[0], x[1], x[2]];
                                        mask[maskIndex].FirstOrSecond = 1;
                                    }
                                    else
                                    {
                                        mask[maskIndex].Voxel = _voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]];
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
                        ++x[dimension];

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

                                    // 1 -> X
                                    // 2 -> Y
                                    // 3 -> Z
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

                                    int baseVerticesNum = Vertices.Count;
                                    
                                    // Vertices
                                    Vertices.Add(new Vector3(x[0], x[1], x[2]) - offset); // 0
                                    Vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]) - offset); // 1
                                    Vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]) - offset); // 2
                                    Vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]) - offset); // 3

                                    // Normals
                                    switch(faceType)
                                    {
                                        case FaceType.XPositive:
                                        case FaceType.XNegative:
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            break;
                                        case FaceType.YPositive:
                                        case FaceType.YNegative:
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            break;
                                        case FaceType.ZPositive:
                                        case FaceType.ZNegative:
                                            Normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                            Normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                            Normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                            Normals.Add(new Vector3(0.0f, 0.0f, maskNormalDirection));
                                            break;
                                    }

                                    if(maskNormalDirection == 1)
                                    {
                                        // Triangles
                                        Triangles.Add(baseVerticesNum); // 0
                                        Triangles.Add(baseVerticesNum + 1); // 1
                                        Triangles.Add(baseVerticesNum + 2); // 2

                                        Triangles.Add(baseVerticesNum); // 0
                                        Triangles.Add(baseVerticesNum + 2); // 2
                                        Triangles.Add(baseVerticesNum + 3); // 3

                                    }
                                    else
                                    {
                                        // Triangles
                                        Triangles.Add(baseVerticesNum); // 0
                                        Triangles.Add(baseVerticesNum + 2); // 2
                                        Triangles.Add(baseVerticesNum + 1); // 1


                                        Triangles.Add(baseVerticesNum); // 0
                                        Triangles.Add(baseVerticesNum + 3); // 3
                                        Triangles.Add(baseVerticesNum + 2); // 2
                                    }

                                    mask[n].Voxel.AddVoxelToMesh(faceType, w, h, Colors, UV, UV2, UV3);

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

                Completed = true;
            }
            catch(Exception exception)
            {
                Debug.Log(string.Format("Error creating mesh: {0}", exception.Message));
                
            }
        }
    }
}
