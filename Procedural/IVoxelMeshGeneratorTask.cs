// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Interface that all voxel mesh generators need to implement
    /// </summary>
    public interface IVoxelMeshGeneratorTask
    {
        /// <summary>
        /// Set to true when the task is completed
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// Create a mesh from the data passed to the task
        /// </summary>
        void CreateMesh();
    }
}
