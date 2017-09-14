// Copyright (C) afuzzyllama. All rights reserved
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelsForGlory.Procedural
{
    public interface IVoxel : IEquatable<IVoxel>
    {
        /// <summary>
        /// Is the voxel empty or not
        /// </summary>
        bool Empty { get; set; }

        /// <summary>
        /// Adds the voxel to the passed in color and uv lists
        /// </summary>
        /// <param name="faceType">Voxel face that is currently being added to the mesh</param>
        /// <param name="width">SizeX of the current face</param>
        /// <param name="height">SizeY of the current face</param>
        /// <param name="colors">Color list to add the voxel face to</param>
        /// <param name="uv">UV list to add the voxel face to</param>
        /// <param name="uv2">UV2 list to add the voxel face to</param>
        /// <param name="uv3">UV3 list to add the voxel face to</param>
        /// <returns>True if the voxel is added, false otherwise</returns>
        bool AddVoxelDataToMesh(FaceType faceType, int width, int height, List<Color> colors, List<Vector2> uv, List<Vector2> uv2, List<Vector2> uv3);
    }
}