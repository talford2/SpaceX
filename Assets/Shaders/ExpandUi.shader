﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Version of the additive shader without soft particles, stops close mesh from being visible through it

Shader "Custom/ExpandUi"
{
	Properties{
		_Color("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}

	_Expand("Expand", Range(0, 1)) = 1
}

Category {
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }

		SubShader {

		Pass{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				fixed4 _Color;

				struct vertexInput {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct vertexOutput {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				float4 _MainTex_ST;
				float _Expand;

				vertexOutput vert(vertexInput v)
				{
					vertexOutput o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					_Expand = 1 - _Expand;
					v.texcoord -= fixed2(_Expand, 0 - _Expand);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				sampler2D_float _CameraDepthTexture;

				fixed4 frag(vertexOutput i) : SV_Target
				{
					return 2.0f * i.color * _Color * tex2D(_MainTex, i.texcoord);
				}
			ENDCG
			}
		}
	}
}
