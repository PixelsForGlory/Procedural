Shader "Procedural Voxel Mesh/Texture Voxel Surface Shader" {
	Properties 
	{
		_TextureMap("Texture Map (RGB)", 2D) = "white" {}
		_WidthHeightIndex("Width Height Index", 2D) = "white" {}
		_TextureDetailIndex("Width Height Map (A)", 2D) = "white" {}
		_TextureSize("Texture Size", int) = 0
		_TileSize("Tile Size", int) = 0
	}
	
	SubShader 
	{
		Tags
		{ 
			"RenderType" = "Opaque" 
		}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _TextureMap;

		uint _TextureSize;
		uint _TileSize;

		struct Input 
		{
			float2 uv_TextureMap;
			float2 uv2_WidthHeightIndex;
			float2 uv3_TextureDetailIndex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float tileSizeUV = (float)_TileSize / (float)_TextureSize;
			
			// Convert from size/256
			int faceWidth = round(IN.uv2_WidthHeightIndex.x * 256.0);
			int faceHeight = round(IN.uv2_WidthHeightIndex.y * 256.0);
			
			// Covert from index/256
			int textureIndex = round(IN.uv3_TextureDetailIndex.x * 256.0);
			int detailIndex = round(IN.uv3_TextureDetailIndex.y * 256.0);
			
			int tileNum = _TextureSize / _TileSize;
			float2 textureIndexUV = float2((uint)textureIndex % (uint)tileNum, (tileNum - 1) - (uint)textureIndex / (uint)tileNum) * tileSizeUV;
			float2 detailIndexUV = float2((uint)detailIndex % (uint)tileNum, (tileNum - 1) - (uint)detailIndex / (uint)tileNum) * tileSizeUV;
			
			float2 scaledUV = float2(IN.uv_TextureMap. x* faceWidth, IN.uv_TextureMap.y * faceHeight);
			float2 currentUVBound = float2((uint)scaledUV.x % faceWidth, (uint)scaledUV.y % faceHeight);

			float2 textureUV = (scaledUV - currentUVBound) * tileSizeUV + textureIndexUV;
			float2 detailUV = (scaledUV - currentUVBound) * tileSizeUV + detailIndexUV;

			if(tex2D(_TextureMap, detailUV).a > 0.0)
			{
				o.Albedo = tex2D(_TextureMap, detailUV).rgb;
			}
			else
			{
				o.Albedo = tex2D(_TextureMap, textureUV).rgb;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
