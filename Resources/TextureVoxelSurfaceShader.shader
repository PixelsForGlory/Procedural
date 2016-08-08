Shader "Procedural Voxel Mesh/Texture Voxel Surface Shader" {
    Properties 
    {
        _TextureMap("Texture Map (RGBA)", 2D) = "white" {}
        _WidthHeightIndex("Width Height Index", 2D) = "white" {}
        _TextureDetailIndex("Width Height Map (A)", 2D) = "white" {}
        _TextureSize("Texture Size", int) = 0
        _TileSize("Tile Size", int) = 0
    }
    
    SubShader 
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _TextureMap;

        int _TextureSize;
        int _TileSize;

        struct Input 
        {
            float2 uv_TextureMap;
            float2 uv2_WidthHeightIndex;
            float2 uv3_TextureDetailIndex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float tileSize = (float)_TileSize / (float)_TextureSize;
            
            // Convert from size / 256
            uint faceWidth = uint(floor(IN.uv2_WidthHeightIndex.x * 256.0 + 0.5));
            uint faceHeight = uint(floor(IN.uv2_WidthHeightIndex.y * 256.0 + 0.5));
            
            // Covert from index / 256
            uint textureIndex = uint(floor(IN.uv3_TextureDetailIndex.x * 256.0 +0.5));
            uint detailIndex = uint(floor(IN.uv3_TextureDetailIndex.y * 256.0 + 0.5));
            
            int tileCountUDirection = _TextureSize / _TileSize;
            float2 textureIndexUV = float2(float(textureIndex % tileCountUDirection), float((tileCountUDirection - 1) - textureIndex / tileCountUDirection)) * tileSize;
            float2 detailIndexUV = float2(float(detailIndex % tileCountUDirection), float((tileCountUDirection - 1) - detailIndex / tileCountUDirection)) * tileSize;
            
            float2 scaledUV = float2(IN.uv_TextureMap.x * float(faceWidth), IN.uv_TextureMap.y * float(faceHeight));
            
            // Just interested in the decimal part
            float2 currentUVBound = float2(float(scaledUV.x - (int)scaledUV.x), float(scaledUV.y - (int)scaledUV.y));

            float2 textureUV = currentUVBound * tileSize + textureIndexUV;
            
            float2 detailUV = currentUVBound * tileSize + detailIndexUV;
            float4 detailTextureSample = tex2D(_TextureMap, detailUV);
            if(detailIndex != 0 && detailTextureSample.a > 0)
            {
                o.Albedo = detailTextureSample;
            }
            else
            {
                o.Albedo = tex2D(_TextureMap, textureUV);
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
