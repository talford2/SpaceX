Shader "Custom/AdditiveMaskSlide"
{
	Properties{
		_Color("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_ThrustTex("Thrust Effect", 2D) = "black" {}
		_ScrollXSpeed("X Scroll Speed", Range(0, 50)) = 5
	}
	Category
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }

		SubShader {
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles

				#include "UnityCG.cginc"

				fixed _ScrollXSpeed;
				fixed4 _Color;
				sampler2D _MainTex;
				sampler2D _ThrustTex;

				struct vertexInput {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
				};

				struct vertexOutput {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
				};

				float4 _MainTex_ST;
				float4 _ThrustTex_ST;

				vertexOutput vert(vertexInput v)
				{
					fixed xScrollValue = _ScrollXSpeed * _Time;

					vertexOutput o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color * _Color;

					//fixed flameOsselate = 50 * _Time;
					//half OsselateAmount = 0.03;
					//v.texcoord -= fixed2(sin(flameOsselate)* OsselateAmount - OsselateAmount, 0);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

					// Scrolling texture
					v.texcoord2 -= fixed2(xScrollValue, 0);
					o.texcoord2 = TRANSFORM_TEX(v.texcoord2, _ThrustTex);
					return o;
				}

				sampler2D_float _CameraDepthTexture;

				fixed4 frag(vertexOutput i) : SV_Target
				{
					half4 baseTex = tex2D(_MainTex, i.texcoord);
					return (i.color * baseTex + (baseTex * tex2D(_ThrustTex, i.texcoord2))) * i.color.a;
				}
				ENDCG
			}
		}
	}
}
