using System;
using UnityEngine;

namespace ProceduralVoxelMesh
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
                throw new Exception("Index not in bounds");
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

        public static void CreateTexture()
        {
            // Alpha map
            Texture2D alphaMap = new Texture2D(32, 32);
            for(int a = 0; a < 256; ++a)
            {
                int x = (a%16)*2;
                int y = (a/16)*2;

                alphaMap.SetPixel(x, y, new Color(a / 255.0f, a / 255.0f, a / 255.0f));
                alphaMap.SetPixel(x, y + 1, new Color(a / 255.0f, a / 255.0f, a / 255.0f));
                alphaMap.SetPixel(x + 1, y, new Color(a / 255.0f, a / 255.0f, a / 255.0f));
                alphaMap.SetPixel(x + 1, y + 1, new Color(a / 255.0f, a / 255.0f, a / 255.0f));
            }
            alphaMap.Apply();

            System.IO.File.WriteAllBytes("AlphaMap.png", alphaMap.EncodeToPNG());
        }
    }
}
