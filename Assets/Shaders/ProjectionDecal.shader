// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Custom/ProjectionDecal" {
	Properties{
		_ShadowTex("Projected Image", 2D) = "white" {}
		_Alpha("Alpha", Range(0,1)) = 1.0
	}
	SubShader{
			Tags{ "Queue" = "Transparent" }
		Pass {
			Blend One One

			//ColorMask RGB
			// add color of _ShadowTex to the color in the framebuffer 
			ZWrite Off // don't change depths
			//Offset - 1, -1 // avoid depth fighting

			//Blend DstColor One
			Offset -1, -1

		CGPROGRAM

	#pragma vertex vert  
	#pragma fragment frag 

//#pragma multi_compile_fog
//#include "UnityCG.cginc"

			// User-specified properties
			uniform sampler2D _ShadowTex;

		// Projector-specific uniforms
		uniform float4x4 unity_Projector; // transformation matrix 
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

			output.posProj = mul(unity_Projector, input.vertex);
			output.pos = UnityObjectToClipPos(input.vertex);
			return output;
		}


		float _Alpha;

		float4 frag(vertexOutput input) : COLOR
		{
			if (input.posProj.w > 0.0) // in front of projector?
			{
				return tex2D(_ShadowTex ,
					input.posProj.xy / input.posProj.w) * _Alpha;
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