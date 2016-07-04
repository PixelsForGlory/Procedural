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
        private readonly T[,,] _voxels;
        private readonly int _width;
        private readonly int _depth;
        private readonly int _height;
        private readonly int _levelOfDetailDivisor;

        public VoxelMeshGeneratorTask(IList<T> voxels, int levelOfDetail, int width, int height, int depth)
        {
            _levelOfDetailDivisor = Mathf.RoundToInt(Mathf.Pow(2f, levelOfDetail));
            _width = width / _levelOfDetailDivisor;
            _height = height / _levelOfDetailDivisor;
            _depth = depth / _levelOfDetailDivisor;
            Completed = false;

            // Make a deep copy of the passed in array to this task
            _voxels = new T[_width, _height, _depth];
            for(int w = 0; w < _width; w++)
            {
                for(int h = 0; h < _height; h++)
                {
                    for(int d = 0; d < _depth; d++)
                    {
                        _voxels[w, h, d] = (T)voxels[Utilities.GetIndex(w * _levelOfDetailDivisor, h * _levelOfDetailDivisor, d * _levelOfDetailDivisor, width, height, depth)].DeepCopy();
                    }
                }
            }
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
