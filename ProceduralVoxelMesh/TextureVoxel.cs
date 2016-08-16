// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    /// <summary>
    /// Represents the texture setup for a cube.  Each face can be mapped to a texture.
    /// 
    /// Example mapping of texture
    ///  ___________________
    /// |    |    |    |    |
    /// |  0 |  1 |  2 |  3 |
    /// |____|____|____|____|
    /// |    |    |    |    |
    /// |  4 |  5 |  6 |  7 |
    /// |____|____|____|____|
    /// |    |    |    |    |
    /// |  8 |  9 | 10 | 11 |
    /// |____|____|____|____|
    /// |    |    |    |    |
    /// | 12 | 13 | 14 | 15 |
    /// |____|____|____|____|
    /// </summary>
    public struct TextureVoxelSetup
    {
        public int XPositiveTextureIndex;
        public int XNegativeTextureIndex;
        public int YPositiveTextureIndex;
        public int YNegativeTextureIndex;
        public int ZPositiveTextureIndex;
        public int ZNegativeTextureIndex;
    }

    /// <summary>
    /// Represents a single voxel.
    /// </summary>
    [Serializable]
    public struct TextureVoxel : IVoxel
    {
        /// <summary>
        /// List of texture setups that texture voxels can be set to
        /// </summary>
        public static List<TextureVoxelSetup> TextureVoxelMap = new List<TextureVoxelSetup>();

        /// <summary>
        /// Serializable value of empty
        /// </summary>
        [SerializeField]
        private bool _hasTexture;

        /// <summary>
        /// Is the voxel empty or not
        /// </summary>
        public bool Empty
        {
            get { return !_hasTexture; }

            set
            {
                if(value == false)
                {
                    throw new InvalidOperationException("Set voxel to be non-empty via texture map index property");
                }

                _hasTexture = false;
                _textureMapIndex = -1;
                _detailMapIndex = -1;
            }
        }
        
        [SerializeField]
        private int _textureMapIndex;

        /// <summary>
        /// Main texture index to map to TextureVoxelMap
        /// </summary>
        public int TextureMapIndex
        {
            get
            {
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot get the texture index of an empty voxel");
                }
                return _textureMapIndex;
            }

            set
            {
                if(value < 0)
                {
                    throw new InvalidOperationException("Texture map index must be positive");
                }

                if(value >= TextureVoxelMap.Count)
                {
                    throw new InvalidOperationException("Texture voxel index cannot be greater than number of elements in texture map");
                }

                _hasTexture = true;
                _textureMapIndex = value;
            }
        }

        [SerializeField]
        private int _detailMapIndex;

        /// <summary>
        /// Secondary detail texture to map to TextureVoxelMap.  Will override main texture
        /// </summary>
        public int DetailMapIndex
        {
            get
            {
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot get the detail index of an empty voxel");
                }
                return _detailMapIndex;
            }

            set
            {
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot set the detail index of an empty voxel");
                }

                if(value < 0)
                {
                    throw new InvalidOperationException("Detail map index must be positive");
                }

                if(value >= TextureVoxelMap.Count)
                {
                    throw new InvalidOperationException("Detail voxel index cannot be greater than number of elements in texture map");
                }

                _hasTexture = true;
                _detailMapIndex = value;
            }

        }

        [SerializeField]
        private float _alphaLevel;

        public float AlphaLevel
        {
            get
            {
                if(Empty)
                {
                    throw new InvalidOperationException("Cannot get the alpha level of an empty voxel");
                }
                return _alphaLevel;
            }

            set
            {
                if(value < 0f || value > 1f)
                {
                    throw new InvalidOperationException("Alpha value must be between [0f, 1f]");
                }

                _alphaLevel = value;
            }
        }


        /// <summary>
        /// Copy textured voxel to new instance
        /// </summary>
        /// <param name="textureVoxel"></param>
        private TextureVoxel(TextureVoxel textureVoxel)
        {
            _hasTexture = textureVoxel._hasTexture;
            _textureMapIndex = textureVoxel._textureMapIndex;
            _detailMapIndex = textureVoxel._detailMapIndex;
            _alphaLevel = textureVoxel._alphaLevel;
        }

        /// <summary>
        /// Single textured voxel
        /// </summary>
        /// <param name="textureMapIndex">Main texture map index</param>
        /// <param name="alphaLevel">Optional alphaLevel of the voxel</param>
        public TextureVoxel(int textureMapIndex, float alphaLevel = 1f)
        {
            if (textureMapIndex < 0)
            {
                throw new ArgumentException("Cannot create a textured voxel without positive texture map index", nameof(textureMapIndex));
            }

            if (textureMapIndex >= TextureVoxelMap.Count)
            {
                throw new ArgumentException("Texture voxel index cannot be greater than number of elements in texture map", nameof(textureMapIndex));
            }

            _hasTexture = true;
            _textureMapIndex = textureMapIndex;
            _detailMapIndex = -1;
            _alphaLevel = alphaLevel;
        }

        /// <summary>
        /// Single textured voxel
        /// </summary>
        /// <param name="textureMapIndex">Main texture map index</param>
        /// <param name="detailMapIndex">Detail map index</param>
        /// <param name="alphaLevel">Optional alphaLevel of the voxel</param>
        public TextureVoxel(int textureMapIndex, int detailMapIndex, float alphaLevel = 1f)
        {
            if(textureMapIndex < 0)
            {
                throw new ArgumentException("Cannot create a textured voxel without positive texture map index", nameof(textureMapIndex));
            }

            if(textureMapIndex >= TextureVoxelMap.Count)
            {
                throw new ArgumentException("Texture voxel index cannot be greater than number of elements in texture map", nameof(textureMapIndex));
            }

            if(detailMapIndex < 0)
            {
                throw new ArgumentException("Cannot create a textured voxel with a non-positive detail map index", nameof(detailMapIndex));
            }

            if(detailMapIndex >= TextureVoxelMap.Count)
            {
                throw new ArgumentException("Detail voxel index cannot be greater than number of elements in texture map", nameof(detailMapIndex));
            }

            _hasTexture = true;
            _textureMapIndex = textureMapIndex;
            _detailMapIndex = detailMapIndex;
            _alphaLevel = alphaLevel;
        }

        public void AddVoxelToMesh(FaceType faceType, int width, int height, List<Color> colors, List<Vector2> uv, List<Vector2> uv2, List<Vector2> uv3)
        {
            // Colors
            colors.Add(new Color(0f, 0f, 0f, _alphaLevel));  // 0
            colors.Add(new Color(0f, 0f, 0f, _alphaLevel));  // 1
            colors.Add(new Color(0f, 0f, 0f, _alphaLevel));  // 2
            colors.Add(new Color(0f, 0f, 0f, _alphaLevel));  // 3

            // TEXCOORD0/UV1
            // Map 0 -> 1 UV
            var min = new Vector2(0.0f, 0.0f);
            var max = new Vector2(1.0f, 1.0f);

            switch(faceType)
            {
                case FaceType.XPositive:
                    uv.Add(new Vector2(min.x, min.y)); // 0
                    uv.Add(new Vector2(min.x, max.y)); // 1
                    uv.Add(new Vector2(max.x, max.y)); // 2
                    uv.Add(new Vector2(max.x, min.y)); // 3
                    break;
                case FaceType.XNegative:
                    uv.Add(new Vector2(max.x, min.y)); // 0
                    uv.Add(new Vector2(max.x, max.y)); // 1
                    uv.Add(new Vector2(min.x, max.y)); // 2
                    uv.Add(new Vector2(min.x, min.y)); // 3
                    break;
                case FaceType.YPositive:
                    uv.Add(new Vector2(min.x, min.y)); // 0
                    uv.Add(new Vector2(min.x, max.y)); // 1
                    uv.Add(new Vector2(max.x, max.y)); // 2
                    uv.Add(new Vector2(max.x, min.y)); // 3
                    break;
                case FaceType.YNegative:
                    uv.Add(new Vector2(min.x, min.y)); // 0
                    uv.Add(new Vector2(min.x, max.y)); // 1
                    uv.Add(new Vector2(max.x, max.y)); // 2
                    uv.Add(new Vector2(max.x, min.y)); // 3
                    break;
                case FaceType.ZPositive:
                    uv.Add(new Vector2(max.x, min.y)); // 0
                    uv.Add(new Vector2(min.x, min.y)); // 1
                    uv.Add(new Vector2(min.x, max.y)); // 2
                    uv.Add(new Vector2(max.x, max.y)); // 3
                    break;
                case FaceType.ZNegative:
                    uv.Add(new Vector2(min.x, min.y)); // 0
                    uv.Add(new Vector2(max.x, min.y)); // 1
                    uv.Add(new Vector2(max.x, max.y)); // 2
                    uv.Add(new Vector2(min.x, max.y)); // 3
                    break;
                default:
                    throw new Exception("Unkown face type");
            }

            // TEXCOORD1/UV2
            // Map width and height
            Vector2 value;
            switch(faceType)
            {
                case FaceType.XPositive:
                    value = new Vector2(height / 256.0f, width / 256.0f);
                    break;
                case FaceType.XNegative:
                    value = new Vector2(height / 256.0f, width / 256.0f);
                    break;
                case FaceType.YPositive:
                    value = new Vector2(height / 256.0f, width / 256.0f);
                    break;
                case FaceType.YNegative:
                    value = new Vector2(height / 256.0f, width / 256.0f);
                    break;
                case FaceType.ZPositive:
                    value = new Vector2(width / 256.0f, height / 256.0f);
                    break;
                case FaceType.ZNegative:
                    value = new Vector2(width / 256.0f, height / 256.0f);
                    break;
                default:
                    throw new Exception("Unkown face type");
            }
            uv2.Add(value); // 0
            uv2.Add(value); // 1
            uv2.Add(value); // 2
            uv2.Add(value); // 3

            // TEXCOORD2/UV3
            // Map texture map index and detail map index
            switch(faceType)
            {
                case FaceType.XPositive:
                    value = new Vector2(TextureVoxelMap[TextureMapIndex].XPositiveTextureIndex / 256.0f, DetailMapIndex == -1 ? 0.0f : (TextureVoxelMap[DetailMapIndex].XPositiveTextureIndex / 256.0f));
                    break;
                case FaceType.XNegative:
                    value = new Vector2(TextureVoxelMap[TextureMapIndex].XNegativeTextureIndex / 256.0f, DetailMapIndex == -1 ? 0.0f : (TextureVoxelMap[DetailMapIndex].XNegativeTextureIndex / 256.0f));
                    break;
                case FaceType.YPositive:
                    value = new Vector2(TextureVoxelMap[TextureMapIndex].YPositiveTextureIndex / 256.0f, DetailMapIndex == -1 ? 0.0f : (TextureVoxelMap[DetailMapIndex].YPositiveTextureIndex / 256.0f));
                    break;
                case FaceType.YNegative:
                    value = new Vector2(TextureVoxelMap[TextureMapIndex].YNegativeTextureIndex / 256.0f, DetailMapIndex == -1 ? 0.0f : (TextureVoxelMap[DetailMapIndex].YNegativeTextureIndex / 256.0f));
                    break;
                case FaceType.ZPositive:
                    value = new Vector2(TextureVoxelMap[TextureMapIndex].ZPositiveTextureIndex / 256.0f, DetailMapIndex == -1 ? 0.0f : (TextureVoxelMap[DetailMapIndex].ZPositiveTextureIndex / 256.0f));
                    break;
                case FaceType.ZNegative:
                    value = new Vector2(TextureVoxelMap[TextureMapIndex].ZNegativeTextureIndex / 256.0f, DetailMapIndex == -1 ? 0.0f : (TextureVoxelMap[DetailMapIndex].ZNegativeTextureIndex / 256.0f));
                    break;
                default:
                    throw new Exception("Unkown face type");
            }
            
            uv3.Add(value); // 0
            uv3.Add(value); // 1
            uv3.Add(value); // 2
            uv3.Add(value); // 3   
        }

        public object DeepCopy()
        {
            return new TextureVoxel(this);
        }

        public override bool Equals(object obj)
        {
            // If one is null, but not both
            // ReSharper disable once UseNullPropagation
            if(obj == null)
            {
                return false;
            }

            if(obj.GetType() != typeof(TextureVoxel))
            {
                return false;
            }

            var texVoxel = (TextureVoxel)obj;

            return
                _hasTexture == texVoxel._hasTexture
                && _textureMapIndex == texVoxel._textureMapIndex
                && _detailMapIndex == texVoxel._detailMapIndex
                && Mathf.Abs(_alphaLevel - texVoxel._alphaLevel) < 0.00000001f;
        }

        public override int GetHashCode()
        {
            unchecked // overflow is fine, this will wrap 
            {
                int hash = 23;

                // ReSharper disable NonReadonlyMemberInGetHashCode
                hash = hash * 31 + _hasTexture.GetHashCode();
                hash = hash * 31 + _textureMapIndex.GetHashCode();
                hash = hash * 31 + _detailMapIndex.GetHashCode();
                hash = hash * 31 + _alphaLevel.GetHashCode();
                // ReSharper restore NonReadonlyMemberInGetHashCode

                return hash;
            }
        }
    }
}