Shader "Custom/DistantFade" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_FadeColor("Fade Color", Color) = (0,0,0,0)

		_Near("Near", Float) = 10
		_Far("Far", Float) = 100
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			Pass{
			//Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _FadeColor;

			float _Near;
			float _Far;

			struct vertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float fadeFraction : float;
				float3 normal : NORMAL;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.uv = TRANSFORM_TEX(input.uv, _MainTex);

				output.normal = input.normal;

				float4 viewPos = mul(UNITY_MATRIX_MV, input.vertex);
				float4 vertexPos = mul(UNITY_MATRIX_MV, float4(0, 0, 0, 0));
				float dist = distance(viewPos, vertexPos);

				float frac = clamp((dist - _Near) / (_Far - _Near), 0, 1);
				output.fadeFraction = frac;

				return output;
			}
			float4 frag(vertexOutput input) : COLOR
			{
				fixed4 litText = tex2D(_MainTex, input.uv);
				
				// Applying simple lighting
				litText *= dot(_WorldSpaceLightPos0, input.normal);

				fixed4 color = litText * (1 - input.fadeFraction) + _FadeColor * input.fadeFraction;
				return color;
			}
			ENDCG
		}
		}
}