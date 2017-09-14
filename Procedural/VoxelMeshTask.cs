// Copyright (C) afuzzyllama. All rights reserved
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Thread safe voxel mesh generation task
    /// </summary>
    public partial class VoxelMeshTask
    {
        /// <summary>
        /// Generated vertices
        /// </summary>
        public Vector3[] Vertices;

        /// <summary>
        /// Generated normals
        /// </summary>
        public Vector3[] Normals;

        /// <summary>
        /// Generated colors
        /// </summary>
        public Color[] Colors;

        /// <summary>
        /// Generated metallic uvs
        /// </summary>
        public Vector2[] UV;

        /// <summary>
        /// Generated smoohtness uvs
        /// </summary>
        public Vector2[] UV2;

        /// <summary>
        /// Generated emission uvs
        /// </summary>
        public Vector2[] UV3;

        /// <summary>
        /// Generated triangles
        /// </summary>
        public int[] Triangles;

        /// <summary>
        /// Generated tangents
        /// </summary>
        public Vector4[] Tangents;

        private readonly VoxelMeshData _data;

        public VoxelMeshTask(VoxelMeshData data)
        {
            _data = data;
        }

        /// <summary>
        /// Run mesh generation task
        /// </summary>
        public void RunTask()
        {
            try
            {
                // First, generate 2D planes that represent visible voxels in each dimension
                var tasks = new Task[_data.Voxels.Dimensions.X + _data.Voxels.Dimensions.Y + _data.Voxels.Dimensions.Z];
                var voxelMeshPlanes = new VoxelMeshPlane[_data.Voxels.Dimensions.X + _data.Voxels.Dimensions.Y + _data.Voxels.Dimensions.Z];

                int indexOffset = 0;
                for (int x = 0; x < _data.Voxels.Dimensions.X; x++)
                {
                    var voxelMeshPlane = new VoxelMeshPlane(x, FaceType.XNegative | FaceType.XPositive, _data.Voxels);

                    voxelMeshPlanes[x + indexOffset] = voxelMeshPlane;

                    var task = Task.Run(() => voxelMeshPlane.GeneratePlane());
                    tasks[x + indexOffset] = task;
                }

                indexOffset = _data.Voxels.Dimensions.X;
                for (int y = 0; y < _data.Voxels.Dimensions.Y; y++)
                {
                    var voxelMeshPlane = new VoxelMeshPlane(y, FaceType.YNegative | FaceType.YPositive, _data.Voxels);

                    voxelMeshPlanes[y + indexOffset] = voxelMeshPlane;

                    var task = Task.Run(() => voxelMeshPlane.GeneratePlane());
                    tasks[y + indexOffset] = task;
                }

                indexOffset = _data.Voxels.Dimensions.X + _data.Voxels.Dimensions.Y;
                for (int z = 0; z < _data.Voxels.Dimensions.Z; z++)
                {
                    var voxelMeshPlane = new VoxelMeshPlane(z, FaceType.ZNegative | FaceType.ZPositive, _data.Voxels);

                    voxelMeshPlanes[z + indexOffset] = voxelMeshPlane;

                    var task = Task.Run(() => voxelMeshPlane.GeneratePlane());
                    tasks[z + indexOffset] = task;
                }

                Task.WaitAll(tasks);

                // Generate the mesh in each dimension.  Each generated plane per dimension can contain both the positive and negative faces for that dimension
                tasks = new Task[voxelMeshPlanes.Length * 2];
                var voxelMeshParts = new VoxelMeshPart[voxelMeshPlanes.Length * 2];

                for (int i = 0; i < voxelMeshPlanes.Length * 2; i += 2)
                {
                    var voxelMeshPlane = voxelMeshPlanes[i / 2];

                    var voxelMeshPositiveFacePart = new VoxelMeshPart(voxelMeshPlane.FaceIndex, voxelMeshPlane.FaceDirection & (FaceType.XPositive | FaceType.YPositive | FaceType.ZPositive), voxelMeshPlane.VisibleVoxels, _data.Voxels);
                    var voxelMeshNegativeFacePart = new VoxelMeshPart(voxelMeshPlane.FaceIndex, voxelMeshPlane.FaceDirection & (FaceType.XNegative | FaceType.YNegative | FaceType.ZNegative), voxelMeshPlane.VisibleVoxels, _data.Voxels);

                    voxelMeshParts[i] = voxelMeshPositiveFacePart;
                    voxelMeshParts[i + 1] = voxelMeshNegativeFacePart;

                    var voxelMeshPositiveTask = Task.Run(() => voxelMeshPositiveFacePart.GenerateMeshPart());
                    var voxelMeshNegativeTask = Task.Run(() => voxelMeshNegativeFacePart.GenerateMeshPart());

                    tasks[i] = voxelMeshPositiveTask;
                    tasks[i + 1] = voxelMeshNegativeTask;
                }

                Task.WaitAll(tasks);

                // Join all the mesh data together for use
                var vertices = new List<Vector3>();
                var normals = new List<Vector3>();
                var colors = new List<Color>();
                var uv = new List<Vector2>();
                var uv2 = new List<Vector2>();
                var uv3 = new List<Vector2>();
                var triangles = new List<int>();

                foreach (var meshPart in voxelMeshParts)
                {
                    int triangleStartIndex = vertices.Count;

                    vertices.AddRange(meshPart.Vertices);
                    normals.AddRange(meshPart.Normals);
                    colors.AddRange(meshPart.Colors);
                    uv.AddRange(meshPart.UV);
                    uv2.AddRange(meshPart.UV2);
                    uv3.AddRange(meshPart.UV3);

                    foreach (int t in meshPart.Triangles)
                    {
                        triangles.Add(triangleStartIndex + t);
                    }
                }

                Vertices = vertices.ToArray();
                Normals = normals.ToArray();
                Colors = colors.ToArray();
                UV = uv.ToArray();
                UV2 = uv2.ToArray();
                UV3 = uv3.ToArray();
                Triangles = triangles.ToArray();

                Tangents = TangentSolver(Triangles, Vertices, Normals, UV);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }
    }
}
