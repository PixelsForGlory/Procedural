// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;

namespace PixelsForGlory.Procedural
{
    public interface IVoxel<T>
    {
        /// <summary>
        /// Is the voxel empty or not
        /// </summary>
        bool Empty
        {
            get;
            set;
        }

        /// <summary>
        /// Which facts of the mesh to render
        /// </summary>
        FaceType FacesToRender
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the voxel to the passed in color and uv lists
        /// </summary>
        /// <param name="faceType">Voxel face that is currently being added to the mesh</param>
        /// <param name="width">Width of the current face</param>
        /// <param name="height">Height of the current face</param>
        /// <param name="colors">Color list to add the voxel face to</param>
        /// <param name="uv">UV list to add the voxel face to</param>
        /// <param name="uv2">UV2 list to add the voxel face to</param>
        /// <param name="uv3">UV3 list to add the voxel face to</param>
        /// <returns>True if the voxel is added, false otherwise</returns>
        bool AddVoxelToMesh(FaceType faceType, int width, int height, List<Color> colors, List<Vector2> uv, List<Vector2> uv2, List<Vector2> uv3);

        /// <summary>
        /// Makes a deep copy of the voxel
        /// </summary>
        /// <returns></returns>
        T DeepCopy();
    }
}
