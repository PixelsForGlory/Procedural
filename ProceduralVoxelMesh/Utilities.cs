// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    public static class Utilities
    {
        /// <summary>
        /// Calculate 3-dimensional index based on passed in lengths
        /// </summary>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d">depth</param>
        /// <param name="wLength">max width</param>
        /// <param name="hLength">max height</param>
        /// <param name="dLength">max length</param>
        /// <returns>1-dimensional index</returns>
        public static int GetIndex(int w, int h, int d, int wLength, int hLength, int dLength)
        {
            if(!InBounds(w, h, d, wLength, hLength, dLength))
            {
                throw new ArgumentOutOfRangeException();
            }
            return w + h * wLength + d * wLength * hLength;
        }

        /// <summary>
        /// Test that passed in dimension is in bounds
        /// </summary>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d">depth</param>
        /// <param name="wLength">max width</param>
        /// <param name="hLength">max height</param>
        /// <param name="dLength">max length</param>
        /// <returns>True if in bounds, false otherwise</returns>
        public static bool InBounds(int w, int h, int d, int wLength, int hLength, int dLength)
        {
            return w >= 0 && w < wLength && h >= 0 && h < hLength && d >= 0 && d < dLength;
        }
    }
}
