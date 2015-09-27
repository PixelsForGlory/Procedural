Shader "Procedural Voxel Mesh/Voxel Surface Shader" {
	Properties {
		_MetallicMap("Metallic Map (A)", 2D) = "white" {}
		_SmoothnessMap("Smoothness Map (A)", 2D) = "white" {}
		_EmissionMap("Emission Map (A)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MetallicMap;
		sampler2D _SmoothnessMap;
		sampler2D _EmissionMap;


		struct Input {
			float4 color : COLOR;
			float2 uv_MetallicMap;
			float2 uv2_SmoothnessMap;
			float2 uv3_EmissionMap;
		};
				
		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = IN.color;

			fixed4 map;
								
			map = tex2D(_MetallicMap, IN.uv_MetallicMap);
			o.Metallic = map.r;

			map = tex2D(_SmoothnessMap, IN.uv2_SmoothnessMap);
			o.Smoothness = map.r;
			
			map = tex2D(_EmissionMap, IN.uv3_EmissionMap);
			o.Emission = IN.color * map.r;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
