Shader "Custom/AdditiveDistantFade" {
	Properties{
		_Color("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_FadeDistance("Fade Start Distance", float) = 0.5
	}

		SubShader{
			Tags {"Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			Blend SrcAlpha One
			AlphaTest Greater .01
			ColorMask RGB
			Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }

			//Blend SrcAlpha OneMinusSrcAlpha
			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 3.0
					#include "UnityCG.cginc"

					sampler2D _MainTex;
					uniform float _FadeDistance;
					fixed4 _Color;

					struct vertInput {
						float4 vertex : POSITION;
						float4 texcoord : TEXCOORD0;
						float4 color : COLOR;
					};



					struct fragInput {
						float4 pos : SV_POSITION;
						float2	texcoord : TEXCOORD0;
						float4 color : COLOR;
					};

					float4 _MainTex_ST;

					fragInput vert(vertInput v) {
						fragInput o;
						o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
						//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

						//o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

						float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);
						float4 vertexPos = mul(UNITY_MATRIX_MV, float4(0,0,0,0));

						float alpha = 1 - (distance(viewPos, vertexPos) / _FadeDistance);

						alpha = min(alpha, 1);
						alpha = max(alpha, 0);

						o.color = float4(v.color.rgb, v.color.a * alpha);
						return o;
					}

					/*float4 frag(fragInput i) : COLOR {
						half4 texcol = tex2D(_MainTex, i.texcoord);
						return texcol * i.color;
					}*/

					fixed4 frag(fragInput i) : SV_Target {
						return _Color * tex2D(_MainTex, i.texcoord) * i.color;
					//return texcol * i.color;
					}
			ENDCG
		}
		}

			Fallback "Particles/Alpha Blended"
}