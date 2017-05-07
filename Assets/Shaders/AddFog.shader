// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AddFog"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Near("Near", Float) = 10
		_Far("Far", Float) = 100
	}

		//Blend SrcAlpha OneMinusSrcAlpha
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		Offset -1, -1

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color: COLOR;
			};

			float4 _Color;
			float _Near;
			float _Far;

			v2f vert(appdata v)
			{


				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);
				float4 vertexPos = mul(UNITY_MATRIX_MV, float4(0, 0, 0, 0));

				float dist = distance(viewPos, vertexPos);
				float frac = clamp((dist - _Near) / (_Far - _Near), 0, 1);

				o.color = _Color;
				o.color.a =  frac;

				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{

				//fixed4 color = fixed4(1,1,1,0);
				//return _Color;

				return i.color;
			}
			ENDCG
		}
	}
}
