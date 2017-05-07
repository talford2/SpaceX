// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Version of the additive shader without soft particles, stops close mesh from being visible through it

Shader "Custom/Burning" 
 {
Properties {
	_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Mask", 2D) = "white" {}
	_Flames ("Flames", 2D) = "white" {}

	_ScrollXSpeed ("X Scroll Speed", Range(-20, 20)) = -5
    _ScrollYSpeed ("Y Scroll Speed", Range(-20, 20)) = 0
}


Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Flames;
			fixed4 _Color;

			fixed _ScrollXSpeed;
			fixed _ScrollYSpeed;
			
			struct vertexInput {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 flamecoord : TEXCOORD0;
				float2 maskcoord : TEXCOORD1;
			};

			struct vertexOutput {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 flamecoord : TEXCOORD0;
				float2 maskcoord : TEXCOORD1;
			};
			

			float4 _MainTex_ST;
			float4 _Flames_ST;

			vertexOutput vert (vertexInput v)
			{
				fixed xScrollValue = _ScrollXSpeed * _Time;
				fixed yScrollValue = _ScrollYSpeed * _Time;

				vertexOutput o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;


				v.maskcoord += fixed2(xScrollValue, yScrollValue);

				o.maskcoord = TRANSFORM_TEX(v.maskcoord, _MainTex);

				
				o.flamecoord = TRANSFORM_TEX(v.flamecoord,_Flames);
				
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			
			fixed4 frag (vertexOutput i) : SV_Target
			{
				return 2.0f * i.color * _Color * tex2D(_MainTex, i.flamecoord) * tex2D(_Flames, i.maskcoord);
			}
			ENDCG 
		}
	}	
}
}
