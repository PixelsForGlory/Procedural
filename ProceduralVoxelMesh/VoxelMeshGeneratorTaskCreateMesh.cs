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
    public partial class VoxelMeshGeneratorTask
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
                    int dimensionDimension = 0;

                    switch(dimension)
                    {
                        case 0:
                            dimensionDimension = _width;
                            break;
                        case 1:
                            dimensionDimension = _height;
                            break;
                        case 2:
                            dimensionDimension = _depth;
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

                    FaceMask[] mask = new FaceMask[maskSize];

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

                    while(x[dimension] < dimensionDimension)
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
                                if(x[dimension] >= dimensionDimension - 1 || _voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]].Empty)
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
                                        mask[maskIndex].FaceColor = _voxels[x[0], x[1], x[2]].Color;
                                        mask[maskIndex].FirstOrSecond = 1;
                                        mask[maskIndex].Metallic = _voxels[x[0], x[1], x[2]].Metallic;
                                        mask[maskIndex].Smoothness = _voxels[x[0], x[1], x[2]].Smoothness;
                                        mask[maskIndex].Emission = _voxels[x[0], x[1], x[2]].Emission;
                                    }
                                    else
                                    {
                                        mask[maskIndex].FaceColor = _voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]].Color;
                                        mask[maskIndex].FirstOrSecond = 2;
                                        mask[maskIndex].Metallic = _voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]].Metallic;
                                        mask[maskIndex].Smoothness = _voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]].Smoothness;
                                        mask[maskIndex].Emission = _voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]].Emission;
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
                                            && mask[n].FaceColor == mask[n + w].FaceColor 
                                            && i + w < dimensionU && mask[n].FirstOrSecond == mask[n + w].FirstOrSecond
                                            && Mathf.Abs(mask[n].Metallic - mask[n + w].Metallic) < 0.00390625f // 1/256
                                            && Mathf.Abs(mask[n].Smoothness - mask[n + w].Smoothness) < 0.00390625f // 1/256
                                            && Mathf.Abs(mask[n].Emission - mask[n + w].Emission) < 0.00390625f // 1/256
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
                                                || mask[n].FaceColor != mask[n + k + h * dimensionU].FaceColor 
                                                || mask[n].FirstOrSecond != mask[n + k + h * dimensionU].FirstOrSecond
                                                || Math.Abs(mask[n].Metallic - mask[n + k + h * dimensionU].Metallic) > 0.00390625f // 1/256
                                                || Math.Abs(mask[n].Smoothness - mask[n + k + h * dimensionU].Smoothness) > 0.00390625f // 1/256
                                                || Math.Abs(mask[n].Emission - mask[n + k + h * dimensionU].Emission) > 0.00390625f // 1/256
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
                                    int faceIndex = 0;

                                    // Where what is stored:
                                    // uv0 - Standard texture coordinates 0.0->1.0

                                    if(u == 1 && v == 2) // X
                                    {
                                        faceIndex = 1;
                                    }
                                    else if(u == 2 && v == 0) // Y
                                    {
                                        faceIndex = 2;
                                    }
                                    else if(u == 0 && v == 1) // 2
                                    {
                                        faceIndex = 3;
                                    }

                                    int baseVerticesNum = Vertices.Count;
                                    
                                    // Vertices
                                    Vertices.Add(new Vector3(x[0], x[1], x[2]) - offset); // 0
                                    Vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]) - offset); // 1
                                    Vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]) -offset); // 2
                                    Vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]) - offset); // 3

                                    // Normals
                                    switch(faceIndex)
                                    {
                                        case 1:
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            Normals.Add(new Vector3(maskNormalDirection, 0.0f, 0.0f));
                                            break;
                                        case 2:
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            Normals.Add(new Vector3(0.0f, maskNormalDirection, 0.0f));
                                            break;
                                        case 3:
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

                                    // Colors
                                    Colors.Add(mask[n].FaceColor);  // 0
                                    Colors.Add(mask[n].FaceColor);  // 1
                                    Colors.Add(mask[n].FaceColor);  // 2
                                    Colors.Add(mask[n].FaceColor);  // 3

                                    const float texelSize = 1.0f / 32.0f;
                                    // TEXCOORD0/UV1
                                    // Map to metallic
                                    int metallicIndex = Mathf.RoundToInt(mask[n].Metallic * 255.0f);
                                    int textureX = (metallicIndex % 16) * 2;
                                    int textureY = (metallicIndex / 16) * 2;
                                    

                                    var minTexel = new Vector2(
                                        textureX * texelSize    // Get to metallic location
                                        + texelSize / 2.0f,     // Move half a texel length in
                                        textureY * texelSize 
                                        + texelSize / 2.0f);

                                    var maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

                                    switch(faceIndex)
                                    {
                                        case 1:
                                            UV.Add(new Vector2(maxTexel.x, maxTexel.y)); // 0
                                            UV.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                                            UV.Add(new Vector2(minTexel.x, minTexel.y)); // 2
                                            UV.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                                            break;
                                        case 2:
                                            UV.Add(new Vector2(minTexel.x, minTexel.y)); // 0
                                            UV.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                                            UV.Add(new Vector2(maxTexel.x, maxTexel.y)); // 2
                                            UV.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                                            break;
                                        case 3:
                                            UV.Add(new Vector2(minTexel.x, maxTexel.y)); // 0
                                            UV.Add(new Vector2(minTexel.x, minTexel.y)); // 1
                                            UV.Add(new Vector2(maxTexel.x, minTexel.y)); // 2
                                            UV.Add(new Vector2(maxTexel.x, maxTexel.y)); // 3
                                            break;
                                    }

                                    // TEXCOORD1/UV2
                                    // Map to smoothness
                                    int smoothnessIndex = Mathf.RoundToInt(mask[n].Smoothness * 255.0f);
                                    textureX = (smoothnessIndex % 16) * 2;
                                    textureY = (smoothnessIndex / 16) * 2;

                                    minTexel = new Vector2(
                                        textureX * texelSize    // Get to metallic location
                                        + texelSize / 2.0f,     // Move half a texel length in
                                        textureY * texelSize
                                        + texelSize / 2.0f);

                                    maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

                                    switch(faceIndex)
                                    {
                                        case 1:
                                            UV2.Add(new Vector2(maxTexel.x, maxTexel.y)); // 0
                                            UV2.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                                            UV2.Add(new Vector2(minTexel.x, minTexel.y)); // 2
                                            UV2.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                                            break;
                                        case 2:
                                            UV2.Add(new Vector2(minTexel.x, minTexel.y)); // 0
                                            UV2.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                                            UV2.Add(new Vector2(maxTexel.x, maxTexel.y)); // 2
                                            UV2.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                                            break;
                                        case 3:
                                            UV2.Add(new Vector2(minTexel.x, maxTexel.y)); // 0
                                            UV2.Add(new Vector2(minTexel.x, minTexel.y)); // 1
                                            UV2.Add(new Vector2(maxTexel.x, minTexel.y)); // 2
                                            UV2.Add(new Vector2(maxTexel.x, maxTexel.y)); // 3
                                            break;
                                    }

                                    // TEXCOORD2/UV3
                                    // Map to emission
                                    int emissionIndex = Mathf.RoundToInt(mask[n].Emission * 255.0f);
                                    textureX = (emissionIndex % 16) * 2;
                                    textureY = (emissionIndex / 16) * 2;

                                    minTexel = new Vector2(
                                        textureX * texelSize    // Get to metallic location
                                        + texelSize / 2.0f,     // Move half a texel length in
                                        textureY * texelSize
                                        + texelSize / 2.0f);

                                    maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

                                    switch(faceIndex)
                                    {
                                        case 1:
                                            UV3.Add(new Vector2(maxTexel.x, maxTexel.y)); // 0
                                            UV3.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                                            UV3.Add(new Vector2(minTexel.x, minTexel.y)); // 2
                                            UV3.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                                            break;
                                        case 2:
                                            UV3.Add(new Vector2(minTexel.x, minTexel.y)); // 0
                                            UV3.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                                            UV3.Add(new Vector2(maxTexel.x, maxTexel.y)); // 2
                                            UV3.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                                            break;
                                        case 3:
                                            UV3.Add(new Vector2(minTexel.x, maxTexel.y)); // 0
                                            UV3.Add(new Vector2(minTexel.x, minTexel.y)); // 1
                                            UV3.Add(new Vector2(maxTexel.x, minTexel.y)); // 2
                                            UV3.Add(new Vector2(maxTexel.x, maxTexel.y)); // 3
                                            break;
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

                Completed = true;
            }
            catch(Exception exception)
            {
                Debug.Log(string.Format("Error creating mesh: {0}", exception.Message));
                
            }
        }
    }
}
