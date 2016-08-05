// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    [Serializable]
    public class ColorVoxelMeshData : VoxelMeshData<ColorVoxel>
    {
        public ColorVoxelMeshData() {}

        public ColorVoxelMeshData(string name, int width, int height, int depth, float scale)
            : base(name, width, height, depth, scale) { }

        public ColorVoxelMeshData(string uniqueId, string name, int width, int height, int depth, float scale, IList<ColorVoxel> voxels) 
            : base(uniqueId, name, width, height, depth, scale, voxels) { }

        public ColorVoxelMeshData(string name, int width, int height, int depth, float scale, IList<ColorVoxel> voxels) 
            : base(name, width, height, depth, scale, voxels) { }
    }

    [Serializable]
    public class TextureVoxelMeshData : VoxelMeshData<TextureVoxel>
    {
        public TextureVoxelMeshData() { }

        public TextureVoxelMeshData(string name, int width, int height, int depth, float scale)
            : base(name, width, height, depth, scale) { }

        public TextureVoxelMeshData(string uniqueId, string name, int width, int height, int depth, float scale, IList<TextureVoxel> voxels) 
            : base(uniqueId, name, width, height, depth, scale, voxels) { }

        public TextureVoxelMeshData(string name, int width, int height, int depth, float scale, IList<TextureVoxel> voxels) 
            : base(name, width, height, depth, scale, voxels) { }
    }
    
    /// <summary>
    /// Data structure to separate data from component 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VoxelMeshData<T> where T : new()
    {
        /// <summary>
        /// UniqueId based on System.Guid.  Creates a persistant unique identifier.
        /// </summary>
        public string UniqueId;

        /// <summary>
        /// Name associated with voxel data
        /// </summary>
        public string Name;

        /// <summary>
        /// Width of voxel data
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of voxel data
        /// </summary>
        public int Height;

        /// <summary>
        /// Depth of voxel data
        /// </summary>
        public int Depth;

        /// <summary>
        /// Scale of the voxel data.  By default a voxel is one unit in Unity3D.  Setting this to greater or less than 1 will cause the voxel not to be unit size.
        /// </summary>
        public float Scale;

        /// <summary>
        /// 3-dimensional voxel volume represented in 1-dimensional list.  Lists play nice with serialization and allows for persisting data.
        /// </summary>
        public IList<T> Voxels;

        protected VoxelMeshData()
        {
            UniqueId = Guid.NewGuid().ToString();
            Name = string.Empty;
            Width = 0;
            Height = 0;
            Depth = 0;
            Voxels = new List<T>();
        }

        protected VoxelMeshData(string name, int width, int height, int depth, float scale)
        {
            UniqueId = Guid.NewGuid().ToString();
            Name = name;
            Width = width;
            Height = height;
            Depth = depth;
            Scale = scale;
            Voxels = new List<T>();

            for(int i = 0; i < Width * Height * Depth; i++)
            {
                Voxels.Add(new T());
            }
        }

        protected VoxelMeshData(string uniqueId, string name, int width, int height, int depth, float scale, IList<T> voxels)
        {
            UniqueId = uniqueId;
            Name = name;
            Width = width;
            Height = height;
            Depth = depth;
            Scale = scale;
            Voxels = new List<T>(voxels);
        }

        protected VoxelMeshData(string name, int width, int height, int depth, float scale, IList<T> voxels)
        {
            UniqueId = Guid.NewGuid().ToString();
            Name = name;
            Width = width;
            Height = height;
            Depth = depth;
            Scale = scale;
            Voxels = new List<T>(voxels);
        }
    }
}
