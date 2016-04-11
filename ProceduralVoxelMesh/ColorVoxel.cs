// Copyright 2015 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Represents a single voxel.
    /// </summary>
    [Serializable]
    public struct ColorVoxel : IVoxel
    {
        /// <summary>
        /// Serializable value of empty
        /// </summary>
        [SerializeField]
        private bool _hasColor;

        /// <summary>
        /// Is the voxel empty or not
        /// </summary>
        public bool Empty
        {
            get { return !_hasColor; }
            set
            {
                if(value == false)
                {
                    throw new InvalidOperationException("Set voxel to be non-empty via Color property");
                }

                _hasColor = false;
                _colorR = 0.0f;
                _colorG = 0.0f;
                _colorB = 0.0f;
                _metallic = 0.0f;
                _smoothness = 0.0f;
                _emission = 0.0f;
            }
        }

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
                _hasColor = true;
                _colorR = value.r;
                _colorG = value.g;
                _colorB = value.b;
                _colorA = value.a;
            }
        }

        [SerializeField]
        private float _metallic;

        /// <summary>
        /// Metallic value of the voxel
        /// </summary>
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
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot set the metallic value of an empty voxel");
                }

                if(value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException("Metallic value cannot be less than 0.0f or greater than 1.0f");
                }
                _metallic = value;
            }
        }

        [SerializeField]
        private float _smoothness;

        /// <summary>
        /// Smoothness value of the voxel
        /// </summary>
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
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot set the smoothness value of an empty voxel");
                }

                if(value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException("Smoothness value cannot be less than 0.0f or greater than 1.0f");
                }
                _smoothness = value;
            }
        }

        [SerializeField]
        private float _emission;

        /// <summary>
        /// Emissive value of the voxel
        /// </summary>
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

                if(Empty)
                {
                    throw new InvalidOperationException("Cannot set the emission value of an empty voxel");
                }

                if(value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException("Emission value cannot be less than 0.0f or greater than 1.0f");
                }
                _emission = value;
            }
        }

        /// <summary>
        /// Copy color voxel to new instance
        /// </summary>
        /// <param name="colorVoxel"></param>
        private ColorVoxel(ColorVoxel colorVoxel)
        {
            _hasColor = colorVoxel._hasColor;
            _colorR = colorVoxel._colorR;
            _colorG = colorVoxel._colorG;
            _colorB = colorVoxel._colorB;
            _colorA = colorVoxel._colorA;
            _metallic = colorVoxel._metallic;
            _smoothness = colorVoxel._smoothness;
            _emission = colorVoxel._emission;
        }

        /// <summary>
        /// Single colored voxel
        /// </summary>
        /// <param name="color">If the voxel is not empty, the color of this voxel</param>
        /// <param name="metallic">Metallic value for this voxel</param>
        /// /// <param name="smoothness">Smoothness value for this voxel</param>
        /// /// <param name="emission">Emission value for this voxel</param>
        public ColorVoxel(Color color, float metallic = 0.0f, float smoothness = 0.0f, float emission = 0.0f) : this()
        {
            _hasColor = true;
            Color = color;
            Metallic = metallic;
            Smoothness = smoothness;
            Emission = emission;
        }

        public void AddVoxelToMesh(FaceType faceType, int width, int height, List<Color> colors, List<Vector2> uv, List<Vector2> uv2, List<Vector2> uv3)
        {
            if(faceType == FaceType.None)
            {
                throw new ArgumentException("Cannot add a voxel to a none face type", nameof(faceType));
            }

            // Colors
            colors.Add(Color);  // 0
            colors.Add(Color);  // 1
            colors.Add(Color);  // 2
            colors.Add(Color);  // 3

            const float texelSize = 1.0f / 32.0f;
            // TEXCOORD0/UV1
            // Map to metallic
            int metallicIndex = Mathf.RoundToInt(Metallic * 255.0f);
            int textureX = (metallicIndex % 16) * 2;
            int textureY = (metallicIndex / 16) * 2;


            var minTexel = new Vector2(
                textureX * texelSize    // Get to metallic location
                + texelSize / 2.0f,     // Move half a texel length in
                textureY * texelSize
                + texelSize / 2.0f);

            var maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

            switch(faceType)
            {
                case FaceType.XPositive:
                case FaceType.XNegative:
                    uv.Add(new Vector2(maxTexel.x, maxTexel.y)); // 0
                    uv.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                    uv.Add(new Vector2(minTexel.x, minTexel.y)); // 2
                    uv.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                    break;
                case FaceType.YPositive:
                case FaceType.YNegative:
                    uv.Add(new Vector2(minTexel.x, minTexel.y)); // 0
                    uv.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                    uv.Add(new Vector2(maxTexel.x, maxTexel.y)); // 2
                    uv.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                    break;
                case FaceType.ZPositive:
                case FaceType.ZNegative:
                    uv.Add(new Vector2(minTexel.x, maxTexel.y)); // 0
                    uv.Add(new Vector2(minTexel.x, minTexel.y)); // 1
                    uv.Add(new Vector2(maxTexel.x, minTexel.y)); // 2
                    uv.Add(new Vector2(maxTexel.x, maxTexel.y)); // 3
                    break;
            }

            // TEXCOORD1/UV2
            // Map to smoothness
            int smoothnessIndex = Mathf.RoundToInt(Smoothness * 255.0f);
            textureX = (smoothnessIndex % 16) * 2;
            textureY = (smoothnessIndex / 16) * 2;

            minTexel = new Vector2(
                textureX * texelSize    // Get to metallic location
                + texelSize / 2.0f,     // Move half a texel length in
                textureY * texelSize
                + texelSize / 2.0f);

            maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

            switch(faceType)
            {
                case FaceType.XPositive:
                case FaceType.XNegative:
                    uv2.Add(new Vector2(maxTexel.x, maxTexel.y)); // 0
                    uv2.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                    uv2.Add(new Vector2(minTexel.x, minTexel.y)); // 2
                    uv2.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                    break;
                case FaceType.YPositive:
                case FaceType.YNegative:
                    uv2.Add(new Vector2(minTexel.x, minTexel.y)); // 0
                    uv2.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                    uv2.Add(new Vector2(maxTexel.x, maxTexel.y)); // 2
                    uv2.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                    break;
                case FaceType.ZPositive:
                case FaceType.ZNegative:
                    uv2.Add(new Vector2(minTexel.x, maxTexel.y)); // 0
                    uv2.Add(new Vector2(minTexel.x, minTexel.y)); // 1
                    uv2.Add(new Vector2(maxTexel.x, minTexel.y)); // 2
                    uv2.Add(new Vector2(maxTexel.x, maxTexel.y)); // 3
                    break;
            }

            // TEXCOORD2/UV3
            // Map to emission
            int emissionIndex = Mathf.RoundToInt(Emission * 255.0f);
            textureX = (emissionIndex % 16) * 2;
            textureY = (emissionIndex / 16) * 2;

            minTexel = new Vector2(
                textureX * texelSize    // Get to metallic location
                + texelSize / 2.0f,     // Move half a texel length in
                textureY * texelSize
                + texelSize / 2.0f);

            maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

            switch(faceType)
            {
                case FaceType.XPositive:
                case FaceType.XNegative:
                    uv3.Add(new Vector2(maxTexel.x, maxTexel.y)); // 0
                    uv3.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                    uv3.Add(new Vector2(minTexel.x, minTexel.y)); // 2
                    uv3.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                    break;
                case FaceType.YPositive:
                case FaceType.YNegative:
                    uv3.Add(new Vector2(minTexel.x, minTexel.y)); // 0
                    uv3.Add(new Vector2(minTexel.x, maxTexel.y)); // 1
                    uv3.Add(new Vector2(maxTexel.x, maxTexel.y)); // 2
                    uv3.Add(new Vector2(maxTexel.x, minTexel.y)); // 3
                    break;
                case FaceType.ZPositive:
                case FaceType.ZNegative:
                    uv3.Add(new Vector2(minTexel.x, maxTexel.y)); // 0
                    uv3.Add(new Vector2(minTexel.x, minTexel.y)); // 1
                    uv3.Add(new Vector2(maxTexel.x, minTexel.y)); // 2
                    uv3.Add(new Vector2(maxTexel.x, maxTexel.y)); // 3
                    break;
            }
        }

        public object DeepCopy()
        {
            return new ColorVoxel(this);
        }

        public override bool Equals(object obj)
        {
            // If one is null, but not both
            // ReSharper disable once UseNullPropagation
            if(obj == null)
            {
                return false;
            }

            if(obj.GetType() != typeof(ColorVoxel))
            {
                return false;  
            }

            var voxelObj = (ColorVoxel)obj;
            
            return
                _hasColor == voxelObj._hasColor
                && Math.Abs(_colorR - voxelObj._colorR) < 0.00390625f // 1/256 
                && Math.Abs(_colorG - voxelObj._colorG) < 0.00390625f // 1/256 
                && Math.Abs(_colorB - voxelObj._colorB) < 0.00390625f // 1/256 
                && Math.Abs(_colorA - voxelObj._colorA) < 0.00390625f // 1/256 
                && Math.Abs(_metallic - voxelObj._metallic) < 0.00390625f // 1/256 
                && Math.Abs(_smoothness - voxelObj._smoothness) < 0.00390625f // 1/256 
                && Math.Abs(_emission - voxelObj._emission) < 0.00390625f; // 1/256 
        }

        public override int GetHashCode()
        {
            unchecked // overflow is fine, this will wrap 
            {
                int hash = 23;

                // ReSharper disable NonReadonlyMemberInGetHashCode
                hash = hash * 31 + _hasColor.GetHashCode();
                hash = hash * 31 + _colorR.GetHashCode();
                hash = hash * 31 + _colorG.GetHashCode();
                hash = hash * 31 + _colorB.GetHashCode();
                hash = hash * 31 + _colorA.GetHashCode();
                hash = hash * 31 + _metallic.GetHashCode();
                hash = hash * 31 + _smoothness.GetHashCode();
                hash = hash * 31 + _emission.GetHashCode();
                // ReSharper restore NonReadonlyMemberInGetHashCode

                return hash;
            }
        }
    }
}