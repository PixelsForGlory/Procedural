// Copyright 2015 afuzzyllama. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralVoxelMesh
{
    public class Voxel
    {
        // ReSharper disable once InconsistentNaming
        protected bool _empty;

        /// <summary>
        /// Is the voxel empty or not
        /// </summary>
        public virtual bool Empty
        {
            get
            {
                throw new NotImplementedException();
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
                throw new NotImplementedException(); ;
            }
        }

        /// <summary>
        /// Single empty voxel
        /// </summary>
        public Voxel()
        {
            _empty = true;
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
        public virtual void AddVoxelToMesh(FaceType faceType, int width, int height, List<Color> colors, List<Vector2> uv, List<Vector2> uv2, List<Vector2> uv3)
        {
            throw new NotImplementedException();
        }
    }
}
