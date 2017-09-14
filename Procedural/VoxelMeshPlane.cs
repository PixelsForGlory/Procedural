// Copyright (C) afuzzyllama. All rights reserved
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using UnityEngine;
using System;
using PixelsForGlory.Collections;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// 2D slice of a voxel volume
    /// </summary>
    public class VoxelMeshPlane
    {
        /// <summary>
        /// Index of the plane in the volume
        /// </summary>
        public readonly int FaceIndex;

        /// <summary>
        /// Direction of the face.  Contains both positive and negative directions
        /// </summary>
        public readonly FaceType FaceDirection;

        /// <summary>
        /// Visible faces of the plane
        /// </summary>
        public List2D<FaceType> VisibleVoxels;

        private readonly List3D<IVoxel> _voxels;

        public VoxelMeshPlane(int faceIndex, FaceType faceDirection, List3D<IVoxel> voxels)
        {
            FaceIndex = faceIndex;
            FaceDirection = faceDirection;
            _voxels = voxels;
        }

        /// <summary>
        /// Generates the 2D plane
        /// </summary>
        public void GeneratePlane()
        {
            try
            {
                if ((FaceDirection & (FaceType.XNegative | FaceType.XPositive)) == (FaceType.XNegative | FaceType.XPositive))
                {
                    GeneratePlaneHelper(new Dimensions2D(_voxels.Dimensions.Y, _voxels.Dimensions.Z), _voxels.Dimensions.X, FaceType.XNegative, FaceType.XPositive);
                }
                else if ((FaceDirection & (FaceType.YNegative | FaceType.YPositive)) == (FaceType.YNegative | FaceType.YPositive))
                {
                    GeneratePlaneHelper(new Dimensions2D(_voxels.Dimensions.X, _voxels.Dimensions.Z), _voxels.Dimensions.Y, FaceType.YNegative, FaceType.YPositive);
                }
                else if ((FaceDirection & (FaceType.ZNegative | FaceType.ZPositive)) == (FaceType.ZNegative | FaceType.ZPositive))
                {
                    GeneratePlaneHelper(new Dimensions2D(_voxels.Dimensions.X, _voxels.Dimensions.Y), _voxels.Dimensions.Z, FaceType.ZNegative, FaceType.ZPositive);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }

        /// <summary>
        /// Helper for generation method
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="faceDimension"></param>
        /// <param name="negativeFace"></param>
        /// <param name="positiveFace"></param>
        private void GeneratePlaneHelper(Dimensions2D dimensions, int faceDimension, FaceType negativeFace, FaceType positiveFace)
        {
            VisibleVoxels = new List2D<FaceType>(new Dimensions2D(dimensions.X, dimensions.Y));

            // Loop over plane and find which voxels should be visible.
            // A voxel is visible if it is at the edge of a volume or if a face is not touching another voxel
            for (int u = 0; u < dimensions.X; u++)
            {
                for (int v = 0; v < dimensions.Y; v++)
                {
                    FaceType renderFaces = FaceType.None;

                    if (!GetVoxel(0, u, v).Empty)
                    {
                        if (FaceIndex == 0)
                        {
                            renderFaces |= negativeFace;
                        }
                        else if (GetVoxel(-1, u, v).Empty)
                        {
                            renderFaces |= negativeFace;
                        }

                        if (FaceIndex == faceDimension - 1)
                        {
                            renderFaces |= positiveFace;
                        }
                        else if (GetVoxel(1, u, v).Empty)
                        {
                            renderFaces |= positiveFace;
                        }
                    }

                    VisibleVoxels[u, v] = renderFaces;
                }
            }
        }

        private IVoxel GetVoxel(int faceOffset, int u, int v)
        {
            switch (FaceDirection)
            {
                case FaceType.XPositive | FaceType.XNegative:
                    return _voxels[FaceIndex + faceOffset, u, v];

                case FaceType.YPositive | FaceType.YNegative:
                    return _voxels[u, FaceIndex + faceOffset, v];

                case FaceType.ZPositive | FaceType.ZNegative:
                    return _voxels[u, v, FaceIndex + faceOffset];

                default:
                    throw new Exception("Unkown face type");
            }
        }
    }
}