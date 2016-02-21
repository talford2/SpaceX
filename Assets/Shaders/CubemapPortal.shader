Shader "Custom/CubemapPortal"
{
	Properties
	{
		_Cube("Cubemap", CUBE) = "" {}
		//_BumpMap("Bumpmap", 2D) = "bump" {}
	}
	SubShader{
		Cull Off
		CGPROGRAM
			#pragma surface surf Lambert
			struct Input {
				//float2 uv_BumpMap;
				float3 worldRefl;
				INTERNAL_DATA
			};

			sampler2D _MainTex;
			sampler2D _BumpMap;
			samplerCUBE _Cube;
			void surf(Input IN, inout SurfaceOutput o) {
				//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * 0.2;
				o.Emission = texCUBE(_Cube, WorldReflectionVector(IN, o.Normal)).rgb;
			}
		ENDCG
	}
	Fallback "Diffuse"
}