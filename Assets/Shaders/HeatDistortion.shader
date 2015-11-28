Shader "Custom/HeatDistortion" {
Properties {
	_BumpAmt  ("Distortion", range (0,128)) = 10
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_BumpMask ("Mask", 2D) = "white" {}

	_ScrollXSpeed ("X Scroll Speed", Range(-20, 20)) = -5
    _ScrollYSpeed ("Y Scroll Speed", Range(-20, 20)) = 0
}

Category {
	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent" "RenderType"="Opaque" }

	SubShader {

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		GrabPass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
		}
		
		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
//#pragma multi_compile_fog
#include "UnityCG.cginc"

fixed _ScrollXSpeed;
fixed _ScrollYSpeed;

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
	float2 texcoord1: TEXCOORD1;
};

struct v2f {
	float4 vertex : SV_POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	float2 uvmask : TEXCOORD2;
	//UNITY_FOG_COORDS(3)
};

float _BumpAmt;
float4 _BumpMap_ST;
float4 _BumpMask_ST;

v2f vert (appdata_t v)
{
	fixed xScrollValue = _ScrollXSpeed * _Time;
	fixed yScrollValue = _ScrollYSpeed * _Time;

	v2f o;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	
	v.texcoord += fixed2(xScrollValue, yScrollValue);


	o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);

	//o.uvmask = TRANSFORM_TEX( v.texcoord1, _BumpMask);

	//o.uvbump += _BumpMask;

	//UNITY_TRANSFER_FOG(o,o.vertex);
	return o;
}

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;
sampler2D _BumpMap;
sampler2D _BumpMask;

half4 frag (v2f i) : SV_Target
{
	// calculate perturbed coordinates



	//_BumpMap += _BumpMask;


	half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x & y without reconstructing the Z
	
	half2 bump2 = UnpackNormal(tex2D( _BumpMask, i.uvmask )).rg;
	
	float2 offset = (bump * bump2) * _BumpAmt * _GrabTexture_TexelSize.xy;
	//float2 offset = (0) * _BumpAmt * _GrabTexture_TexelSize.xy;


	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

	


	half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));

	
	//col += tex2D(_BumpMask, i.uvmask);

	//UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}
ENDCG
		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro

	/*SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}*/
}

}
