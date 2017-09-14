// Copyright (C) afuzzyllama. All rights reserved
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using PixelsForGlory.Collections;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Data structure to separate data from component 
    /// </summary>
    public class VoxelMeshData
    {
        /// <summary>
        /// Name associated with voxel data
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 3-dimensional voxel volume represented in 1-dimensional list.  Lists play nice with serialization and allows for persisting data.
        /// </summary>
        public readonly List3D<IVoxel> Voxels;

        public VoxelMeshData()
        {
            Name = string.Empty;
            Voxels = new List3D<IVoxel>(new Dimensions3D(0, 0, 0));
        }

        public VoxelMeshData(string name, Dimensions3D dimensions)
        {
            Name = name;
            Voxels = new List3D<IVoxel>(dimensions);
        }

        public VoxelMeshData(string name, List3D<IVoxel> voxels)
        {
            Name = name;
            Voxels = new List3D<IVoxel>(voxels);
        }
    }
}