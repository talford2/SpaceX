// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Magic" {
	Properties{
	}
	Category{
	Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }

	SubShader{

	GrabPass{
	Name "BASE"
	Tags{ "LightMode" = "Always" }
	}

	Pass{
	Name "BASE"
	Tags{ "LightMode" = "Always" }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
	float2 texcoord1: TEXCOORD1;
};

struct v2f {
	float4 vertex : SV_POSITION;
	float4 uvgrab : TEXCOORD0;
};

v2f vert(appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	return o;
}

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;

half4 frag(v2f i) : SV_Target
{
	half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
	return col;
}
	ENDCG
}
}
	}

}
