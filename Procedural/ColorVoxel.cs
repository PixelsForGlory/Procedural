// Copyright (C) afuzzyllama. All rights reserved
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using UnityEngine;
using PixelsForGlory.Extensions;

namespace PixelsForGlory.Procedural
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
        [SerializeField] private bool _hasVoxel;

        /// <summary>
        /// Is the voxel empty or not
        /// </summary>
        public bool Empty
        {
            get { return !_hasVoxel; }
            set
            {
                if (value == false)
                {
                    throw new InvalidOperationException("Set voxel to be non-empty via Color property");
                }

                _hasVoxel = false;
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
        [SerializeField] private float _colorR;

        /// <summary>
        /// G part of color, broken up for serialization purposes
        /// </summary>
        [SerializeField] private float _colorG;

        /// <summary>
        /// B part of color, broken up for serialization purposes
        /// </summary>
        [SerializeField] private float _colorB;

        /// <summary>
        /// A part of color, broken up for serialization purposes
        /// </summary>
        [SerializeField] private float _colorA;

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
                _hasVoxel = true;
                _colorR = value.r;
                _colorG = value.g;
                _colorB = value.b;
                _colorA = value.a;
            }
        }

        [SerializeField] private float _metallic;

        /// <summary>
        /// Metallic value of the voxel
        /// </summary>
        public float Metallic
        {
            get
            {
                if (Empty)
                {
                    throw new InvalidOperationException("Cannot get the metallic of an empty voxel");
                }
                return _metallic;
            }
            set
            {
                if (Empty)
                {
                    throw new InvalidOperationException("Cannot set the metallic value of an empty voxel");
                }

                if (value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException("Metallic value cannot be less than 0.0f or greater than 1.0f");
                }
                _metallic = value;
            }
        }

        [SerializeField] private float _smoothness;

        /// <summary>
        /// Smoothness value of the voxel
        /// </summary>
        public float Smoothness
        {
            get
            {
                if (Empty)
                {
                    throw new InvalidOperationException("Cannot get the smoothness of an empty voxel");
                }
                return _smoothness;
            }
            set
            {
                if (Empty)
                {
                    throw new InvalidOperationException("Cannot set the smoothness value of an empty voxel");
                }

                if (value < 0.0f || value > 1.0f)
                {
                    throw new InvalidOperationException(
                        "Smoothness value cannot be less than 0.0f or greater than 1.0f");
                }
                _smoothness = value;
            }
        }

        [SerializeField] private float _emission;

        /// <summary>
        /// Emissive value of the voxel
        /// </summary>
        public float Emission
        {
            get
            {
                if (Empty)
                {
                    throw new InvalidOperationException("Cannot get the emission of an empty voxel");
                }
                return _emission;
            }
            set
            {

                if (Empty)
                {
                    throw new InvalidOperationException("Cannot set the emission value of an empty voxel");
                }

                if (value < 0.0f || value > 1.0f)
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
        public ColorVoxel(ColorVoxel colorVoxel)
        {
            _hasVoxel = colorVoxel._hasVoxel;
            _colorR = colorVoxel._colorR;
            _colorG = colorVoxel._colorG;
            _colorB = colorVoxel._colorB;
            _colorA = colorVoxel._colorA;
            _metallic = colorVoxel.Metallic;
            _smoothness = colorVoxel.Smoothness;
            _emission = colorVoxel.Emission;
        }

        /// <summary>
        /// Single colored voxel
        /// </summary>
        /// <param name="color">If the voxel is not empty, the color of this voxel</param>
        /// <param name="metallic">Metallic value for this voxel</param>
        /// <param name="smoothness">Smoothness value for this voxel</param>
        /// <param name="emission">Emission value for this voxel</param>
        public ColorVoxel(Color color, float metallic = 0.0f, float smoothness = 0.0f, float emission = 0.0f) : this()
        {
            _hasVoxel = true;
            _colorR = color.r;
            _colorG = color.g;
            _colorB = color.b;
            _colorA = color.a;
            Metallic = metallic;
            Smoothness = smoothness;
            Emission = emission;
        }

        public bool AddVoxelDataToMesh(FaceType faceType, int width, int height, List<Color> colors, List<Vector2> uv,
            List<Vector2> uv2, List<Vector2> uv3)
        {
            // Colors
            colors.Add(Color); // 0
            colors.Add(Color); // 1
            colors.Add(Color); // 2
            colors.Add(Color); // 3

            const float texelSize = 1.0f / 32.0f;
            // TEXCOORD0/UV1
            // Map to metallic
            int metallicIndex = Mathf.RoundToInt(Metallic * 255.0f);
            int textureX = (metallicIndex % 16) * 2;
            int textureY = (metallicIndex / 16) * 2;

            var minTexel = new Vector2(
                textureX * texelSize // Get to metallic location
                + texelSize / 2.0f, // Move half a texel length in
                textureY * texelSize
                + texelSize / 2.0f);

            var maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

            switch (faceType)
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
                textureX * texelSize // Get to metallic location
                + texelSize / 2.0f, // Move half a texel length in
                textureY * texelSize
                + texelSize / 2.0f);

            maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

            switch (faceType)
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
                textureX * texelSize // Get to metallic location
                + texelSize / 2.0f, // Move half a texel length in
                textureY * texelSize
                + texelSize / 2.0f);

            maxTexel = new Vector2(minTexel.x + texelSize, minTexel.y + texelSize);

            switch (faceType)
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

            return true;
        }

        public bool Equals(IVoxel other)
        {
            if (ReferenceEquals(null, other)) return false;
            var isColorVoxel = other is ColorVoxel;

            return isColorVoxel && Equals((ColorVoxel)other);
        }

        public bool Equals(ColorVoxel other)
        {
            return
                Empty == other.Empty &&
                Color == other.Color &&
                Math.Abs(Metallic - other.Metallic) < FloatExtensions.SmallNumber &&
                Math.Abs(Smoothness - other.Smoothness) < FloatExtensions.SmallNumber &&
                Math.Abs(Emission - other.Emission) < FloatExtensions.SmallNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var isColorVoxel = obj is ColorVoxel;

            if (isColorVoxel)
            {
                return Equals((ColorVoxel)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Empty.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Metallic.GetHashCode();
                hashCode = (hashCode * 397) ^ Smoothness.GetHashCode();
                hashCode = (hashCode * 397) ^ Emission.GetHashCode();
                return hashCode;
            }
        }
    }
}