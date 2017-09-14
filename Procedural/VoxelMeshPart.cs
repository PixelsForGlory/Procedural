// Copyright (C) afuzzyllama. All rights reserved
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using PixelsForGlory.Collections;
using UnityEngine;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Generates part of a voxel mesh from a 2D plane
    /// </summary>
    public class VoxelMeshPart
    {
        public List<Vector3> Vertices;
        public List<Vector3> Normals;
        public List<Color> Colors;
        public List<Vector2> UV;
        public List<Vector2> UV2;
        public List<Vector2> UV3;
        public List<int> Triangles;

        private readonly int _faceIndex;
        private readonly FaceType _faceType;
        private readonly List2D<FaceType> _voxelPlaneFaces;
        private readonly List3D<IVoxel> _voxels;

        private readonly Vector3 _offset;
        private readonly float _normalDirection;

        public VoxelMeshPart(int faceIndex, FaceType faceType, List2D<FaceType> voxelPlaneFaces, List3D<IVoxel> voxels)
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            Colors = new List<Color>();
            UV = new List<Vector2>();
            UV2 = new List<Vector2>();
            UV3 = new List<Vector2>();
            Triangles = new List<int>();

            _faceIndex = faceIndex;
            _faceType = faceType;
            _voxelPlaneFaces = voxelPlaneFaces;
            _voxels = voxels;

            _offset = new Vector3(_voxels.Dimensions.X, _voxels.Dimensions.Y, _voxels.Dimensions.Z) / 2f;

            if ((faceType & FaceType.XPositive) == FaceType.XPositive ||
                (faceType & FaceType.YPositive) == FaceType.YPositive ||
                (faceType & FaceType.ZPositive) == FaceType.ZPositive)
            {
                _normalDirection = 1f;
            }
            else
            {
                _normalDirection = -1f;
            }

        }

        /// <summary>
        /// Generate the mesh part
        /// </summary>
        public void GenerateMeshPart()
        {
            try
            {
                switch (_faceType)
                {
                    case FaceType.XPositive:
                    case FaceType.XNegative:
                        GenerateMeshPartHelper(new Dimensions2D(_voxels.Dimensions.Y, _voxels.Dimensions.Z));
                        break;

                    case FaceType.YPositive:
                    case FaceType.YNegative:
                        GenerateMeshPartHelper(new Dimensions2D(_voxels.Dimensions.X, _voxels.Dimensions.Z));
                        break;

                    case FaceType.ZPositive:
                    case FaceType.ZNegative:
                        GenerateMeshPartHelper(new Dimensions2D(_voxels.Dimensions.X, _voxels.Dimensions.Y));
                        break;

                    default:
                        throw new Exception("Unknown face type");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }

        /// <summary>
        /// Helper method for mesh generation
        /// </summary>
        /// <param name="dimensions"></param>
        private void GenerateMeshPartHelper(Dimensions2D dimensions)
        {
            // Create a 2D list that represents if a coordinate has been processed or not
            // Also create a queue that contains all possible voxels to process
            var processed = new List2D<bool>(dimensions);
            var processQueue = new Queue<Dimensions2D>();
            for (int u = 0; u < dimensions.X; u++)
            {
                for (int v = 0; v < dimensions.Y; v++)
                {
                    processed[u, v] = false;
                    processQueue.Enqueue(new Dimensions2D(u, v));
                }
            }

            // Process the entire queue
            while (processQueue.Count > 0)
            {
                // Find the first voxel that has not been processed and has a visible face
                var currentCoord = processQueue.Dequeue();
                int startU = currentCoord.X;
                int startV = currentCoord.Y;
                if (processed[startU, startV] || (_voxelPlaneFaces[startU, startV] & _faceType) != _faceType)
                {
                    processed[startU, startV] = true;
                    continue;
                }

                // Find width
                // Stop when one of the following is found
                //      - an empty voxel
                //      - voxel that doesn't have a visible face
                //      - voxel that doesn't match the starting voxel
                //      - voxel that has already been processed
                int width = 0;
                for (int u = startU; u < dimensions.X; u++)
                {
                    if (GetVoxel(u, startV).Empty ||
                        (_voxelPlaneFaces[u, startV] & _faceType) != _faceType ||
                        !GetVoxel(startU, startV).Equals(GetVoxel(u, startV)) ||
                        processed[u, startV])
                    {
                        break;
                    }
                    processed[u, startV] = true;
                    width++;
                }

                // Find height
                // Stop when one of the following is found
                //      - an empty voxel
                //      - voxel that doesn't have a visible face
                //      - voxel that doesn't match the starting voxel
                //      - voxel that has already been processed
                int height = 1;
                for (int v = startV + 1; v < dimensions.Y + 1; v++)
                {
                    bool fullHeight = true;
                    // The entire width will be looped over even if the height is not full..
                    // This is done so the previous full height can be marked as processed
                    for (int u = startU; u < startU + width; u++)
                    {
                        if (v != dimensions.Y &&
                            (GetVoxel(u, v).Empty ||
                             (_voxelPlaneFaces[u, v] & _faceType) != _faceType ||
                             !GetVoxel(startU, startV).Equals(GetVoxel(u, v)) ||
                             processed[u, v]))
                        {
                            fullHeight = false;
                        }


                        processed[u, v - 1] = true;
                    }

                    if (!fullHeight)
                    {
                        break;
                    }

                    if (v != dimensions.Y)
                    {
                        height++;
                    }
                }

                AddFace(startU, startV, width, height);
            }
        }

        /// <summary>
        /// Add a mesh face to the mesh part
        /// </summary>
        /// <param name="startU"></param>
        /// <param name="startV"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void AddFace(int startU, int startV, int width, int height)
        {
            int startingVerticesNumber = Vertices.Count;

            switch (_faceType)
            {
                case FaceType.XPositive:
                    Vertices.Add(new Vector3(_faceIndex + 1, startU, startV) - _offset);                  // 0
                    Vertices.Add(new Vector3(_faceIndex + 1, startU + width, startV) - _offset);          // 1
                    Vertices.Add(new Vector3(_faceIndex + 1, startU + width, startV + height) - _offset); // 2
                    Vertices.Add(new Vector3(_faceIndex + 1, startU, startV + height) - _offset);         // 3
                    break;

                case FaceType.XNegative:
                    Vertices.Add(new Vector3(_faceIndex, startU, startV) - _offset);                  // 0
                    Vertices.Add(new Vector3(_faceIndex, startU + width, startV) - _offset);          // 1
                    Vertices.Add(new Vector3(_faceIndex, startU + width, startV + height) - _offset); // 2
                    Vertices.Add(new Vector3(_faceIndex, startU, startV + height) - _offset);         // 3
                    break;

                case FaceType.YPositive:
                    Vertices.Add(new Vector3(startU, _faceIndex + 1, startV) - _offset);                  // 0
                    Vertices.Add(new Vector3(startU + width, _faceIndex + 1, startV) - _offset);          // 1
                    Vertices.Add(new Vector3(startU + width, _faceIndex + 1, startV + height) - _offset); // 2
                    Vertices.Add(new Vector3(startU, _faceIndex + 1, startV + height) - _offset);         // 3
                    break;

                case FaceType.YNegative:
                    Vertices.Add(new Vector3(startU, _faceIndex, startV) - _offset);                  // 0
                    Vertices.Add(new Vector3(startU + width, _faceIndex, startV) - _offset);          // 1
                    Vertices.Add(new Vector3(startU + width, _faceIndex, startV + height) - _offset); // 2
                    Vertices.Add(new Vector3(startU, _faceIndex, startV + height) - _offset);         // 3
                    break;

                case FaceType.ZPositive:
                    Vertices.Add(new Vector3(startU, startV, _faceIndex + 1) - _offset);                  // 0
                    Vertices.Add(new Vector3(startU + width, startV, _faceIndex + 1) - _offset);          // 1
                    Vertices.Add(new Vector3(startU + width, startV + height, _faceIndex + 1) - _offset); // 2
                    Vertices.Add(new Vector3(startU, startV + height, _faceIndex + 1) - _offset);         // 3
                    break;

                case FaceType.ZNegative:
                    Vertices.Add(new Vector3(startU, startV, _faceIndex) - _offset);                  // 0
                    Vertices.Add(new Vector3(startU + width, startV, _faceIndex) - _offset);          // 1
                    Vertices.Add(new Vector3(startU + width, startV + height, _faceIndex) - _offset); // 2
                    Vertices.Add(new Vector3(startU, startV + height, _faceIndex) - _offset);         // 3
                    break;

                default:
                    throw new Exception("Unknown face type");
            }

            switch (_faceType)
            {
                case FaceType.XPositive:
                case FaceType.YNegative:
                case FaceType.ZPositive:
                    Triangles.Add(startingVerticesNumber);     // 0
                    Triangles.Add(startingVerticesNumber + 1); // 1
                    Triangles.Add(startingVerticesNumber + 2); // 2

                    Triangles.Add(startingVerticesNumber);     // 0
                    Triangles.Add(startingVerticesNumber + 2); // 2
                    Triangles.Add(startingVerticesNumber + 3); // 3
                    break;

                case FaceType.XNegative:
                case FaceType.YPositive:
                case FaceType.ZNegative:
                    Triangles.Add(startingVerticesNumber);     // 0
                    Triangles.Add(startingVerticesNumber + 2); // 2
                    Triangles.Add(startingVerticesNumber + 1); // 1


                    Triangles.Add(startingVerticesNumber);     // 0
                    Triangles.Add(startingVerticesNumber + 3); // 3
                    Triangles.Add(startingVerticesNumber + 2); // 2
                    break;

                default:
                    throw new Exception("Unknown face type");
            }

            switch (_faceType)
            {
                case FaceType.XPositive:
                case FaceType.XNegative:
                    Normals.Add(new Vector3(_normalDirection, 0f, 0f));
                    Normals.Add(new Vector3(_normalDirection, 0f, 0f));
                    Normals.Add(new Vector3(_normalDirection, 0f, 0f));
                    Normals.Add(new Vector3(_normalDirection, 0f, 0f));
                    break;

                case FaceType.YPositive:
                case FaceType.YNegative:
                    Normals.Add(new Vector3(0f, _normalDirection, 0f));
                    Normals.Add(new Vector3(0f, _normalDirection, 0f));
                    Normals.Add(new Vector3(0f, _normalDirection, 0f));
                    Normals.Add(new Vector3(0f, _normalDirection, 0f));
                    break;

                case FaceType.ZPositive:
                case FaceType.ZNegative:
                    Normals.Add(new Vector3(0f, 0f, _normalDirection));
                    Normals.Add(new Vector3(0f, 0f, _normalDirection));
                    Normals.Add(new Vector3(0f, 0f, _normalDirection));
                    Normals.Add(new Vector3(0f, 0f, _normalDirection));
                    break;

                default:
                    throw new Exception("Unknown face type");
            }

            GetVoxel(startU, startV).AddVoxelDataToMesh(_faceType, width, height, Colors, UV, UV2, UV3);
        }

        private IVoxel GetVoxel(int u, int v)
        {
            switch (_faceType)
            {
                case FaceType.XPositive:
                case FaceType.XNegative:
                    return _voxels[_faceIndex, u, v];

                case FaceType.YPositive:
                case FaceType.YNegative:
                    return _voxels[u, _faceIndex, v];

                case FaceType.ZPositive:
                case FaceType.ZNegative:
                    return _voxels[u, v, _faceIndex];

                default:
                    throw new Exception("Unknown face type");
            }
        }
    }
}