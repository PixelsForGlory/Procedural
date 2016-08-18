// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    /// <summary>
    /// Task to generate single mesh from voxel data
    /// </summary>
    public partial class VoxelMeshGeneratorTask<T>
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

        public Vector4[] Tangents;

        /// <summary>
        /// Set to true when the task is completed
        /// </summary>
        public bool Completed;

        // Data passed in to task 
        private readonly IList<T> _voxels;
        private readonly int _width;
        private readonly int _depth;
        private readonly int _height;
        private readonly int _levelOfDetailDivisor;
        private readonly Vector3 _voxelScaleFactor;

        public VoxelMeshGeneratorTask(IList<T> voxels, int levelOfDetail, int width, int height, int depth)
        {
            _levelOfDetailDivisor = Mathf.RoundToInt(Mathf.Pow(2f, levelOfDetail));
            _width = width;
            _height = height;
            _depth = depth;
            Completed = false;
            _voxels = new List<T>(voxels);

            // Since we want the voxel mesh to be the scale as represented in Unity
            // this scale factor will make sure the mesh comes out the right size
            _voxelScaleFactor = new Vector3( 1f / _width, 1f / _height, 1f / _depth);
        }

        /// <summary>
        /// Information needed to determine face mask
        /// </summary>
        private struct FaceMask<TVoxel> where TVoxel : IVoxel
        {
            public bool HasFace;
            public int FirstOrSecond;
            public TVoxel Voxel;
        };
    }
}
