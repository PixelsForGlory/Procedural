// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Data required to generate a billboard cross mesh
    /// </summary>
    [Serializable]
    public class BillboardCrossMeshData
    {
        public readonly string UniqueId;
        public readonly string Name;
        public readonly int MiddleCount;
        public readonly int BottomTextureIndex;
        public readonly int MiddleTextureIndex;
        public readonly int TopTextureIndex;

        public BillboardCrossMeshData()
        {
            UniqueId = Guid.NewGuid().ToString();
            Name = string.Empty;
            MiddleCount = 0;
            BottomTextureIndex = -1;
            MiddleTextureIndex = -1;
            TopTextureIndex = -1;
        }

        public BillboardCrossMeshData(string name, int middleCount, int middleTextureIndex, int topTextureIndex = -1, int bottomTextureIndex = -1)
        {
            UniqueId = Guid.NewGuid().ToString();
            Name = name;
            MiddleCount = middleCount;
            BottomTextureIndex = bottomTextureIndex;
            MiddleTextureIndex = middleTextureIndex;
            TopTextureIndex = topTextureIndex;
        }

        public BillboardCrossMeshData(string uniqueId, string name, int middleCount, int middleTextureIndex, int topTextureIndex = -1, int bottomTextureIndex = -1)
        {
            UniqueId = uniqueId;
            Name = name;
            MiddleCount = middleCount;
            BottomTextureIndex = bottomTextureIndex;
            MiddleTextureIndex = middleTextureIndex;
            TopTextureIndex = topTextureIndex;
        }

        public BillboardCrossMeshData(BillboardCrossMeshData objectToCopy)
        {
            UniqueId = objectToCopy.UniqueId;
            Name = objectToCopy.Name;
            MiddleCount = objectToCopy.MiddleCount;
            BottomTextureIndex = objectToCopy.BottomTextureIndex;
            MiddleTextureIndex = objectToCopy.MiddleTextureIndex;
            TopTextureIndex = objectToCopy.TopTextureIndex;
        }
    }
}
