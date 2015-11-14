// Version of the additive shader without soft particles, stops close mesh from being visible through it

Shader "Custom/ThrusterSpin"
{
	Properties{
		_Color("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}

		_SpinSpeed("Spin Speed", Range(0, 50)) = 0.5
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

				fixed _SpinSpeed;

				fixed4 _Color;
				sampler2D _MainTex;

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
					vertexOutput o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;

					fixed spin = _SpinSpeed * _Time;

					float sinX = sin(spin);
					float cosX = cos(spin);
					float sinY = sin(spin);

					float2x2 clockwiseMatrix = float2x2(cosX, sinX, -sinY, cosX);
					v.texcoord.xy = mul(v.texcoord.xy - fixed2(0.5, 0.5), clockwiseMatrix);
					v.texcoord += fixed2(0.5, 0.5);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

					float2x2 antiClockwiseMatrix = float2x2(cosX, -sinX, sinY, cosX);
					v.texcoord2.xy = mul(v.texcoord2.xy - fixed2(0.5, 0.5), antiClockwiseMatrix);
					v.texcoord2 += fixed2(0.5, 0.5);
					o.texcoord2 = TRANSFORM_TEX(v.texcoord2, _MainTex);

					return o;
				}

				sampler2D_float _CameraDepthTexture;

				fixed4 frag(vertexOutput i) : SV_Target
				{
					return i.color * tex2D(_MainTex, i.texcoord) * 0.5 + i.color * tex2D(_MainTex, i.texcoord2) * 0.5;
				}
			ENDCG
			}
		}
	}
}
