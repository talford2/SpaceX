Shader "Custom/AdditiveDistanceAlpha" {
	Properties{
		_Position("Collision", Vector) = (-1, -1, -1, -1)
		_MaxDistance("Effect Size", float) = 40
		_ShieldColor("Color (RGBA)", Color) = (0.7, 1, 1, 0)
		_EmissionColor("Emission color (RGBA)", Color) = (0.7, 1, 1, 0.01)
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 2000
		Cull Off

		CGPROGRAM
#pragma surface surf Lambert vertex:vert alpha
#pragma target 3.0

	struct Input {
		float customDist;
	};

	float4 _Position;
	float _MaxDistance;
	float4 _ShieldColor;
	float4 _EmissionColor;

	float _Amount;

	void vert(inout appdata_full v, out Input o) {
		o.customDist = distance(_Position.xyz, v.vertex.xyz);
	}

	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = _ShieldColor.rgb;
		o.Emission = _EmissionColor;
		o.Alpha = 0 - (IN.customDist / _MaxDistance) + _ShieldColor.a;
	}

	ENDCG
	}
		Fallback "Transparent/Diffuse"
}