// Copyright 2015 afuzzyllama. All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Task to generate single mesh from voxel data
    /// </summary>
    public partial class VoxelMeshGeneratorTask
    {
        /// <summary>
        /// Generated vertices
        /// </summary>
        public List<Vector3> Vertices;

        /// <summary>
        /// Generated normals
        /// </summary>
        public List<Vector3> Normals;

        /// <summary>
        /// Generated colors
        /// </summary>
        public List<Color> Colors;

        /// <summary>
        /// Generated metallic uvs
        /// </summary>
        public List<Vector2> UV;

        /// <summary>
        /// Generated smoohtness uvs
        /// </summary>
        public List<Vector2> UV2;

        /// <summary>
        /// Generated emission uvs
        /// </summary>
        public List<Vector2> UV3;

        /// <summary>
        /// Generated triangles
        /// </summary>
        public List<int> Triangles;

        /// <summary>
        /// Set to true when the task is completed
        /// </summary>
        public bool Completed;

        // Data passed in to task 
        private readonly Voxel[,,] _voxels;
        private readonly int _width;
        private readonly int _depth;
        private readonly int _height;

        public VoxelMeshGeneratorTask(List<Voxel> voxels, int width, int height, int depth)
        {
            _width = width;
            _height = height;
            _depth = depth;
            Completed = false;

            // Make a deep copy of the passed in array to this task
            _voxels = new Voxel[width, height, depth];
            for(int w = 0; w < _width; ++w)
            {
                for(int h = 0; h < _height; ++h)
                {
                    for(int d = 0; d < _depth; ++d)
                    {
                        _voxels[w, h, d] = voxels[Utilities.GetIndex(w, h, d, _width, _height, _depth)];
                    }
                }
            }
        }

        /// <summary>
        /// Information needed to determine face mask
        /// </summary>
        struct FaceMask
        {
            public bool HasFace;
            public Color FaceColor;
            public float Metallic;
            public float Smoothness;
            public float Emission;
            public int FirstOrSecond;
        };
    }
}
