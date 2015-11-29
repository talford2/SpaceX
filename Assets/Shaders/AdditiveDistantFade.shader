Shader "Custom/AdditiveDistantFade" {
	Properties{
		_Color("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}

		_A("A", float) = 0
		_B("B", float) = 30
		_C("C", float) = 200
		_D("D", float) = 80
	}

	SubShader
	{
		Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform float _A;
			uniform float _B;
			uniform float _C;
			uniform float _D;

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

				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);
				float4 vertexPos = mul(UNITY_MATRIX_MV, float4(0,0,0,0));

				float dist = distance(viewPos, vertexPos);

				float alpha = 0;

				if (dist > _A && dist < _D)
				{
					alpha = 1;

					// Close transition
					if (dist > _A && dist < _B)
					{
						alpha = (dist - _A) / (_B - _A);
					}

					// Far transition
					if (dist > _C && dist < _D)
					{
						alpha = 1 - (dist - _C) / (_D - _C);
					}
				}

				alpha = clamp(alpha, 0,1);

				o.color = float4(v.color.rgb, v.color.a * alpha);
				return o;
			}

			fixed4 frag(fragInput i) : SV_Target{
				return _Color * tex2D(_MainTex, i.texcoord) * i.color;
			}
			ENDCG
		}
	}
	Fallback "Particles/Alpha Blended"
}