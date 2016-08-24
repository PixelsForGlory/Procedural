// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
Shader "Procedural Voxel Mesh/Billboard Cross Transparent Cutout Surface Shader" {
    Properties 
    {
        _TextureMap("Texture Map (RGBA)", 2D) = "white" {}
        _TextureIndex("Width Height Map (A)", 2D) = "white" {}
        _TextureSize("Texture Size", int) = 0
        _TileSize("Tile Size", int) = 0
        _Cutoff("Cutoff Alpha", float) = 0.5
    }
    
    SubShader 
    {
        Tags 
        { 
            "Queue" = "Geometry"
            "IgnoreProjector"="True"
            "RenderType" = "TransparentCutout" 
        }
        LOD 300

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _TextureMap;

        int _TextureSize;
        int _TileSize;

        struct Input 
        {
            float2 uv_TextureMap;
            float2 uv2_TextureIndex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float tileSize = (float)_TileSize / (float)_TextureSize;

            // Covert from index / 256
            uint textureIndex = uint(floor(IN.uv2_TextureIndex.x * 256.0 + 0.5));
            
            uint tileCountUDirection = uint(_TextureSize) / uint(_TileSize);
            float2 textureIndexUV = float2(float(textureIndex % tileCountUDirection), float((tileCountUDirection - 1) - textureIndex / tileCountUDirection)) * tileSize;
            
            float2 clampedUV = IN.uv_TextureMap;

            float texelSize = 1.0 / (float)_TextureSize;
            if(clampedUV.x < texelSize / 2.0)
            {
                clampedUV.x = texelSize / 2.0;
            }

            if(clampedUV.y < texelSize / 2.0)
            {
                clampedUV.y = texelSize / 2.0;
            }

            float2 textureUV = clampedUV * tileSize + textureIndexUV;
            float4 mainTextureSample = tex2D(_TextureMap, textureUV);
            
            o.Albedo = mainTextureSample.rgb;
            o.Alpha = mainTextureSample.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
