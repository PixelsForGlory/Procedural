using System;
using System.Collections.Generic;

namespace ProceduralVoxelMesh
{
    [Serializable]
    public class ColorVoxelData : VoxelData<ColorVoxel>
    {
        public ColorVoxelData() 
            : base() {}

        public ColorVoxelData(string uniqueId, string name, int width, int height, int depth, IList<ColorVoxel> voxels) 
            : base(uniqueId, name, width, height, depth, voxels) { }

        public ColorVoxelData(string name, int width, int height, int depth, IList<ColorVoxel> voxels) 
            : base(name, width, height, depth, voxels) { }

        public ColorVoxelData(string name, int width, int height, int depth, ColorVoxel[,,] voxels)
            : base(name, width, height, depth, voxels){}

    }

    [Serializable]
    public class TextureVoxelData : VoxelData<TextureVoxel>
    {
        public TextureVoxelData() 
            : base() { }

        public TextureVoxelData(string uniqueId, string name, int width, int height, int depth, IList<TextureVoxel> voxels) 
            : base(uniqueId, name, width, height, depth, voxels) { }

        public TextureVoxelData(string name, int width, int height, int depth, IList<TextureVoxel> voxels) 
            : base(name, width, height, depth, voxels) { }

        public TextureVoxelData(string name, int width, int height, int depth, TextureVoxel[,,] voxels)
            : base(name, width, height, depth, voxels){ }
    }
    
    /// <summary>
    /// Data structure to separate data from component 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VoxelData<T> where T : new()
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
        /// 3-dimensional voxel volume represented in 1-dimensional list.  Lists play nice with serialization and allows for persisting data.
        /// </summary>
        public List<T> Voxels;

        public VoxelData()
        {
            UniqueId = System.Guid.NewGuid().ToString();
            Name = string.Empty;
            Width = 0;
            Height = 0;
            Depth = 0;
            Voxels = new List<T>();
        } 

        public VoxelData(string uniqueId, string name, int width, int height, int depth, IList<T> voxels)
        {
            UniqueId = uniqueId;
            Name = name;
            Width = width;
            Height = height;
            Depth = depth;
            Voxels = new List<T>();

            for(int i = 0; i < Width * Height * Depth; i++)
            {
                Voxels.Add(new T());
            }

            for(int i = 0; i < voxels.Count; i++)
            {
                Voxels[i] = voxels[i];
            }
        }

        public VoxelData(string name, int width, int height, int depth, IList<T> voxels)
        {
            UniqueId = System.Guid.NewGuid().ToString();
            Name = name;
            Width = width;
            Height = height;
            Depth = depth;
            Voxels = new List<T>();

            for(int i = 0; i < Width * Height * Depth; i++)
            {
                Voxels.Add(new T());
            }

            for(int i = 0; i < voxels.Count; i++)
            {
                Voxels[i] = voxels[i];
            }
        }

        public VoxelData(string name, int width, int height, int depth, T[,,] voxels)
        {
            UniqueId = System.Guid.NewGuid().ToString();
            Name = name;
            Width = width;
            Height = height;
            Depth = depth;
            Voxels = new List<T>();

            for(int i = 0; i < Width * Height * Depth; i++)
            {
                Voxels.Add(new T());
            }

            for(int w = 0; w < Width; w++)
            {
                for(int h = 0; h < Height; h++)
                {
                    for(int d = 0; d < Depth; d++)
                    {
                        Voxels[Utilities.GetIndex(w, h, d, Width, Height, Depth)] = voxels[w, h, d];
                    }
                }
            }
        }
    }
}
