// Copyright 2015-2016 afuzzyllama. All Rights Reserved.

namespace PixelsForGlory.ProceduralVoxelMesh
{
    /// <summary>
    /// Interface that all voxel mesh generators need to implement
    /// </summary>
    public interface IVoxelMeshGeneratorTask
    {
        /// <summary>
        /// Create a mesh from the data passed to the task
        /// </summary>
        void CreateMesh();
    }
}
