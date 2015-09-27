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

        [SerializeField]
        private float _metallic;

        public float Metallic
        {
            get
            {
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot get the metallic of an empty voxel");
                }
                return _metallic;
            }
            set
            {
                if(value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException("Metallic value cannot be less than 0.0f or greater than 1.0f");
                }
                _metallic = value;
            }
        }

        [SerializeField]
        private float _smoothness;

        public float Smoothness
        {
            get
            {
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot get the smoothness of an empty voxel");
                }
                return _smoothness;
            }
            set
            {
                if(value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException("Smoothness value cannot be less than 0.0f or greater than 1.0f");
                }
                _smoothness = value;
            }
        }

        [SerializeField]
        private float _emission;

        public float Emission
        {
            get
            {
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot get the emission of an empty voxel");
                }
                return _emission;
            }
            set
            {
                if(value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException("Emission value cannot be less than 0.0f or greater than 1.0f");
                }
                _emission = value;
            }
        }

        /// <summary>
        /// Single voxel
        /// </summary>
        /// <param name="empty">Is the voxel empty</param>
        /// <param name="color">If the voxel is not empty, the color of this voxel</param>
        /// <param name="metallic">Metallic value for this voxel</param>
        /// /// <param name="smoothness">Smoothness value for this voxel</param>
        /// /// <param name="emission">Emission value for this voxel</param>
        public Voxel(bool empty, Color? color = null, float metallic = 0.0f, float smoothness = 0.0f, float emission = 0.0f) : this()
        {
            Empty = empty;
            _colorR = 0.0f;
            _colorG = 0.0f;
            _colorB = 0.0f;
            _colorA = 0.0f;
            Metallic = metallic;
            Smoothness = smoothness;
            Emission = emission;

            if(!empty)
            {
                if(color == null)
                {
                    throw new ArgumentException("If not empty, color cannot be null");
                }
                Color = (Color) color;
            }
        }
    }
}
