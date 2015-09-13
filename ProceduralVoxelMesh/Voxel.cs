// Copyright 2015 afuzzyllama. All Rights Reserved.
using System;
using UnityEngine;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Represents a single voxel.
    /// </summary>
    [Serializable]
    public struct Voxel
    {
        /// <summary>
        /// Is the voxel empty or not
        /// </summary>
        public bool Empty;

        /// <summary>
        /// R part of color, broken up for serialization purposes
        /// </summary>
        [SerializeField]
        private float _colorR;

        /// <summary>
        /// G part of color, broken up for serialization purposes
        /// </summary>
        [SerializeField]
        private float _colorG;

        /// <summary>
        /// B part of color, broken up for serialization purposes
        /// </summary>
        [SerializeField]
        private float _colorB;

        /// <summary>
        /// A part of color, broken up for serialization purposes
        /// </summary>
        [SerializeField]
        private float _colorA;

        /// <summary>
        /// Color of the voxel
        /// </summary>
        public Color Color
        {
            get
            {
                if (Empty)
                {
                    throw new InvalidOperationException("Cannot get the color of an empty voxel");
                }
                return new Color(_colorR, _colorG, _colorB, _colorA);
            }
            set
            {
                Empty = false;
                _colorR = value.r;
                _colorG = value.g;
                _colorB = value.b;
                _colorA = value.a;
            }
        }

        /// <summary>
        /// Single voxel
        /// </summary>
        /// <param name="empty">Is the voxel empty</param>
        /// <param name="color">If the voxel is not empty, the color of this voxel</param>
        public Voxel(bool empty, Color? color = null)
        {
            Empty = empty;
            _colorR = 0.0f;
            _colorG = 0.0f;
            _colorB = 0.0f;
            _colorA = 0.0f;
            
            if (!empty)
            {
                if (color == null)
                {
                    throw new ArgumentException("If not empty, color cannot be null");
                }
                Color = (Color) color;
            }
        }
    }
}
