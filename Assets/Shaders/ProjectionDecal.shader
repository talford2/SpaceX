Shader "Custom/ProjectionDecal" {
	Properties{
		_ShadowTex("Projected Image", 2D) = "white" {}		
	_Alpha("Alpha", Range(0,1)) = 1.0
	}
		SubShader{
		Pass{
		Blend One One
		// add color of _ShadowTex to the color in the framebuffer 
		ZWrite Off // don't change depths
		//Offset - 1, -1 // avoid depth fighting



		CGPROGRAM

#pragma vertex vert  
#pragma fragment frag 

		// User-specified properties
		uniform sampler2D _ShadowTex;

	// Projector-specific uniforms
	uniform float4x4 _Projector; // transformation matrix 
								 // from object space to projector space 

	struct vertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 posProj : TEXCOORD0;
		// position in projector space
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		output.posProj = mul(_Projector, input.vertex);
		output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
		return output;
	}


	float _Alpha;

	float4 frag(vertexOutput input) : COLOR
	{
		if (input.posProj.w > 0.0) // in front of projector?
		{
			return tex2D(_ShadowTex ,
				input.posProj.xy / input.posProj.w) * _Alpha;
			// alternatively: return tex2Dproj(  
			//    _ShadowTex, input.posProj);
		}
		else // behind projector
		{
			return float4(0.0, 0.0, 0.0, 0.0);
		}
	}

		ENDCG
	}
	}
		Fallback "Projector/Light"
}


//Shader "Custom/ProjectionDecal" {
//		Properties {
//		_ShadowTex ("Cookie", 2D) = "gray" {}
//		_FalloffTex ("FallOff", 2D) = "white" {}
//		_Alpha ("Alpha", Range(0,1)) = 1.0
//	}
//	Subshader {
//		Tags {"Queue"="Transparent"}
//		Pass {
//			ZWrite Off
//			ColorMask RGB
//			Blend DstColor Zero
//			Offset -1, -1
//
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#pragma multi_compile_fog
//			#include "UnityCG.cginc"
//			
//			struct v2f {
//				float4 uvShadow : TEXCOORD0;
//				float4 uvFalloff : TEXCOORD1;
//				UNITY_FOG_COORDS(2)
//				float4 pos : SV_POSITION;
//			};
//			
//			float4x4 _Projector;
//			float4x4 _ProjectorClip;
//			
//			v2f vert (float4 vertex : POSITION)
//			{
//				v2f o;
//				o.pos = mul (UNITY_MATRIX_MVP, vertex);
//				o.uvShadow = mul (_Projector, vertex);
//				o.uvFalloff = mul (_ProjectorClip, vertex);
//				UNITY_TRANSFER_FOG(o,o.pos);
//				return o;
//			}
//			
//			sampler2D _ShadowTex;
//			sampler2D _FalloffTex;
//			float _Alpha;
//
//			fixed4 frag (v2f i) : SV_Target
//			{
//				fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
//				texS.a = texS.a;
//
//				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
//				fixed4 res = lerp(fixed4(1,1,1,0), texS, texF.a * texS.a * _Alpha);
//
//				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(1,1,1,1));
//				return res;
//			}
//			ENDCG
//		}
//	}
//}